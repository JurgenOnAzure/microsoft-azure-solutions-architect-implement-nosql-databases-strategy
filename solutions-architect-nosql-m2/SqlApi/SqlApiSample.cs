/*
  This demo application accompanies Pluralsight course 'Microsoft Azure Solutions Architect: Implement a NoSQL Databases Strategy', by Jurgen Kevelaers. 
  See https://app.pluralsight.com/profile/author/jurgen-kevelaers.

  MIT License

  Copyright (c) 2020 Jurgen Kevelaers

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.
*/

using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace solutions_architect_nosql_m2.SqlApi
{
  public static class SqlApiSample
  {
    // TODO: fill in your COSMOS DB USING SQL API string
    private const string connectionString = "AccountEndpoint=https://...";

    private const string databaseName = "DatabaseFromCode";
    private const string containerName = "ContainerFromCode";

    public static async Task RunSample()
    {
      Console.Clear();

      using (var client = new CosmosClient(connectionString))
      {
        var container = await GetContainer(client);
        Console.ReadLine();

        await AddContainerData(container);
        Console.ReadLine();

        await GetContainerData(container);
        Console.ReadLine();

        await DeleteDatabaseIfExists(client);
        Console.ReadLine();
      }
    }

    private static async Task<Container> GetContainer(CosmosClient client)
    {
      var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);

      var databaseWasCreated = databaseResponse.StatusCode == System.Net.HttpStatusCode.Created;

      if (databaseWasCreated)
      {
        Console.WriteLine($"Database {databaseName} was created");
      }
      else
      {
        Console.WriteLine($"Database {databaseName} was not created, because is already exists");
      }

      var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(
        id: containerName, 
        partitionKeyPath: $"/{nameof(SensorMeasurementItem.SensorId)}");

      return containerResponse.Container;
    }

    private static async Task DeleteDatabaseIfExists(CosmosClient client)
    {
      var database = client.GetDatabase(databaseName);
      var databaseWasDeleted = false;

      try
      {
        // will throw exception if database doesn't exist
        var deleteResponse = await database.DeleteAsync();

        databaseWasDeleted = true;
      }
      catch (CosmosException) { }

      if (databaseWasDeleted)
      {
        Console.WriteLine($"Database {databaseName} was deleted");
      }
      else
      {
        Console.WriteLine($"Database {databaseName} was not deleted, because it doesn't exist");
      }
    }

    private static async Task AddContainerData(Container container)
    {
      var utcNow = DateTime.UtcNow;

      var items = new[]
      {
        new SensorMeasurementItem
        {
          SensorId = "sensor-01",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-10),
          AltitudeMeters = 12,
          Fahrenheit = 89.6
        },
        new SensorMeasurementItem
        {
          SensorId = "sensor-01",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-9),
          AltitudeMeters = 13,
          Fahrenheit = 88.2
        },
        new SensorMeasurementItem
        {
          SensorId = "sensor-02",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-12),
          AltitudeMeters = 123,
          Fahrenheit = 78.3
        },
        new SensorMeasurementItem
        {
          SensorId = "sensor-03",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-6),
          AltitudeMeters = 35,
          Fahrenheit = 53.3
        },
        new SensorMeasurementItem
        {
          SensorId = "sensor-03",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-4),
          AltitudeMeters = 32,
          Fahrenheit = 53.8
        },
        new SensorMeasurementItem
        {
          SensorId = "sensor-03",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-3),
          AltitudeMeters = 33,
          Fahrenheit = 54.1
        }
      };

      foreach (var item in items)
      {
        Console.WriteLine($"Inserting measurement from {item}");

        var response = await container.UpsertItemAsync(item);

        Console.WriteLine($"  - status: {response.StatusCode}");
      }
    }

    private static async Task GetContainerData(Container container)
    {
      using (var iterator = container.GetItemQueryIterator<SensorMeasurementItem>(
        queryText: $"SELECT * FROM c WHERE c.Fahrenheit < 54",
        requestOptions: new QueryRequestOptions
        {
          PartitionKey = new PartitionKey("sensor-03")
        }))
      {
        while (iterator.HasMoreResults)
        {
          var response = await iterator.ReadNextAsync();

          response
            .ToList()
            .ForEach(item =>
            {
              Console.WriteLine($"Found {item}");
            });
        }
      }
    }
  }
}
