select * from transfers;

select username, a.user_id, amount, transfer_types, from transfers as t
join accounts as a 
on t.account_from = a.account_id
--join accounts 
--on t.account_to = a.account_id
join transfer_types as tt
on t.transfer_type_id = tt.transfer_type_id
join transfer_statuses as ts
on t.transfer_status_id = ts.transfer_status_id
join users as u 
on u.user_id = a.account_id

