select * from tenmo_user

select a.account_id Account_From from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = @;

select a.account_id Account_To from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = 1002;

BEGIN TRANSACTION

BEGIN TRY
INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) OUTPUT INSERTED.transfer_id
VALUES (2, 2, (select a.account_id Account_From from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = 1001), (select a.account_id Account_To from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = 1002), 500)

UPDATE account SET balance -= 500 WHERE account_id = (select a.account_id Account_From from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = 1001);
UPDATE account SET balance += 500 WHERE account_id =  (select a.account_id Account_To from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = 1002);
END TRY
BEGIN CATCH;
ROLLBACK; 
END CATCH;
COMMIT TRANSACTION;

INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount)
VALUES (@TransferTypeId, @TransferStatusId, @AccountFrom, @AccountTo, @Amount) 
select t.transfer_id from transfer t INNER JOIN account a ON t.account_from = a.account_id
select * from transfer