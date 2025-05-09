-- Cursor-Based Questions (5)
-- 1. Write a cursor that loops through all films and prints titles longer than 120 minutes.

DO
$$
DECLARE film_cursor CURSOR
FOR SELECT title, length FROM film;
record RECORD;
BEGIN
	OPEN film_cursor;
	LOOP
		FETCH film_cursor INTO record;
		EXIT WHEN NOT FOUND;
		IF record.length > 120 THEN
		RAISE NOTICE 'Movie Name is % of length % mins',record.title, record.length;
		END IF;
	END LOOP;
	CLOSE film_cursor;
END
$$;


-- 2. Create a cursor that iterates through all customers and counts how many rentals each made.

DO $$
DECLARE customer_cursor CURSOR
FOR SELECT customer_id, first_name, last_name FROM customer;
each_customer RECORD;
totalCount INT;
BEGIN
	OPEN customer_cursor;
	LOOP
		FETCH customer_cursor INTO each_customer;
		EXIT WHEN NOT FOUND;
		SELECT COUNT(*) INTO totalCount
		FROM rental
		WHERE customer_id = each_customer.customer_id;
		RAISE NOTICE 'id: % Name: % Total_Rental: %',each_customer.customer_id, each_customer.first_name || ' ' || each_customer.last_name, totalCount;
	END LOOP;
END;
$$

-- 3. Using a cursor, update rental rates: Increase rental rate by $1 for films with less than 5 rentals.
DO $$
DECLARE
    film_rec RECORD;
    rental_count INT;
    film_cursor CURSOR FOR
        SELECT film_id, title, rental_rate FROM film;
BEGIN
    OPEN film_cursor;

    LOOP
        FETCH film_cursor INTO film_rec;
        EXIT WHEN NOT FOUND;

        -- Count how many times the film was rented
        SELECT COUNT(*) INTO rental_count
        FROM inventory i
        JOIN rental r ON i.inventory_id = r.inventory_id
        WHERE i.film_id = film_rec.film_id;

        -- If rented less than 5 times, increase rental rate
        IF rental_count < 5 THEN
            UPDATE film
            SET rental_rate = rental_rate + 1
            WHERE film_id = film_rec.film_id;

            RAISE NOTICE 'Updated film: %, New rate: %',
                film_rec.title, film_rec.rental_rate + 1;
        END IF;
    END LOOP;

    CLOSE film_cursor;
END;
$$;


-- 4. Create a function using a cursor that collects titles of all films from a particular category.
CREATE OR REPLACE FUNCTION get_film_titles_by_category(cat_name TEXT)
RETURNS TABLE(title TEXT)
LANGUAGE plpgsql
AS $$
DECLARE
    film_rec RECORD;
    film_cursor CURSOR FOR
        SELECT f.title
        FROM film f
        JOIN film_category fc ON f.film_id = fc.film_id
        JOIN category c ON fc.category_id = c.category_id
        WHERE c.name = cat_name;
BEGIN
    OPEN film_cursor;

    LOOP
        FETCH film_cursor INTO film_rec;
        EXIT WHEN NOT FOUND;

        -- Use a row assignment for the RETURN NEXT
        title := film_rec.title;
        RETURN NEXT;
    END LOOP;

    CLOSE film_cursor;
END;
$$;

SELECT * FROM get_film_titles_by_category('Action');


-- 5. Loop through all stores and count how many distinct films are available in each store using a cursor.
DO $$
DECLARE
    store_rec RECORD;
    film_count INT;
    store_cursor CURSOR FOR
        SELECT store_id, address_id FROM store;
BEGIN
    OPEN store_cursor;

    LOOP
        FETCH store_cursor INTO store_rec;
        EXIT WHEN NOT FOUND;

        -- Count distinct films in each store
        SELECT COUNT(DISTINCT inventory.film_id)
        INTO film_count
        FROM inventory
        WHERE inventory.store_id = store_rec.store_id;

        -- Print the result
        RAISE NOTICE 'Store ID: %, Address ID: %, Distinct Films: %',
            store_rec.store_id, store_rec.address_id, film_count;
    END LOOP;

    CLOSE store_cursor;
END;
$$;

