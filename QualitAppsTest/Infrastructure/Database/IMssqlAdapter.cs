using System.Data;
using System.Data.SqlClient;

namespace QualitAppsTest.Infrastructure.Database
{
    public interface IMssqlAdapter
    {
        bool OpenConnection(string connectionString);

        bool CloseConnection();

        SqlDataReader Select(string sql, List<SqlParameter> sqlParameters);

        int Execute(string sql, List<SqlParameter> sqlParameters);

        bool BeginTransaction();

        bool CommitTransaction();

        bool RollbackTransaction();

        DataTable ConvertToDataTable(SqlDataReader sqlDataReader);

        bool IsConnectionOpen();
    }
}
