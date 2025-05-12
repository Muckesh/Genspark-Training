select * from tbl_bank_accounts
-- Trans B
begin;
select balance from tbl_bank_accounts where account_id = 1;
-- concurrent transaction in Trans B
begin;
UPDATE tbl_bank_accounts SET balance = balance + 500 WHERE account_name = 'Alice';
commit;
---
abort;
-- Trans B
begin;
update tbl_bank_accounts set balance = 100 where account_id =1;
commit; -- updated to new value only in this instance

-- Phantom reads and serialization
-- Transaction A
BEGIN ISOLATION LEVEL SERIALIZABLE;
SELECT * FROM tbl_bank_accounts WHERE balance > 1000;

-- Transaction B
BEGIN;
INSERT INTO tbl_bank_accounts (account_name, balance) VALUES ('Eve', 2000);
COMMIT;

-- Back to Transaction A
SELECT * FROM tbl_bank_accounts WHERE balance > 1000;  --  This will raise an error or block
COMMIT;
abort;
-- Trans B
BEGIN;
INSERT INTO Accounts
(id, balance)
VALUES
(4, 500);
COMMIT;
SELECT * FROM Accounts
