using DbCommands;
using DbCommands.Extensions;
using System.Data.SqlClient;

var sqlConnection = new SqlConnection("Server=.;Database=DbCommands;Trusted_Connection=true;TrustServerCertificate=true");

var values = new List<PerformanceCheckData>()
{
    new PerformanceCheckData(Guid.NewGuid(), 1),
    new PerformanceCheckData(Guid.NewGuid(), 2),
    new PerformanceCheckData(Guid.NewGuid(), 3),
    new PerformanceCheckData(Guid.NewGuid(), 4),
    new PerformanceCheckData(Guid.NewGuid(), 5),
    new PerformanceCheckData(Guid.NewGuid(), 6),
    new PerformanceCheckData(Guid.NewGuid(), 7)
};

sqlConnection.Open();
sqlConnection.InsertInBatches(values);

sqlConnection.Open();
sqlConnection.DeleteInBatches<PerformanceCheckData, Guid>(values.Select(x => x.Id));