create table audit_log
(audit_id serial primary key,
table_name text,
field_name text,
old_value text,
new_value text,
updated_date timestamp default current_timestamp)

create or replace function Update_Audit_log()
returns trigger
as $$
begin
	insert into audit_log(table_name,field_name, old_value, new_value,updated_date)
	values ('customer','email',old.email,new.email,current_timestamp);
	return new;
end;
$$ language plpgsql;

create trigger trg_log_customer_email_change
before update
on customer
for each row 
execute function Update_Audit_log();

drop trigger trg_log_customer_email_change on customer;

select * from customer order by customer_id;
select * from audit_log;

update customer set email = 'mary.smith@sakilacustomer.org' where customer_id = 1;

create or replace function Update_Audit_Log()
returns trigger
as $$
declare
	col_name text := TG_ARGV[0];
	tab_name text := TG_ARGV[1];
	o_value text;
	n_value text;
begin
	o_value := row_to_json(old);
	n_value := row_to_json(new);
	if o_value is distinct from n_value then
		insert into audit_log(table_name,field_name,old_value,new_value,updated_date)
		values (tab_name, col_name, o_value, n_value, current_timestamp);
	end if;
	return new;
end;
$$ language plpgsql

drop trigger trg_log_customer_email_change on customer;

create trigger trg_log_customer_email_change
after update on customer
for each row execute function update_audit_log('last_name','customer');

update customer set last_name = 'smith' where customer_id = 1;

select * from customer order by customer_id;
select * from audit_log;

create or replace function Update_Audit_log()
returns trigger 
as $$
declare 
   col_name text := TG_ARGV[0];
   tab_name text := TG_ARGV[1];
   o_value text;
   n_value text;
begin
    EXECUTE FORMAT('select ($1).%I::TEXT', COL_NAME) INTO O_VALUE USING OLD;
    EXECUTE FORMAT('select ($1).%I::TEXT', COL_NAME) INTO N_VALUE USING NEW;
	if o_value is distinct from n_value then
		Insert into audit_log(table_name,field_name,old_value,new_value,updated_date) 
		values(tab_name,col_name,o_value,n_value,current_Timestamp);
	end if;
	return new;
end;
$$ language plpgsql