-- Trigger-Based Questions (5)
-- 1. Write a trigger that logs whenever a new customer is inserted.
-- AFTER is also known as FOR
-- log table
CREATE TABLE customer_log (
    log_id SERIAL PRIMARY KEY,
    customer_id INT,
    name TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
-- trigger function
CREATE OR REPLACE FUNCTION log_new_customer()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO customer_log (customer_id, name)
    VALUES (NEW.customer_id, NEW.first_name || ' ' || NEW.last_name);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- trigger on customer table
CREATE TRIGGER trg_log_customer_insert
AFTER INSERT ON customer
FOR EACH ROW
EXECUTE FUNCTION log_new_customer();

select * from customer_log

INSERT INTO customer (store_id, first_name, last_name, email, address_id, active, create_date)
VALUES (1, 'John', 'Doe', 'johndoe@example.com', 1, 1, CURRENT_DATE);

-- 2. Create a trigger that prevents inserting a payment of amount 0.

-- trigger fn
CREATE OR REPLACE FUNCTION prevent_zero_payment()
RETURNS TRIGGER AS $$
BEGIN
	IF NEW.amount = 0 THEN
		RAISE EXCEPTION 'Payment amount cannot be 0';
	END IF;
	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- create the trigger
CREATE TRIGGER trg_prevent_zero_payment
BEFORE INSERT ON payment FOR EACH ROW
EXECUTE FUNCTION prevent_zero_payment();

-- test
INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
VALUES (1, 1, 1, 0, CURRENT_TIMESTAMP);

-- 3. Set up a trigger to automatically set last_update on the film table before update.

-- trigger fn
CREATE OR REPLACE FUNCTION update_last_update()
RETURNS TRIGGER AS $$
BEGIN
	NEW.last_update := CURRENT_TIMESTAMP;
	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- create trigger
CREATE TRIGGER trg_update_last_update BEFORE UPDATE ON film
FOR EACH ROW
EXECUTE FUNCTION update_last_update();

-- example
UPDATE film
SET title = 'New Movie Title'
WHERE film_id = 1;

select * from film order by film_id;

-- 4. Create a trigger to log changes in the inventory table (insert/delete).

-- log table
CREATE TABLE inventory_log (
    log_id SERIAL PRIMARY KEY,
    action_type TEXT,  -- 'INSERT' or 'DELETE'
    film_id INT,
    store_id INT,
    action_timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- trigger function
CREATE OR REPLACE FUNCTION log_inventory_changes()
RETURNS TRIGGER AS $$
BEGIN
    -- Log the insert action
    IF TG_OP = 'INSERT' THEN
        INSERT INTO inventory_log (action_type, film_id, store_id)
        VALUES ('INSERT', NEW.film_id, NEW.store_id);
    -- Log the delete action
    ELSIF TG_OP = 'DELETE' THEN
        INSERT INTO inventory_log (action_type, film_id, store_id)
        VALUES ('DELETE', OLD.film_id, OLD.store_id);
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- create trigger 
CREATE TRIGGER trg_inventory_insert
AFTER INSERT ON inventory
FOR EACH ROW
EXECUTE FUNCTION log_inventory_changes();

CREATE TRIGGER trg_inventory_delete
AFTER DELETE ON inventory
FOR EACH ROW
EXECUTE FUNCTION log_inventory_changes();

-- test

INSERT INTO inventory (film_id, store_id)
VALUES (1, 2);

-- DELETE FROM inventory WHERE inventory_id = 1;
-- log
SELECT * FROM inventory_log ORDER BY action_timestamp DESC;

-- 5. Write a trigger that ensures a rental can’t be made for a customer who owes more than $50.
-- trigger fn

CREATE OR REPLACE FUNCTION prevent_rental_if_debt_exceeds()
RETURNS TRIGGER AS $$
DECLARE
    total_paid NUMERIC;
    total_rented NUMERIC;
    total_due NUMERIC;
BEGIN
    -- Calculate total amount paid by the customer
    SELECT COALESCE(SUM(amount), 0) INTO total_paid
    FROM payment
    WHERE customer_id = NEW.customer_id;

    -- Approximate total amount due (assuming each rental should be paid for)
    SELECT COUNT(*) * 4.99 INTO total_rented  -- assuming $4.99 per rental
    FROM rental
    WHERE customer_id = NEW.customer_id;

    total_due := total_rented - total_paid;

    IF total_due > 50 THEN
        RAISE EXCEPTION 'Customer ID % has outstanding dues of $%. Rental not allowed.', NEW.customer_id, total_due;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- create trigger
CREATE TRIGGER check_customer_debt_before_rental
BEFORE INSERT ON rental
FOR EACH ROW
EXECUTE FUNCTION prevent_rental_if_debt_exceeds();

-- test 
INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id)
VALUES (CURRENT_TIMESTAMP, 1, 1, 1);

-- Transaction-Based Questions (5)
-- 1. Write a transaction that inserts a customer and an initial rental in one atomic operation.

BEGIN;

-- Step 1: Insert new customer and get customer_id
WITH new_customer AS (
    INSERT INTO customer (store_id, first_name, last_name, email, address_id, active)
    VALUES (1, 'Jon', 'Doe', 'johndoe@example.com', 5, 1)
    RETURNING customer_id
)

-- Step 2: Insert new rental using the newly inserted customer_id
INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id)
SELECT CURRENT_TIMESTAMP, 10, customer_id, 1
FROM new_customer;

COMMIT;


-- 2. Simulate a failure in a multi-step transaction (update film + insert into inventory) and roll back.
BEGIN;

-- Update a film
UPDATE film SET rental_rate = rental_rate + 1 WHERE film_id = 1;

-- Simulate error during inventory insert (e.g., missing NOT NULL field)
INSERT INTO inventory (film_id, store_id) VALUES (1, NULL); -- store_id NULL violates NOT NULL constraint

-- This line won't be reached. Rollback happens due to the error above.
ROLLBACK;

--3. Create a transaction that transfers an inventory item from one store to another.

BEGIN;

-- Example: Move inventory item ID 100 from store 1 to store 2
UPDATE inventory
SET store_id = 2
WHERE inventory_id = 100 AND store_id = 1;

-- Optionally log the transfer
INSERT INTO inventory_log (inventory_id, from_store, to_store, transfer_date)
VALUES (100, 1, 2, CURRENT_TIMESTAMP);

COMMIT;
