/*
	1. Create a stored procedure to encrypt a given text
	Task: Write a stored procedure sp_encrypt_text that takes a plain text input (e.g., email or mobile number) and returns an encrypted version using PostgreSQL's pgcrypto extension.
 
	Use pgp_sym_encrypt(text, key) from pgcrypto.
*/

create extension if not exists pgcrypto;

create or replace procedure sp_encrypt_text (
	in input_text text,
	in secret_key text,
	out encrypted_data bytea
)
language plpgsql			
as $$
begin
	encrypted_data := pgp_sym_encrypt(input_text,secret_key);
end;
$$;

call sp_encrypt_text('Hello','secret123',null);

do $$
declare
	encrypted_data bytea;
begin
	call sp_encrypt_text('Hello','secret123',encrypted_data);
	raise notice 'Encrypted Data : %',encrypted_data;
end;
$$;


/*
	2. Create a stored procedure to compare two encrypted texts
	Task: Write a procedure sp_compare_encrypted that takes two encrypted values and checks if they decrypt to the same plain text.
*/

create or replace procedure sp_compare_encrypted(
	in encrypted_1 bytea,
	in encrypted_2 bytea,
	in secret_key text,
	out is_equal boolean
)
language plpgsql
as $$
declare
	text1 text;
	text2 text;
begin
	text1:= pgp_sym_decrypt(encrypted_1,secret_key);
	text2:= pgp_sym_decrypt(encrypted_2,secret_key);
	is_equal:= (text1=text2);
end;
$$;

call sp_compare_encrypted('\xc30d040703024dba215bc23753a473d23c01c136ab9b082f45bf305a6c821af34443daa04ab71dd36646ea219f67705d8d4fbdd40a14b2588d789d6f649aa3b63d8f7c12cee150b2975c3ba108','\xc30d04070302de4b03c8594a69c77bd23601ce6ee0ee8024a746cdf5ec8185145b408bc3e841113ef9a6f1e818a10f2aa5bac39628c5c0e5a04b4ad8007c612df2312b4b640f24','secret123',null)

do $$
declare
	is_equal boolean;
begin
	call sp_compare_encrypted('\xc30d040703024dba215bc23753a473d23c01c136ab9b082f45bf305a6c821af34443daa04ab71dd36646ea219f67705d8d4fbdd40a14b2588d789d6f649aa3b63d8f7c12cee150b2975c3ba108',
	'\xc30d04070302de4b03c8594a69c77bd23601ce6ee0ee8024a746cdf5ec8185145b408bc3e841113ef9a6f1e818a10f2aa5bac39628c5c0e5a04b4ad8007c612df2312b4b640f24',
	'secret123',
	is_equal);
	if is_equal then
	raise notice 'The two encrypted values decrypts to same plain text';
	else
	raise notice 'The two encrypted values do not decrypts to same plain text';
	end if;
end;
$$;

/* 
	3. Create a stored procedure to partially mask a given text
	Task: Write a procedure sp_mask_text that:
 
	Shows only the first 2 and last 2 characters of the input string
 
	Masks the rest with *
 
	E.g., input: 'john.doe@example.com' → output: 'jo***************om'
*/

create or replace procedure sp_mask_text (
	in input_text text,
	out masked_text text
)
language plpgsql
as $$
declare 
	len int;
begin
	len:=length(input_text);
	if len <= 4 then
	masked_text := input_text;
	else
		masked_text := substring(input_text from 1 for 2) ||
						repeat('*',len-4) || substring(input_text from len-1 for 2);
	end if;
end;
$$;

call sp_mask_text('janesmith@gmail.com',null);

do $$
declare
	masked_text text;
begin
	call sp_mask_text('janesmith@gmail.com',masked_text);
	raise notice 'Masked email : %',masked_text;
end;
$$;

/* 
	4. Create a procedure to insert into customer with encrypted email and masked name
	Task:
 
	Call sp_encrypt_text for email
 
	Call sp_mask_text for first_name
 
	Insert masked and encrypted values into the customer table
 
	Use any valid address_id and store_id to satisfy FK constraints.
*/
drop table customer;


create table customer (
	id serial primary key,
	name text,
	email bytea,
	address_id int default 1,
	store_id int default 1
)

create or replace procedure sp_insert_customer(
	in name text,
	in email text,
	in secret_key text
) language plpgsql
as $$
declare 
	masked_name text;
	encrypted_email bytea;
begin
	call sp_mask_text(name,masked_name);
	call sp_encrypt_text(email,secret_key,encrypted_email);

	insert into customer (name,email)
	values (masked_name,encrypted_email);
end;
$$;

call sp_insert_customer('John Smith','janesmith@gmail.com','secret123');

select * from customer;

/*
	5. Create a procedure to fetch and display masked first_name and decrypted email for all customers
	Task:
	Write sp_read_customer_masked() that:
 
	Loops through all rows
 
	Decrypts email
 
	Displays customer_id, masked first name, and decrypted email
*/

create or replace procedure sp_read_customer_masked (
	in secret_key text
)language plpgsql
as $$
declare
	decrypted_email text;
	rec record;
	cur cursor for
	select * from customer;
begin
	open cur;
	loop
	fetch cur into rec;
	exit when not found;
		decrypted_email := pgp_sym_decrypt(rec.email,secret_key);
		raise Notice 'Customer ID : %, Name : %, Email : %.',rec.id,rec.name,decrypted_email;
	end loop;
	close cur;
end;
$$;

call sp_read_customer_masked('secret123');



---------------------

do
$$
declare
	encrypted_data bytea;
	decrypted_data text;
begin
	call sp_encrypt_text('Hello','secret123',encrypted_data);
	raise notice 'Encrypted data : %',encrypted_data;

	call sp_decrypt_text(encrypted_data,'secret123',decrypted_data);
	raise notice 'Decrypted data : %',decrypted_data;
end;
$$;

create or replace procedure sp_decrypt_text (
	in encrypted_text bytea,
	in secret_key text,
	out decrypted_data text
)
language plpgsql
as $$
begin
	decrypted_data := pgp_sym_decrypt(encrypted_text,secret_key);
end;
$$;