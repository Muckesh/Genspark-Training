-- Write a transaction that inserts a new customer, adds their rental, and logs the payment – all atomically.

select * from customer;
select * from rental order by inventory;
select * from payment;
create or replace procedure proc_create_cust_rental_payment (
p_first_name text, p_last_name text, p_email text, p_address_id int,
p_inventory_id int, p_store_id int, p_staff_id int, p_amount numeric
)
language plpgsql
as $$
declare
	v_customer_id int;
	v_rental_id int;
begin
	begin
	-- cust
	insert into customer (store_id,first_name,last_name,email,address_id,create_date,active)
	values (p_store_id,p_first_name,p_last_name,p_email,p_address_id,current_timestamp,1)
	returning customer_id into v_customer_id;

	-- rental
	insert into rental (rental_date,inventory_id,customer_id,staff_id)
	values (current_timestamp, p_inventory_id, v_customer_id,p_staff_id)
	returning rental_id into v_rental_id;
	
	-- payment
	insert into payment (customer_id,staff_id,rental_id,amount,payment_date)
	values (v_customer_id, p_staff_id, v_rental_id,p_amount,current_timestamp);

	Exception when others then
		raise notice 'Transaction failed %',sqlerrm;
	
	end;
end;
$$;

select * from customer order by customer_id  desc

call proc_create_cust_rental_payment ('Ram','Som','ram_som@gmail.com',1,1,1,1,10)

--------------------------------------

-- loop through all the films and update the rental rate by +1 for teh films when rental count < 5
select * from film;
select * from rental;
select * from inventory;
create or replace procedure proc_update_rental_rate()
language plpgsql
as $$
declare 
	rec record;
	cur cursor for 
		select f.film_id, f.rental_rate, count(r.rental_id) as rental_count
		from film f left join inventory i on  f.film_id = i.film_id
		left join rental r on i.inventory_id = r.inventory_id
		group by f.film_id, f.rental_rate  order by rental_count;
begin
	open cur;
	loop
		fetch cur into rec;
		exit when not found;

		if rec.rental_count > 5 then
		update film set rental_rate = rec.rental_rate + 1
		where film_id = rec.film_id;

		raise notice 'Updated film with id : %. The new rental rate is : %',rec.film_id,rec.rental_rate + 1;
		end if;
		end loop;
		close cur;
end;
$$;

call proc_update_rental_rate();
-----------------------------------------------------------------------
