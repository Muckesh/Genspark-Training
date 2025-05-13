1. Try two concurrent updates to same row → see lock in action.

-- Trans B
BEGIN;
UPDATE account SET balance = balance - 50 WHERE id = 1;
-- This will hang until Session A commits or rolls back
SELECT * FROM account;
COMMIT;

2. Write a query using SELECT...FOR UPDATE and check how it locks row.

-- Trans B
BEGIN;

UPDATE account
SET balance = balance + 50
WHERE id = 1;		--Writing on Locked record

-- Wait Until Trans A from session 1 is commited or Rolled Back
COMMIT;


3. Intentionally create a deadlock and observe PostgreSQL cancel one transaction.


--- Dead lock
-- trans b
begin;
update account set balance = 600 where id = 2;
-- B locks row 2

update account set balance = 100 where id = 1;
-- B locks row 1 already locked by B
-- it automatically aborts a transaction to resolve deadlock.

5. Explore about Lock Modes.

BEGIN;

UPDATE items SET name = 'Session 2 Update' WHERE id = 1;

-- advisory lock is used in session 1
COMMIT;


-- EXCLUSIVE MODE
BEGIN;

UPDATE items SET name = 'exclusive mode test' WHERE id = 1;

-- advisory lock is used in session 1
COMMIT;

-- ACCESS EXCLUSIVE MODE;
BEGIN;

SELECT * FROM items;

-- Had to wait until session 1 commits

COMMIT;