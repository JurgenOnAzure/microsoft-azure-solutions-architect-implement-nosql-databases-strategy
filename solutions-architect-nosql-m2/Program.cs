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

using System;
using System.Threading.Tasks;

namespace solutions_architect_nosql_m2
{
  class Program
  {
    static async Task Main()
    {
      Console.Clear();
      Console.WriteLine("Press ENTER to run the SQL API sample");
      Console.ReadLine();
      await SqlApi.SqlApiSample.RunSample();

      Console.Clear();
      Console.WriteLine("Press ENTER to run the TABLE API sample");
      Console.ReadLine();
      await TableApi.TableApiSample.RunSample();

      Console.Clear();
      Console.WriteLine("Press ENTER to run the MONGO API sample");
      Console.ReadLine();
      await MongoApi.MongoApiSample.RunSample();

      Console.Clear();
      Console.WriteLine("Press ENTER to run the CASSANDRA API sample");
      Console.ReadLine();
      await CassandraApi.CassandraApiSample.RunSample();

      Console.Clear();
      Console.WriteLine("Press ENTER to run the GREMLIN API sample");
      Console.ReadLine();
      await GremlinApi.GremlinApiSample.RunSample();
    }
  }
}
