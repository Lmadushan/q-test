using System.Data;
using System.Data.SqlClient;

namespace QualitAppsTest.Infrastructure.Database
{
    public class MSSqlAdapter : IMssqlAdapter
    {
        SqlConnection sqlConnection;
        SqlTransaction sqlTransaction;
        //open connection
        public bool OpenConnection(string connectionString)
        {
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            return true;
        }

        //close connection
        public bool CloseConnection()
        {
            sqlTransaction = null;
            sqlConnection.Close();
            sqlConnection = null;
            return true;
        }

        //select query
        public SqlDataReader Select(string sql, List<SqlParameter> sqlParameters)
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                SqlCommand sqlCommand = new(sql, sqlConnection);
                if (sqlTransaction != null)
                {
                    sqlCommand.Transaction = sqlTransaction;
                }

                foreach (var sqlParam in sqlParameters)
                {
                    sqlCommand.Parameters.Add(sqlParam);
                }

                return sqlCommand.ExecuteReader();
            }
            throw new Exception("Database connection is not open.");
        }

        //insert/update/delete
        public int Execute(string sql, List<SqlParameter> sqlParameters)
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                SqlCommand sqlCommand = new(sql, sqlConnection);
                if (sqlTransaction != null)
                {
                    sqlCommand.Transaction = sqlTransaction;
                }

                foreach (var sqlParam in sqlParameters)
                {
                    sqlCommand.Parameters.Add(sqlParam);
                }

                return sqlCommand.ExecuteNonQuery();
            }
            throw new Exception("Database connection is not open.");
        }

        //begin transaction
        public bool BeginTransaction()
        {
            if (sqlConnection == null)
            {
                throw new Exception("Connection not open. Cannot begin transaction.");
            }

            if (sqlTransaction == null)
            {
                sqlTransaction = sqlConnection.BeginTransaction();
                return true;
            }
            else
            {
                throw new Exception("Transaction already started.");
            }
        }

        //commit transaction
        public bool CommitTransaction()
        {
            if (sqlConnection == null)
            {
                throw new Exception("Sql connection not initialized. Cannot commit transaction.");
            }

            if (sqlTransaction != null)
            {
                sqlTransaction.Commit();
                return true;
            }
            else
            {
                throw new Exception("Transaction not started. Cannot commit transaction.");
            }
        }

        //rollback transaction
        public bool RollbackTransaction()
        {
            if (sqlConnection == null)
            {
                throw new Exception("Sql connection not initialized. Cannot rollback transaction.");
            }

            if (sqlTransaction != null)
            {
                sqlTransaction.Rollback();
                return true;
            }
            else
            {
                throw new Exception("Transaction not started. Cannot rollback transaction.");
            }
        }

        #region helpers
        //to datatable
        public DataTable ConvertToDataTable(SqlDataReader sqlDataReader)
        {
            DataTable dataTable = new();
            dataTable.Load(sqlDataReader);
            return dataTable;
        }

        //is connection open
        public bool IsConnectionOpen()
        {
            return sqlConnection != null && sqlConnection.State == ConnectionState.Open;
        }
        #endregion
    }
}
