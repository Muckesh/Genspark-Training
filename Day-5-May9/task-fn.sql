-- SELECT Queries
-- List all films with their length and rental rate, sorted by length descending.
--Columns: title, length, rental_rate
select * from film
select title, length, rental_rate from film  order by length desc;

-- Find the top 5 customers who have rented the most films.
-- Hint: Use the rental and customer tables.
select * from rental
select * from customer
select c.customer_id, c.first_name, c.last_name, count(r.rental_id) as total_rentals
from customer c join rental r on c.customer_id = r.customer_id
group by c.customer_id, c.first_name, c.last_name
order by total_rentals desc limit 5;

-- Display all films that have never been rented.
-- Hint: Use LEFT JOIN between film and inventory → rental.
select * from film;
select * from inventory;
select * from rental;

select f.title from film f left join inventory i on f.film_id = i.film_id
left join rental r on i.inventory_id = r.inventory_id where r.rental_id is null;

-- JOIN Queries
-- List all actors who appeared in the film ‘Academy Dinosaur’.
-- Tables: film, film_actor, actor
select * from actor;
select * from film;
select * from film_actor;

select a.first_name, a.last_name,f.title from actor a join film_actor fa on a.actor_id = fa.actor_id
join film f on fa.film_id = f.film_id where f.title = 'Academy Dinosaur';

-- List each customer along with the total number of rentals they made and the total amount paid.
-- Tables: customer, rental, payment
select * from customer;
select * from rental;
select * from payment;
select c.first_name || ' ' || c.last_name as CustomerName, count(distinct r.rental_id) as TotalRentals, sum(p.amount) as TotalAmountPaid
from customer c left join rental r on c.customer_id = r.customer_id
left join payment p on r.customer_id = p.customer_id
group by c.first_name, c.last_name
order by TotalAmountPaid desc;

-- CTE-Based Queries
-- Using a CTE, show the top 3 rented movies by number of rentals.
-- Columns: title, rental_count
select * from film
select * from rental
select * from inventory
with film_rental_counts as (
 select f.title, count(r.rental_id) as rental_count
 from film f join inventory i on f.film_id = i.film_id
 join rental r on i.inventory_id = r.inventory_id
 group by f.title
 order by rental_count desc
)
select title, rental_count from film_rental_counts limit 3

-- Find customers who have rented more than the average number of films.
-- Use a CTE to compute the average rentals per customer, then filter.
select * from rental
with customer_rentals as (
	select customer_id,
	count(rental_id) as rental_count
	from rental group by customer_id
),
average_rentals as (
select avg(rental_count) as avg_rentals from customer_rentals
)select cr.customer_id, cr.rental_count from customer_rentals cr, average_rentals ar 
where cr.rental_count > ar.avg_rentals
order by cr.rental_count desc

-- Function Questions
-- Write a function that returns the total number of rentals for a given customer ID.
-- Function: get_total_rentals(customer_id INT)
create or replace function get_total_rentals(customer_id INT)
returns int as $$
declare
    total_rentals INT;
begin
    select count(*) into total_rentals
    from rental
    where rental.customer_id = get_total_rentals.customer_id;

    return total_rentals;
end;
$$ language plpgsql;

-- Stored Procedure Questions
-- Write a stored procedure that updates the rental rate of a film by film ID and new rate.
-- Procedure: update_rental_rate(film_id INT, new_rate NUMERIC)
create or replace procedure update_rental_rate(film_id INT, new_rate NUMERIC)
language plpgsql
as $$
begin
    update film
    set rental_rate = new_rate
    where film_id = update_rental_rate.film_id;
    
    raise notice 'Rental rate updated for film ID % to %', film_id, new_rate;
end;
$$;

-- Write a procedure to list overdue rentals (return date is NULL and rental date older than 7 days).
-- Procedure: get_overdue_rentals() that selects relevant columns.
create or replace procedure get_overdue_rentals()
LANGUAGE plpgsql
AS $$
BEGIN
    -- This uses RETURN QUERY for a procedure that selects rows
    -- You can also use a function if you want to return a table
    RAISE NOTICE 'Overdue rentals:';
    -- In a real application, you might instead use a cursor or RETURN QUERY
    -- Here, we use a temporary table-like output via RAISE NOTICE or SELECT

    -- Direct SELECT for simplicity:
    SELECT r.rental_id, r.customer_id, r.inventory_id, r.rental_date
    FROM rental r
    WHERE r.return_date IS NULL
      AND r.rental_date < NOW() - INTERVAL '7 days';
END;
$$;
