-- Cursors 
--1. Write a cursor to list all customers and how many rentals each made. Insert these into a summary table.
create table customer_rental_log (
	customer_id int,
	customer_name text,
	total_rentals int
);


select c.customer_id, c.first_name || ' ' || c.last_name as customer_name, count(r.rental_id) as total_rentals
from customer c join rental r on c.customer_id = r.customer_id group by c.customer_id, c.first_name, c.last_name;

do $$
declare
	rec record;
	cur cursor for
		select c.customer_id, c.first_name || ' ' || c.last_name as customer_name, count(r.rental_id) as total_rentals
		from customer c join rental r on c.customer_id = r.customer_id group by c.customer_id,c.first_name,c.last_name;
begin
	open cur;
	loop
	fetch cur into rec;
	exit when not found;

	insert into customer_rental_log(customer_id,customer_name,total_rentals)
	values (rec.customer_id,rec.customer_name, rec.total_rentals);
	
	end loop;
	close cur;
end;
$$;

select * from customer_rental_log;

-- 2. Using a cursor, print the titles of films in the 'Comedy' category rented more than 10 times.
select * from film;
select * from category;
select * from film_category;
select * from rental;
select * from inventory;
select f.title, count(r.rental_id) as rental_count
from film f
join film_category fc on f.film_id = fc.film_id
join category c on fc.category_id = c.category_id
join inventory i on f.film_id = i.film_id
join rental r on i.inventory_id = r.inventory_id
where c.name = 'Comedy'
group by f.title
having count(r.rental_id) > 10;

do $$
declare
	rec record;
	cur cursor for 
		select f.title, count(r.rental_id) as rental_count
		from film f
		join film_category fc on f.film_id = fc.film_id
		join category c on fc.category_id = c.category_id
		join inventory i on f.film_id = i.film_id
		join rental r on i.inventory_id = r.inventory_id
		where c.name = 'Comedy'
		group by f.title
		having count(r.rental_id) > 10;
begin
	open cur;
	loop
		fetch cur into rec;
		exit when not found;

		raise notice 'Title : %, Rentals : %', rec.title, rec.rental_count;
	end loop;
	close cur;
end
$$;

-- 3. Create a cursor to go through each store and count the number of distinct films available, and insert results into a report table.
select * from film;
select * from store;
select * from inventory;

create table store_film_log (
store_id int,
film_count int
);

select s.store_id, count(distinct i.film_id) as film_count
from store s
join inventory i on s.store_id = i.store_id
group by s.store_id;

do $$
declare
	rec record;
	cur cursor for
		select s.store_id, count(distinct i.film_id) as film_count
		from store s
		join inventory i on s.store_id = i.store_id
		group by s.store_id;
begin
	open cur;
	loop
		fetch cur into rec;
		exit when not found;

		insert into store_film_log(store_id,film_count)
		values (rec.store_id, rec.film_count);
	end loop;
	close cur;
end
$$;
drop table store_film_log;
select * from store_film_log;

-- 4. Loop through all customers who haven't rented in the last 6 months and insert their details into an inactive_customers table.
create table inactive_customers(
customer_id int,
full_name text,
email text,
last_rental_date date
);

select * from rental;
select * from customer;

do $$
declare 
	rec record;
	cur cursor for
		select c.customer_id, c.first_name || ' ' || c.last_name as full_name, c.email, max(r.rental_date) as last_rental_date
		from customer c
		left join rental r on c.customer_id = r.customer_id
		group by c.customer_id,c.first_name,c.last_name
		having max(r.rental_date) is null or max(r.rental_date) < current_Date - interval '6 months';
begin
	open cur;
	loop
		fetch cur into rec;
		exit when not found;

		insert into inactive_customers(customer_id,full_name,email,last_rental_date)
		values (rec.customer_id,rec.full_name,rec.email,rec.last_rental_date);
	end loop;
	close cur;
end
$$;
select * from inactive_customers;
--------------------------------------------------------------------------

-- Transactions 
--1. Write a transaction that inserts a new customer, adds their rental, and logs the payment – all atomically.
select * from customer order by customer_id desc;
select * from rental order by customer_id desc;
select * from payment order by customer_id desc;
-- start transaction
begin;
-- (1) insert a customer
with new_cust as (
	insert into customer (store_id,first_name,last_name,email,address_id,create_Date,active)
	values (
		1,
		'Lionel',
		'Messi',
		'lm10@gmail.com',
		1,
		current_timestamp,
		1
	)
	returning customer_id
),
-- (2) insert new rental with the created customer_id
 new_rental as (
	insert into rental (rental_date,inventory_id,customer_id,staff_id)
	select current_timestamp, 1,customer_id,1 from new_cust
	returning customer_id, rental_id
)
-- (3) insert payment log
insert into payment (customer_id,staff_id,rental_id,amount,payment_date)
select customer_id, 1,rental_id,5.99, current_timestamp from new_rental;

commit;

-- 2. Simulate a transaction where one update fails (e.g., invalid rental ID), and ensure the entire transaction rolls back.
select * from rental order by customer_id desc;
-- currently the return_date of rental_id = 16054 is null
begin;
-- valid update
update rental
set return_date = current_timestamp
where rental_id = 16054;

