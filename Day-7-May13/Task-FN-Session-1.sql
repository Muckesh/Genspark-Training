1. Try two concurrent updates to same row → see lock in action.
-- Trans A
BEGIN;

UPDATE account
SET balance = balance + 50
WHERE id = 1;

SELECT * FROM account;		-- temp updates but uncommitted

COMMIT;

2. Write a query using SELECT...FOR UPDATE and check how it locks row.
 -- Trans A
BEGIN;

SELECT balance 
FROM account
WHERE id = 1
FOR UPDATE; 		-- Lock Acquired

COMMIT;

3. Intentionally create a deadlock and observe PostgreSQL cancel one transaction.


--- Dead lock
-- trans a
begin;
update account set balance = 900 where id = 1;
-- A locks row 1
update account set balance = 700 where id = 2;
-- A locks row 2 which is already locked by B
commit;

4. Use pg_locks query to monitor active locks.
BEGIN;
-- Lock first row
UPDATE items SET name = 'Item A updated by T1' WHERE id = 1;

SELECT * FROM pg_locks;

END;

5. Explore about Lock Modes.

BEGIN;

SELECT pg_advisory_lock(123);
	UPDATE items SET name = 'Custom lock' WHERE id = 1;

SELECT pg_advisory_unlock(123);

COMMIT;

-- EXCLUSIVE MODE

BEGIN;

LOCK TABLE items IN EXCLUSIVE MODE;


COMMIT;

-- ACCESS EXCLUSIVE MODE;
BEGIN;

LOCK TABLE items IN ACCESS EXCLUSIVE MODE;


