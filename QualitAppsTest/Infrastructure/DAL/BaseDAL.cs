using QualitAppsTest.Common.Attribute;
using QualitAppsTest.Infrastructure.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Reflection;
using Dapper;
using System.Data.SqlClient;

namespace QualitAppsTest.Infrastructure.DAL;

public class BaseDAL
{
    private readonly IConfiguration configuration;
    private readonly ILogger<BaseDAL> logger;
    private readonly string ConnectionString;
    private readonly int LongQueryCommandTimeOut;

    protected BaseDAL(IConfiguration _configuration, ILogger<BaseDAL> _logger) :
        this(_configuration, _logger, "ConnectionString")
    {
    }

    protected BaseDAL(IConfiguration _configuration, ILogger<BaseDAL> _logger, string databaseName)
    {
        configuration = _configuration;
        logger = _logger;
        ConnectionString = configuration.GetConnectionString(databaseName);
        LongQueryCommandTimeOut = Convert.ToInt32(configuration.GetSection("Settings:LongQueryCommandTimeout").Value);
    }

    public T QueryWithResult<T>(string sql, CommandType commandType, object parameters = null)
    {
        try
        {
            using (var connection = OpenConnection())
            {
                return connection.QueryFirst<T>(sql: sql, param: parameters, commandType: commandType);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    public IEnumerable<T> QueryWithResultList<T>(string sql, CommandType commandType, object parameters = null)
    {
        try
        {
            using (var connection = OpenConnection())
            {
                return connection.Query<T>(sql: sql, param: parameters, commandType: commandType);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    public DynamicParameters ExecuteSPWithParams(string sql, DynamicParameters param)
    {
        try
        {
            using (var connection = OpenConnection())
            {
                connection.Execute(sql: sql, param: param, commandType: CommandType.StoredProcedure);
                return param;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Executes sp with params passed and returns single record result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sp"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<T> ExecSPWithReturnAsync<T>(string sp, object param)
    {
        try
        {
            using (var connection = await OpenConnectionAsync())
            {
                var dParam = new DynamicParameters(param);
                dParam.Add("RETURN_VALUE", 0, DbType.Int32, ParameterDirection.ReturnValue);
                logger.LogInformation("Executing sp {@sp} with {@params}", sp, param);
                var result = await connection.QueryFirstOrDefaultAsync<T>(sql: sp, param: dParam, commandType: CommandType.StoredProcedure);
                return dParam.Get<T>("RETURN_VALUE");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while executing sp {@sql} with {@param}", sp, param);
            throw;
        }
    }

    /// <summary>
    /// Executes sp with params passed and returns single record result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sp"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<T> ExecSPWithResultAsync<T>(string sp, object param)
    {
        try
        {
            using (var connection = await OpenConnectionAsync())
            {
                var dParam = new DynamicParameters(param);
                logger.LogInformation("Executing sp {@sp} with {@params}", sp, param);
                return await connection.QueryFirstOrDefaultAsync<T>(sql: sp, param: dParam, commandType: CommandType.StoredProcedure);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while executing sp {@sql} with {@param}", sp, param);
            throw;
        }
    }

    /// <summary>
    /// Executes sp with params passed and returns only meta result.
    /// </summary>
    /// <typeparam name="TMetaResult"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="sp"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<TMetaResult> ExecSPForMetaResultAsync<TMetaResult>(string sp, object param)
    {
        try
        {
            TMetaResult first;
            using (var conn = await OpenConnectionAsync())
            {
                DynamicParameters dParam = new DynamicParameters();
                if (param is DynamicParameters)
                {
                    dParam = (DynamicParameters)param;
                }
                else
                {
                    dParam = new DynamicParameters(param);
                }

                dParam.Add("RETURN_VALUE", 0, DbType.Int32, ParameterDirection.ReturnValue);
                logger.LogInformation("Executing sp {@sp} with {@params}", sp, param);
                using (var resultSet = await conn.QueryMultipleAsync(sql: sp, param: dParam, commandType: CommandType.StoredProcedure))
                {
                    first = resultSet.ReadFirstOrDefault<TMetaResult>();
                    //second = resultSet.IsConsumed ? default(TResult) : resultSet.ReadFirstOrDefault<TResult>();
                }
            }
            return first;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while executing sp {@sql} with {@param}", sp, param);
            throw;
        }
    }

    /// <summary>
    /// Executes sp with params passed and returns single record with meta result.
    /// </summary>
    /// <typeparam name="TMetaResult"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="sp"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<TResult> ExecSPForItemResultAsync<TResult>(string sp, object param)
    {
        try
        {
            //TMetaResult first;
            TResult second;
            using (var conn = await OpenConnectionAsync())
            {
                DynamicParameters dParam = new DynamicParameters();
                if (param is DynamicParameters)
                {
                    dParam = (DynamicParameters)param;
                }
                else
                {
                    dParam = new DynamicParameters(param);
                }

                dParam.Add("RETURN_VALUE", 0, DbType.Int32, ParameterDirection.ReturnValue);
                logger.LogInformation("Executing sp {@sp} with {@params}", sp, param);
                using (var resultSet = await conn.QueryMultipleAsync(sql: sp, param: dParam, commandType: CommandType.StoredProcedure))
                {
                    //first = resultSet.ReadFirstOrDefault<TMetaResult>();
                    second = resultSet.IsConsumed ? default(TResult) : resultSet.ReadFirstOrDefault<TResult>();
                }
            }
            return (second);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while executing sp {@sql} with {@param}", sp, param);
            throw;
        }
    }

    public async Task<int> ExecSPForDynamicItemResultAsync<TResult>(string sp, object param)
    {
        try
        {
            //TMetaResult first;
            var result = 0;
            using (var conn = await OpenConnectionAsync())
            {
                DynamicParameters dParam = new DynamicParameters();
                if (param is DynamicParameters)
                {
                    dParam = (DynamicParameters)param;
                }
                else
                {
                    Type type = param.GetType();
                    PropertyInfo[] props = type.GetProperties();
                    foreach (var prop in props)
                    {
                        if (param.IsOutputDirection(prop))
                        {
                            dParam.Add(prop.Name, prop.GetValue(param, null), direction: ParameterDirection.Output);
                        }
                        else
                        {
                            dParam.Add(prop.Name, prop.GetValue(param, null));
                        }
                    }
                }
                dParam.Add("RETURN_VALUE", 0, DbType.Int32, ParameterDirection.ReturnValue);
                logger.LogInformation("Executing sp {@sp} with {@params}", sp, param);
                await conn.ExecuteAsync(sql: sp, param: dParam, commandType: CommandType.StoredProcedure);
                result = dParam.Get<int>("RETURN_VALUE");
                Type typeOutput = param.GetType();
                PropertyInfo[] propOutputs = typeOutput.GetProperties();
                foreach (var prop in propOutputs)
                {
                    if (param.IsOutputDirection(prop))
                    {
                        prop.SetValue(param, dParam.Get<object>(prop.Name));
                    }
                }

            }
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while executing sp {@sql} with {@param}", sp, param);
            throw;
        }
    }

    /// <summary>
    /// Executes sp with params passed and returns list of records with meta result.
    /// </summary>
    /// <typeparam name="TMetaResult"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="sp"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<(TMetaResult, IEnumerable<TResult>)> ExecSPForListResultAsync<TMetaResult, TResult>(string sp, object param)
    {
        try
        {
            TMetaResult first;
            IEnumerable<TResult> second;
            using (var conn = await OpenConnectionAsync())
            {
                var dParam = new DynamicParameters(param);
                dParam.Add("RETURN_VALUE", 0, DbType.Int32, ParameterDirection.ReturnValue);
                logger.LogInformation("Executing sp {@sp} with {@params}", sp, param);
                using (var resultSet = await conn.QueryMultipleAsync(sql: sp, param: param, commandType: CommandType.StoredProcedure))
                {
                    first = resultSet.Read<TMetaResult>().FirstOrDefault();
                    second = resultSet.IsConsumed ? default(IEnumerable<TResult>) : resultSet.Read<TResult>();
                }
            }
            return (first, second);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while executing sp {@sql} with {@param}", sp, param);
            throw;
        }
    }
    public async Task<(TMetaResult, PagingResult<TResult>)> ExecSPReturnListWithAsync<TMetaResult, TResult>(string sp, object param)
    {
        try
        {
            TMetaResult first;
            PagingResult<TResult> pagingResult = new PagingResult<TResult>();
            using (var conn = await OpenConnectionAsync())
            {
                var dParam = new DynamicParameters(param);
                dParam.Add("RETURN_VALUE", 0, DbType.Int32, ParameterDirection.ReturnValue);
                logger.LogInformation("Executing sp {@sp} with {@params}", sp, param);
                using (var resultSet = await conn.QueryMultipleAsync(sql: sp, param: param, commandType: CommandType.StoredProcedure))
                {
                    first = resultSet.Read<TMetaResult>().FirstOrDefault();
                    pagingResult.RecordList = resultSet.IsConsumed ? default(List<TResult>) : resultSet.Read<TResult>().ToList();
                    pagingResult.RecordCount = resultSet.IsConsumed ? 0 : resultSet.Read<int>().FirstOrDefault();
                }
            }
            return (first, pagingResult);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while executing sp {@sql} with {@param}", sp, param);
            throw;
        }
    }

    // <summary>
    // Executes sp with params passed and returns list of records with meta result.
    // </summary>
    // <typeparam name = "TMetaResult" ></ typeparam >
    // < typeparam name="TResult"></typeparam>
    // <param name = "sp" ></ param >
    // < param name="param"></param>
    // <returns></returns>
    public async Task<(TMetaResult, List<TResult>)> ExecSPForListResultAsync2<TMetaResult, TResult>(string sp, object param)
    {
        try
        {
            TMetaResult first;
            List<TResult> second;
            using (var conn = await OpenConnectionAsync())
            {
                var dParam = new DynamicParameters(param);
                dParam.Add("RETURN_VALUE", 0, DbType.Int32, ParameterDirection.ReturnValue);
                logger.LogInformation("Executing sp {@sp} with {@params}", sp, param);
                using (var resultSet = await conn.QueryMultipleAsync(sql: sp, param: param, commandType: CommandType.StoredProcedure))
                {
                    first = resultSet.Read<TMetaResult>().FirstOrDefault();
                    second = resultSet.IsConsumed ? default(List<TResult>) : resultSet.Read<TResult>().ToList();
                }
            }
            return (first, second);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while executing sp {@sql} with {@param}", sp, param);
            throw;
        }
    }
    public async Task<(TMetaResult, PagingResult<TResult>)> ExecSPForPageListResultAsync<TMetaResult, TResult>(string sp, object param)
    {
        try
        {
            TMetaResult first;
            PagingResult<TResult> pagingResult = new PagingResult<TResult>();
            using (var conn = await OpenConnectionAsync())
            {
                var dParam = new DynamicParameters(param);
                dParam.Add("RETURN_VALUE", 0, DbType.Int32, ParameterDirection.ReturnValue);
                logger.LogInformation("Executing sp {@sp} with {@params}", sp, param);
                using (var resultSet = await conn.QueryMultipleAsync(sql: sp, param: param, commandType: CommandType.StoredProcedure))
                {
                    first = resultSet.Read<TMetaResult>().FirstOrDefault();
                    pagingResult.RecordList = resultSet.IsConsumed ? default(IEnumerable<TResult>) : resultSet.Read<TResult>();
                    pagingResult.RecordCount = resultSet.IsConsumed ? 0 : resultSet.Read<int>().FirstOrDefault();
                }
            }
            return (first, pagingResult);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while executing sp {@sql} with {@param}", sp, param);
            throw;
        }
    }

    /// <summary>
    /// Opens and returns the sql connection to db using the connection string in app settings file
    /// </summary>
    /// <returns></returns>
    protected SqlConnection OpenConnection()
    {
        try
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while opening a connection to database. Connection String: " + ConnectionString);
            throw;
        }
    }

    /// <summary>
    /// Opens and returns the sql connection asynchronously to db using the connection string in app settings file
    /// </summary>
    /// <returns></returns>
    protected async Task<SqlConnection> OpenConnectionAsync()
    {
        try
        {
            var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            return connection;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while opening a connection to database. Connection String: " + ConnectionString);
            throw;
        }
    }

    /// <summary>
    /// Executes query with params passed and returns list of records with meta result.
    /// </summary>
    /// <typeparam name="TMetaResult"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="sp"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<(TMetaResult, List<TResult>)> ExecQueryForListResultAsync<TMetaResult, TResult>(string query, CommandType commandType)
    {
        try
        {
            TMetaResult first;
            List<TResult> second;
            using (var conn = await OpenConnectionAsync())
            {

                logger.LogInformation("Executing query {@query}", query);


                using (var resultSet = await conn.QueryMultipleAsync(sql: query, commandType: commandType))
                {
                    first = resultSet.Read<TMetaResult>().FirstOrDefault();
                    second = resultSet.IsConsumed ? default(List<TResult>) : resultSet.Read<TResult>().ToList();
                }
            }
            return (first, second);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while executing sp {@query}.", query);
            throw;
        }
    }
    public async Task<List<TResult>> ExecSPForListResultAsync<TResult>(string sp, object param, int commandTimeout = 30)
    {
        try
        {
            List<TResult> second;
            using (var conn = await OpenConnectionAsync())
            {
                var dParam = new DynamicParameters(param);
                dParam.Add("RETURN_VALUE", 0, DbType.Int32, ParameterDirection.ReturnValue);
                logger.LogInformation("Executing sp {@sp} with {@params}", sp, param);
                using (var resultSet = await conn.QueryMultipleAsync(sql: sp, param: param, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure))
                {
                    second = resultSet.IsConsumed ? default(List<TResult>) : resultSet.Read<TResult>().ToList();
                }
            }
            return (second);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while executing sp {@sql} with {@param}", sp, param);
            throw;
        }
    }

    public async Task<List<TResult>> ExecSPForListResultWithLongQueryCommandTimeOutAsync<TResult>(string sp, object param)
    {
        return await ExecSPForListResultAsync<TResult>(sp, param, LongQueryCommandTimeOut);
    }

}
