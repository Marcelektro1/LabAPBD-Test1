using LabAPBD_Test1.Services.Util;
using Microsoft.Data.SqlClient;

namespace LabAPBD_Test1.Services;

public class SomethingService(IConfiguration configuration) : ISomethingService
{
    public async Task<ServiceResult> DoSomethingAsync()
    {
        await using var connection = new SqlConnection(configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        
        try
        {
            command.CommandText = "";// query
            // command.Parameters.AddWithValue("@IdWarehouse", whatever);

            // do something with it

            command.Parameters.Clear();


            await transaction.CommitAsync();
            return new ServiceResult(true, "something cool");
        } 
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw; // rethrow
        }
        
        return new ServiceResult(true, "something cool");
    }
}