
-- 12 May 2025: Transactions and Concurrency

/* 1. Question:
In a transaction, if I perform multiple updates and an error happens in the third statement, but I have not used SAVEPOINT, what will happen if I issue a ROLLBACK?
Will my first two updates persist? */
No, the first two update will not persist if I issued a ROLLBACK.
In PostgreSQL, a transaction is atomic - all-or-nothing. if an error occurs anywhere in the transaction and if we issue a rollback,
everything is undone, even the successful updates of the same transaction.
If we use SAVEPOINT we could recover partially.

/* 2. Question:
Suppose Transaction A updates Alice’s balance but does not commit. Can Transaction B read the new balance if the isolation level is set to READ COMMITTED?
*/
No, Under READ COMMITTED Isolation level Transaction B cannot read the new balance, since Transaction A has not been committed yet.
Under Read Committed isolation level,
-> only committed data is visible to other transactions.
-> Therefore, Transaction B will still see the old balance, not the uncommitted change of Alice balance in Transaction A.

/* 3. Question:
What will happen if two concurrent transactions both execute:
UPDATE tbl_bank_accounts SET balance = balance - 100 WHERE account_name = 'Alice';
at the same time? Will one overwrite the other?*/
PostgreSQL will serialize the updates using row level locking, so one transaction will wail and not overwrite the other.
under default isolation level -> read committed
-> Trans A begins and executes the update on alice row.
---> postgresql places a row level lock on that row.
-> Trans B tries to execute the same update.
----> it blocks (wait) until Trans A either commits or rollbacks.
-> Once A commits, B proceeds, reading the updated balance and sub 100 from the value.
-> No overwriting occurs

/* 4. Question:
If I issue ROLLBACK TO SAVEPOINT after_alice;, will it only undo changes made after the savepoint or everything?
*/
It will only undo the changes made after the savepoint named after_alice, not the entire transaction.

/* 5. Question:
Which isolation level in PostgreSQL prevents phantom reads?
*/
In PostgreSQL, the SERIALIZABLE Isolation level prevents phantom reads.

/* 6. Question:
Can Postgres perform a dirty read (reading uncommitted data from another transaction)?
*/
No, PostgreSQL cannot peform dirty reads -> it does not support the read uncommitted Isolation level. 
Even if we set SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
PostgreSql will treat it as,
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

/* 7. Question:
If autocommit is ON (default in Postgres), and I execute an UPDATE, is it safe to assume the change is immediately committed?
*/
Yes, if autocommit is on, any update, insert or delete is immediately committed automatically.
But, when we use Begin Manually , we are overriding autocommit and we must 
explicitly commit or rollback.

/* 8. Question:
If I do this:

BEGIN;
UPDATE accounts SET balance = balance - 500 WHERE id = 1;
-- (No COMMIT yet)
And from another session, I run:

SELECT balance FROM accounts WHERE id = 1;
Will the second session see the deducted balance?
*/
No, the second session will not see the deducted balance since the default isolation level is READ COMMITTED .
It will read the old value or the last committed value of the balance