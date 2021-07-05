Use Tenmo;
DELETE FROM accounts;
DELETE FROM transfer_statuses;
DELETE FROM transfer_types;
DELETE FROM transfers;
DELETE FROM users;

-- Add Transfer Statues
SET IDENTITY_INSERT transfer_statuses ON;
INSERT INTO transfer_statuses (transfer_status_id,transfer_status_desc)
VALUES (1,'Pending'),
	   (2,'Approved'),
	   (3,'Rejected');
SET IDENTITY_INSERT transfer_statuses OFF;

-- Add Transfer Types
SET IDENTITY_INSERT transfer_types ON;
INSERT INTO transfer_types (transfer_type_id, transfer_type_desc)
VALUES (1,'Request'),
       (2,'Send');
SET IDENTITY_INSERT transfer_types OFF;

-- Add Users
SET IDENTITY_INSERT users ON;
INSERT INTO users (user_id, username, password_hash, salt)
VALUES (1, 'test1', 'password1', '123'),
       (2, 'test2', 'password2', '123'),
	   (3, 'test3', 'password3', '123');
SET IDENTITY_INSERT employee OFF;

-- Add Accounts

SET IDENTITY_INSERT accounts ON;
INSERT INTO accounts (account_id, user_id, balance)
VALUES (1,1, 1000),
       (2,2, 1000),
       (3, 3, 1000);
SET IDENTITY_INSERT accounts OFF;

-- Add Transfers
SET IDENTITY_INSERT transfers ON;
INSERT INTO transfers (transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount)
VALUES (1, 1, 2, 1 , 2, 100),
	   (2, 1, 2, 1 , 3 , 200),
	   (3, 1, 2, 2, 3, 300);
SET IDENTITY_INSERT transfers OFF;


DBCC CHECKIDENT('transfers', reseed, 10); -- make sure there's no chance new records will have conflicting ids
