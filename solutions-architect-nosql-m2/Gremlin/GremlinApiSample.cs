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

using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace solutions_architect_nosql_m2.GremlinApi
{
  public static class GremlinApiSample
  {
    // TODO: fill in your COSMOS DB USING CASSANDRA API connection settings
    private const string connectionHostname = "TODO.gremlin.cosmos.azure.com";
    private const string connectionPrimaryKey = "TODO";
    private const string connectionDatabaseName = "DatabaseFromPortal";
    private const string connectionContainerName = "GraphFromPortal";
    
    private const string labelName = "measurement";

    public static async Task RunSample()
    {
      Console.Clear();

      var server = new GremlinServer(
        hostname: connectionHostname, 
        port: 443, 
        enableSsl: true, 
        username: $"/dbs/{connectionDatabaseName}/colls/{connectionContainerName}", 
        password: connectionPrimaryKey);

      using (var client = new GremlinClient(
        server, 
        new GraphSON2Reader(),
        new GraphSON2Writer(),
        GremlinClient.GraphSON2MimeType))
      {
        await client.SubmitAsync("g.V().drop()");

        var utcNow = DateTime.UtcNow;

        await client.SubmitAsync($"g.addV('{labelName}').property('SensorId', 'sensor-01').property('MeasurementId', '{Guid.NewGuid()}').property('MeasuredAtUtc', '{utcNow.AddSeconds(-10)}').property('AltitudeMeters', 12).property('Fahrenheit', 89.6)");
        await client.SubmitAsync($"g.addV('{labelName}').property('SensorId', 'sensor-01').property('MeasurementId', '{Guid.NewGuid()}').property('MeasuredAtUtc', '{utcNow.AddSeconds(-9)}').property('AltitudeMeters', 13).property('Fahrenheit', 88.2)");
        await client.SubmitAsync($"g.addV('{labelName}').property('SensorId', 'sensor-02').property('MeasurementId', '{Guid.NewGuid()}').property('MeasuredAtUtc', '{utcNow.AddSeconds(-12)}').property('AltitudeMeters', 123).property('Fahrenheit', 78.3)");
        await client.SubmitAsync($"g.addV('{labelName}').property('SensorId', 'sensor-03').property('MeasurementId', '{Guid.NewGuid()}').property('MeasuredAtUtc', '{utcNow.AddSeconds(-6)}').property('AltitudeMeters', 35).property('Fahrenheit', 53.3)");
        await client.SubmitAsync($"g.addV('{labelName}').property('SensorId', 'sensor-03').property('MeasurementId', '{Guid.NewGuid()}').property('MeasuredAtUtc', '{utcNow.AddSeconds(-4)}').property('AltitudeMeters', 32).property('Fahrenheit', 53.8)");
        await client.SubmitAsync($"g.addV('{labelName}').property('SensorId', 'sensor-03').property('MeasurementId', '{Guid.NewGuid()}').property('MeasuredAtUtc', '{utcNow.AddSeconds(-3)}').property('AltitudeMeters', 33).property('Fahrenheit', 54.1)");

        Console.WriteLine("Items were inserted");
        Console.ReadLine();

        (await client.SubmitAsync<dynamic>($"g.V().hasLabel('{labelName}').has('SensorId', 'sensor-03').has('Fahrenheit', lt(54.0))"))
          .ToList()
          .ForEach(item =>
          {
             Console.WriteLine($"Found {Newtonsoft.Json.JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented)}");
          });

        Console.ReadLine();

        await client.SubmitAsync("g.V().drop()");
        Console.WriteLine("Items were deleted");
        Console.ReadLine();
      }
    }
  }
}
