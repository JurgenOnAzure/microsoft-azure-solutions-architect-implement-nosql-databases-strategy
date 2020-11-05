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

using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace solutions_architect_nosql_m2.MongoApi
{
  public static class MongoApiSample
  {
    // TODO: fill in your COSMOS DB USING MONGO API string
    private const string connectionString = "mongodb://...";

    private const string databaseName = "DatabaseFromCode";
    private const string collectionName = "CollectionFromCode";

    public static async Task RunSample()
    {
      Console.Clear();

      var client = new MongoClient(connectionString);

      var collection = await GetCollection(client);
      Console.ReadLine();

      await AddCollectionData(collection);
      Console.ReadLine();

      await GetCollectionData(collection);
      Console.ReadLine();

      await DeleteDatabaseIfExists(client);
      Console.ReadLine();
    }

    private static async Task<IMongoCollection<SensorMeasurementBsonDocument>> GetCollection(MongoClient client)
    {
      var existingDatabaseNames = (await client.ListDatabaseNamesAsync()).ToList();
      var databaseExists = existingDatabaseNames.Contains(databaseName);

      if (databaseExists)
      {
        Console.WriteLine($"Will get existing database {databaseName}");
      }
      else
      {
        Console.WriteLine($"Will create database {databaseName}");
      }

      var database = client.GetDatabase(databaseName);

      var collection = database.GetCollection<SensorMeasurementBsonDocument>(collectionName);

      return collection;
    }

    private static async Task DeleteDatabaseIfExists(MongoClient client)
    {
      var existingDatabaseNames = (await client.ListDatabaseNamesAsync()).ToList();
      var databaseExists = existingDatabaseNames.Contains(databaseName);

      if (databaseExists)
      {
        await client.DropDatabaseAsync(databaseName);

        Console.WriteLine($"Database {databaseName} was deleted");
      }
      else
      {
        Console.WriteLine($"Database {databaseName} was not deleted, because it doesn't exist");
      }
    }

    private static async Task AddCollectionData(IMongoCollection<SensorMeasurementBsonDocument> collection)
    {
      var utcNow = DateTime.UtcNow;

      var documents = new List<SensorMeasurementBsonDocument>
      {
        new SensorMeasurementBsonDocument
        {
          SensorId = "sensor-01",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-10),
          AltitudeMeters = 12,
          Fahrenheit = 89.6
        },
        new SensorMeasurementBsonDocument
        {
          SensorId = "sensor-01",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-9),
          AltitudeMeters = 13,
          Fahrenheit = 88.2
        },
        new SensorMeasurementBsonDocument
        {
          SensorId = "sensor-02",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-12),
          AltitudeMeters = 123,
          Fahrenheit = 78.3
        },
        new SensorMeasurementBsonDocument
        {
          SensorId = "sensor-03",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-6),
          AltitudeMeters = 35,
          Fahrenheit = 53.3
        },
        new SensorMeasurementBsonDocument
        {
          SensorId = "sensor-03",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-4),
          AltitudeMeters = 32,
          Fahrenheit = 53.8
        },
        new SensorMeasurementBsonDocument
        {
          SensorId = "sensor-03",
          MeasurementId = Guid.NewGuid().ToString(),
          MeasuredAtUtc = utcNow.AddSeconds(-3),
          AltitudeMeters = 33,
          Fahrenheit = 54.1
        }
      };

      documents
        .ForEach(document =>
        {
          // id must be hexadecimal
          document.UniqueId = new ObjectId(string.Join("", document.MeasurementId.Select(c => ((int)c).ToString("X2"))));

          Console.WriteLine($"Inserting measurement from {document}");
        });

      await collection.InsertManyAsync(documents);
    }

    private static async Task GetCollectionData(IMongoCollection<SensorMeasurementBsonDocument> collection)
    {
      (await collection.FindAsync(document => document.SensorId == "sensor-03" && document.Fahrenheit < 54))
        .ToList()
        .ForEach(document =>
        {
          Console.WriteLine($"Found {document}");
        });
    }
  }
}
