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

using Cassandra;
using Cassandra.Mapping;
using System;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace solutions_architect_nosql_m2.CassandraApi
{
  public static class CassandraApiSample
  {
    // TODO: fill in your COSMOS DB USING CASSANDRA API connection settings
    private const string connectionContactPoint = "TODO.cassandra.cosmos.azure.com";
    private const string connectionUsername = "TODO";
    private const string connectionPassword = "TODO";
    private const int connectionPort = 10350;

    internal const string keyspaceName = "keyspacefromcode";
    internal const string tableName = "tablefromcode";

    public static async Task RunSample()
    {
      Console.Clear();

      var options = new Cassandra.SSLOptions(
        sslProtocol: SslProtocols.Tls12,
        checkCertificateRevocation: true,
        remoteCertValidationCallback: (sender, cerfificate, chain, policyErrors) => policyErrors == SslPolicyErrors.None);
      
      options.SetHostNameResolver(ip => connectionContactPoint);

      var cluster = Cluster
        .Builder()
        .WithCredentials(connectionUsername, connectionPassword)
        .WithPort(connectionPort)
        .AddContactPoint(connectionContactPoint)
        .WithSSL(options)
        .Build();

      using (var session = cluster.Connect())
      {
        CreateTable(session);
        Console.ReadLine();

        await AddTableData(session);
        Console.ReadLine();

        await GetTableData(session);
        Console.ReadLine();

        DeleteKeyspaceIfExists(session);
        Console.ReadLine();
      }
    }

    private static void CreateTable(ISession session)
    {
      session.Execute($"DROP KEYSPACE IF EXISTS {keyspaceName};");
      session.Execute($"CREATE KEYSPACE {keyspaceName} WITH replication = {{'class':'SimpleStrategy'}};");

      Console.WriteLine($"Keyspace {keyspaceName} was created");

      session.Execute($"CREATE TABLE IF NOT EXISTS {keyspaceName}.{tableName} (measurementId text PRIMARY KEY, sensorId text, measuredatutc timestamp, fahrenheit double, altitudemeters int);");

      Console.WriteLine($"Table {tableName} was created");

      session.Execute($"CREATE INDEX ON {keyspaceName}.{tableName} (sensorid);");
      session.Execute($"CREATE INDEX ON {keyspaceName}.{tableName} (fahrenheit);");
    }

    private static void DeleteKeyspaceIfExists(ISession session)
    {
      Console.WriteLine($"Deleting keyspace {keyspaceName}");

      session.Execute($"DROP KEYSPACE IF EXISTS {keyspaceName};");
    }

    private static async Task AddTableData(ISession session)
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

      var mapper = new Mapper(session);

      foreach (var item in items)
      {
        Console.WriteLine($"Inserting measurement from {item}");

        await mapper.InsertAsync(item);
      }
    }

    private static async Task GetTableData(ISession session)
    {
      var mapper = new Mapper(session);

      (await mapper
         .FetchAsync<SensorMeasurementItem>($"select * from {keyspaceName}.{tableName} where sensorid = ? and fahrenheit < ?", "sensor-03", 54.0))
         .ToList()
         .ForEach(item =>
         {
           Console.WriteLine($"Found {item}");
         });
    }
  }
}
