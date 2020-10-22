using Microsoft.Azure.Cosmos.Table;
using System;

namespace solutions_architect_nosql_m1
{
  public class SensorMeasurementEntity : TableEntity
  {
    public string SensorId { get; set; }
    public string MeasurementId { get; set; }
    public DateTime MeasuredAtUtc { get; set; }
    public double Fahrenheit { get; set; }
    public short AltitudeMeters { get; set; }

    public override string ToString()
    {
      return $"Sensor {SensorId}, measured at {MeasuredAtUtc}, {AltitudeMeters} meters, {Fahrenheit} F";
    }
  }
}
