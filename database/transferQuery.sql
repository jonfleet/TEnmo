select * from transfers;

select a_from.user_id, , amount, transfer_types, from transfers as t
join accounts as a_from
on t.account_from = a_from.account_id
join accounts  as a_to
on t.account_to = a_to.account_id
join transfer_types as tt
on t.transfer_type_id = tt.transfer_type_id
join transfer_statuses as ts
on t.transfer_status_id = ts.transfer_status_id
join users as u 
on u.user_id = a.account_id

select * from users;
select * from transfers;
select * from accounts as a join users as u on u.user_id = a.user_id where u.username = 'test';

--  GetTransfers
select from_u.user_id as from_user, to_u.user_id ,transfer_id, transfer_type_id, transfer_status_id, account_from, account_to from transfers as t
join accounts as a
on a.account_id = t.account_from
join users as from_u
on from_u.user_id =  a.user_id
join users as to_u
on to_u.user_id = a.user_id
where from_u.username = 'test3' or to_u.username = 'test3';
--where u.username = 'test3';

--public string ToUserId { get; set; }
--public string FromUserId { get; set; }
--public decimal Amount { get; set; }
--public int TransferTypeId { get; set; }

-- GetTransfersById
select transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount from transfers as t
join accounts as a_from 
on a_from.account_id = t.account_from
join users as u
on u.user_id =  a_from.user_id
where u.username = 1 and transfer_id = 1;

--SendPayment 

Update users 
set users.balance = 1,
where user_id = 1,


begin transaction
Insert into transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) output INSERTED.transfer_id values ( (select transfer_type_id from transfer_types where transfer_type_desc = 'Send'), (select transfer_status_id from transfer_statuses where transfer_status_desc = 'Approved'), 1,  3, 45.50);
select * from transfers;
select * from transfer_types;
select * from transfer_statuses;
rollback

-- Decrement the Sender Balance
-- Increment the receiver Balance
 

