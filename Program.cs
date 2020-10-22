using Microsoft.Azure.Cosmos.Table;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace solutions_architect_nosql_m1
{
  class Program
  {
    // TODO: fill in your storage account connection string
    private const string connectionString = "DefaultEndpointsProtocol=https;...";
    
    private const string tableName = "TableFromCode";

    static async Task Main()
    {
      Console.Clear();

      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
      CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
      CloudTable table = tableClient.GetTableReference(tableName);

      await CreateTableIfNotExists(table);
      Console.ReadLine();

      await AddTableData(table);
      Console.ReadLine();

      GetTableData(table);
      Console.ReadLine();

      await DeleteTableIfExists(table);
      Console.ReadLine();
    }

    private static async Task CreateTableIfNotExists(CloudTable table)
    {
      var tableWasCreated = await table.CreateIfNotExistsAsync();

      if (tableWasCreated)
      {
        Console.WriteLine($"Table {tableName} was created");
      }
      else
      {
        Console.WriteLine($"Table {tableName} was not created, because is already exists");
      }
    }

    private static async Task DeleteTableIfExists(CloudTable table)
    {
      var tableWasDeleted = await table.DeleteIfExistsAsync();

      if (tableWasDeleted)
      {
        Console.WriteLine($"Table {tableName} was deleted");
      }
      else
      {
        Console.WriteLine($"Table {tableName} was not deleted, because it doesn't exist");
      }
    }

    private static async Task AddTableData(CloudTable table)
    {
      var utcNow = DateTime.UtcNow;

      var entities = new[]
      {
        new SensorMeasurementEntity
        {
          SensorId = "sensor-01",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-10),
          AltitudeMeters = 12,
          Fahrenheit = 89.6
        },
        new SensorMeasurementEntity
        {
          SensorId = "sensor-01",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-9),
          AltitudeMeters = 13,
          Fahrenheit = 88.2
        },
        new SensorMeasurementEntity
        {
          SensorId = "sensor-02",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-12),
          AltitudeMeters = 123,
          Fahrenheit = 78.3
        },
        new SensorMeasurementEntity
        {
          SensorId = "sensor-03",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-6),
          AltitudeMeters = 35,
          Fahrenheit = 53.3
        },
        new SensorMeasurementEntity
        {
          SensorId = "sensor-03",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-4),
          AltitudeMeters = 32,
          Fahrenheit = 53.8
        },
        new SensorMeasurementEntity
        {
          SensorId = "sensor-03",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-3),
          AltitudeMeters = 33,
          Fahrenheit = 54.1
        }
      };

      foreach (var entity in entities)
      {
        entity.PartitionKey = entity.SensorId;
        entity.RowKey = entity.MeasurementId;

        Console.WriteLine($"Inserting measurement from {entity}");
        
        var operation = TableOperation.Insert(entity);
        var result = await table.ExecuteAsync(operation);

        Console.WriteLine($"  - status: {result.HttpStatusCode}, timestamp: {entity.Timestamp}");
      }
    }

    private static void GetTableData(CloudTable table)
    {
      var query = table.CreateQuery<SensorMeasurementEntity>();

      query
        .Where(entity => entity.PartitionKey == "sensor-03" && entity.Fahrenheit < 54)
        .ToList()
        .ForEach(entity => 
        {
          Console.WriteLine($"Found {entity}");
        });
    }
  }
}