-- simulating failure
do $$
begin
	raise exception 'Simulated Failure : Invalid rental.';
end;
$$;

-- if both succeed -> commit. This line won't be reached if error occurrs
commit; 
rollback; --> when commit is executed due to failure rollback occurs
-- After this, no changes (not even the first UPDATE) will persist. The return_date is still null.


-- 3. Use SAVEPOINT to update multiple payment amounts. Roll back only one payment update using ROLLBACK TO SAVEPOINT.
select * from payment order by customer_id desc;
begin;
-- update 1 : payment of payment_id = 32101
update payment
set amount = amount + 2
where payment_id = 32101;

-- after_first_update savepoint
savepoint after_first_update;

-- update 2 : payment of payment_id = 32100 (assuming a problem)
update payment set amount = amount + 3 where payment_id = 32100;

-- rollback only second update
rollback to savepoint after_first_update;

-- update 3 : update payment id = 18464 (successful)
update payment set amount = amount + 1 where payment_id  = 18464;

-- this will commit all changes before and after savepoint. Except the one that is rolled back
commit;

-- 4. Perform a transaction that transfers inventory from one store to another (delete + insert) safely.
select * from inventory where inventory_id = 4583;
-- to move inventory_id - 4583 from store_id = 2 to store_id = 1
begin;
-- (1) to get details of inventory before deletion
select i.inventory_id, r.inventory_id from inventory i left join rental r on i.inventory_id = r.inventory_id where r.inventory_id is null;
delete from inventory where inventory_id = 4583;

-- (2) to insert into the target store
insert into inventory values (4583, 1, 1, current_timestamp);

commit;

-- 5. Create a transaction that deletes a customer and all associated records (rental, payment), ensuring referential integrity.
select * from customer order by customer_id desc;
begin;
do $$
declare
	cust_id int := 600;
begin
	-- payment
	delete from payment where customer_id = cust_id;

	-- rental
	delete from rental where customer_id = cust_id;

	-- customer
	delete from customer where customer_id = cust_id;
end
$$;
commit;
----------------------------------------------------------------------------

-- Triggers
-- 1. Create a trigger to prevent inserting payments of zero or negative amount.
-- trigger fn
create or replace function prevent_invalid_payment()
returns trigger as $$
begin 
	if new.amount <= 0 then
		raise exception 'Payment amount must be greater than zero. Attempted : %',new.amount;
	end if;
	return new;
end;
$$ language plpgsql;
-- attach trigger
drop trigger trg_check_payment_amount on payment;
create trigger trg_check_payment_amount
before insert on payment
for each row
execute function prevent_invalid_payment();

-- test
insert into payment(customer_id,staff_id,rental_id,amount,payment_date)
values (1,1,1,-50.00,now());

select * from payment;

-- 2. Set up a trigger that automatically updates last_update on the film table when the title or rental rate is changed.
-- trigger fn
create or replace function update_film_last_update()
returns trigger as $$
begin
	if new.title is distinct from old.title 
	or new.rental_rate is distinct from old.rental_rate then
	new.last_update := now();
	end if;
	return new;
end;
$$ language plpgsql;
-- attach trigger
create trigger trg_last_film_update
before update on film
for each row 
execute function update_film_last_update();

select * from film where film_id = 133;
-- test
update film set rental_rate = 2.99 where film_id = 133;

-- 3. Write a trigger that inserts a log into rental_log whenever a film is rented more than 3 times in a week.
------------------------------------------------------------------------------
CREATE TABLE rental_log (
    id SERIAL PRIMARY KEY,
    film_id INT,
    log_time TIMESTAMP DEFAULT now(),
    message TEXT
);


CREATE OR REPLACE FUNCTION log_high_rentals()
RETURNS TRIGGER AS $$
DECLARE
    v_film_id INT;
    v_rental_count INT;
BEGIN

    SELECT film_id INTO v_film_id
    FROM inventory
    WHERE inventory_id = NEW.inventory_id;

    SELECT COUNT(*) INTO v_rental_count
    FROM rental r
    JOIN inventory i ON r.inventory_id = i.inventory_id
    WHERE i.film_id = v_film_id
      AND r.rental_date >= (NOW() - INTERVAL '7 days');


    IF v_rental_count > 3 THEN
        INSERT INTO rental_log (film_id, message)
        VALUES (
            v_film_id,
            'Film ID ' || v_film_id || ' rented more than 3 times in the last week.'
        );
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER trg_log_high_rentals
AFTER INSERT ON rental
FOR EACH ROW
EXECUTE FUNCTION log_high_rentals();


INSERT INTO rental(rental_date, inventory_id, customer_id, staff_id)
VALUES(NOW(), 5, 2, 1);

INSERT INTO rental(rental_date, inventory_id, customer_id, staff_id)
VALUES(NOW(), 5, 2, 1);

INSERT INTO rental(rental_date, inventory_id, customer_id, staff_id)
VALUES(NOW(), 5, 2, 1);

SELECT * FROM rental_log;
