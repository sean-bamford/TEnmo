using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDao : IAccountDao
    {
        private readonly string connectionString;
        const decimal startingBalance = 1000;

        public AccountSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Account GetBalance(int userId)
        {
            Account account = new Account();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("select a.balance from account a INNER JOIN tenmo_user tu ON a.user_id = tu.user_id WHERE tu.user_id = @UserId;", conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    account.Balance = Convert.ToDecimal(cmd.ExecuteScalar());              
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return account;
        }
    }
}

