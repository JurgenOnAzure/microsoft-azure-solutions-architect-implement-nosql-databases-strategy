﻿/*
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

using System;

namespace solutions_architect_nosql_m2.CassandraApi
{
  [Cassandra.Mapping.Attributes.Table(
    CassandraApiSample.tableName, 
    Keyspace = CassandraApiSample.keyspaceName)]
  public class SensorMeasurementItem
  {
    [Cassandra.Mapping.Attributes.Column("sensorid")]
    public string SensorId { get; set; }

    [Cassandra.Mapping.Attributes.Column("measurementid")]
    public string MeasurementId { get; set; }
    
    [Cassandra.Mapping.Attributes.Column("measuredatutc")]
    public DateTime MeasuredAtUtc { get; set; }
    
    [Cassandra.Mapping.Attributes.Column("fahrenheit")]
    public double Fahrenheit { get; set; }
    
    [Cassandra.Mapping.Attributes.Column("altitudemeters")]
    public int AltitudeMeters { get; set; }

    public override string ToString()
    {
      return $"Sensor {SensorId}, measured at {MeasuredAtUtc}, {AltitudeMeters} meters, {Fahrenheit} F";
    }
  }
}
