using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeApi.Extensions
{
    public static class DbContextExtensions
    {
       
        /// Execute a stored procedure and return results
    
        public static async Task<List<T>> LoadStoredProc<T>(
            this DbContext context,
            string procedureName,
            params SqlParameter[] parameters) where T : class
        {
            var paramNames = parameters.Length > 0 
                ? string.Join(", ", parameters.Select(p => p.ParameterName))
                : string.Empty;
            
            var sql = parameters.Length > 0
                ? $"EXEC {procedureName} {paramNames}"
                : $"EXEC {procedureName}";

            return await context.Database
                .SqlQueryRaw<T>(sql, parameters)
                .ToListAsync();
        }

        /// Execute a stored procedure and return single result
      
        public static async Task<T?> LoadStoredProcSingle<T>(
            this DbContext context,
            string procedureName,
            params SqlParameter[] parameters) where T : class
        {
            var results = await LoadStoredProc<T>(context, procedureName, parameters);
            return results.FirstOrDefault();
        }

        
        /// Execute a stored procedure with output parameter

        public static async Task<(List<T> Results, int OutputId)> LoadStoredProcWithOutput<T>(
            this DbContext context,
            string procedureName,
            SqlParameter outputParameter,
            params SqlParameter[] inputParameters) where T : class
        {
            var allParams = inputParameters.Append(outputParameter).ToArray();
            var results = await LoadStoredProc<T>(context, procedureName, allParams);
            var outputId = outputParameter.Value != DBNull.Value ? (int)outputParameter.Value : 0;
            return (results, outputId);
        }
    }
}
