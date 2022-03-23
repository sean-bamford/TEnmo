using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDao : ITransferDao
    {
        private readonly string connectionString;

        public TransferSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Transfer SendMoney(Transfer transfer)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("BEGIN TRANSACTION BEGIN TRY INSERT INTO transfer(transfer_type_id, transfer_status_id, account_from, account_to, amount) OUTPUT INSERTED.transfer_id VALUES(2, 2, (select a.account_id Account_From from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = @SenderId), (select a.account_id Account_To from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = @ReceiverId), @Amount) UPDATE account SET balance -= @Amount WHERE account_id = (select a.account_id Account_From from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = @SenderId); UPDATE account SET balance += @Amount WHERE account_id = (select a.account_id Account_To from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = @ReceiverId); COMMIT TRANSACTION; END TRY BEGIN CATCH; ROLLBACK; END CATCH; ", conn);
                    cmd.Parameters.AddWithValue("@SenderId", transfer.SenderId);
                    cmd.Parameters.AddWithValue("@ReceiverId", transfer.ReceiverId);
                    cmd.Parameters.AddWithValue("@Amount", transfer.Amount);
                    transfer.TransferId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException se)
            { //se.Message
                throw;
            }
            return transfer;
        }

        public Transfer RequestMoney(Transfer transfer)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("BEGIN TRANSACTION BEGIN TRY INSERT INTO transfer(transfer_type_id, transfer_status_id, account_from, account_to, amount) OUTPUT INSERTED.transfer_id VALUES(1, 1, (select a.account_id Account_From from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = @SenderId), (select a.account_id Account_To from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = @ReceiverId), @Amount); COMMIT TRANSACTION; END TRY BEGIN CATCH; ROLLBACK; END CATCH; ", conn);
                    cmd.Parameters.AddWithValue("@SenderId", transfer.SenderId);
                    cmd.Parameters.AddWithValue("@ReceiverId", transfer.ReceiverId);
                    cmd.Parameters.AddWithValue("@Amount", transfer.Amount);
                    transfer.TransferId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException se)
            { //se.Message
                throw;
            }
            return transfer;

        }

        public bool ApproveRejectTransfer(Transfer transfer)
        {
            bool result = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open(); //approve
                    if (transfer.TransferStatus == "Approved")
                    {
                        SqlCommand cmd = new SqlCommand("BEGIN TRANSACTION BEGIN TRY UPDATE transfer SET transfer_status_id = 2 WHERE transfer_id = @TransferId; UPDATE account SET balance -= @Amount WHERE account_id = (select a.account_id Account_From from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = @SenderId); UPDATE account SET balance += @Amount WHERE account_id = (select a.account_id Account_To from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = @ReceiverId); COMMIT TRANSACTION; END TRY BEGIN CATCH; ROLLBACK; END CATCH; ", conn);
                        cmd.Parameters.AddWithValue("@SenderId", transfer.SenderId);
                        cmd.Parameters.AddWithValue("@ReceiverId", transfer.ReceiverId);
                        cmd.Parameters.AddWithValue("@Amount", transfer.Amount);
                        cmd.Parameters.AddWithValue("@TransferId", transfer.TransferId);
                        result = (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0);
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("BEGIN TRANSACTION BEGIN TRY UPDATE transfer SET transfer_status_id = 3 WHERE transfer_id = @TransferId; COMMIT TRANSACTION; END TRY BEGIN CATCH; ROLLBACK; END CATCH;", conn);
                        cmd.Parameters.AddWithValue("@TransferId", transfer.TransferId);
                        result = (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0);
                    }
                }
            }
            catch (SqlException se)
            { //se.Message
                throw;
            }
            return result;
        }

        public List<Transfer> GetTransfers(int userId)
        {
            List<Transfer> transfers = new List<Transfer>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("select t.transfer_id, tuu.username AS sender_name, tuu.user_id AS sender_id, tu.username AS receiver_name, tu.user_id AS receiver_id, ts.transfer_status_desc AS transfer_status, tt.transfer_type_desc as transfer_type, t.amount from transfer t INNER JOIN transfer_status ts ON t.transfer_status_id = ts.transfer_status_id INNER JOIN transfer_type tt ON tt.transfer_type_id = t.transfer_type_id INNER JOIN account a ON a.account_id = t.account_to INNER JOIN account aa ON aa.account_id = t.account_from INNER JOIN tenmo_user tu ON tu.user_id = a.user_id INNER JOIN tenmo_user tuu ON tuu.user_id = aa.user_id WHERE tu.user_id = @UserId OR tuu.user_id = @UserId;", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Transfer transfer = new Transfer();
                    transfer.TransferId = Convert.ToInt32(reader["transfer_id"]);
                    transfer.SenderName = Convert.ToString(reader["sender_name"]); // user id on account_from, 
                    transfer.SenderId = Convert.ToInt32(reader["sender_id"]);
                    transfer.ReceiverName = Convert.ToString(reader["receiver_name"]); // user id on account_to
                    transfer.ReceiverId = Convert.ToInt32(reader["receiver_id"]);
                    transfer.TransferStatus = Convert.ToString(reader["transfer_status"]);
                    transfer.TransferType = Convert.ToString(reader["transfer_type"]);
                    transfer.Amount = Convert.ToDecimal(reader["amount"]);
                    transfers.Add(transfer);
                }
            }
            return transfers;
        }
    }
}

