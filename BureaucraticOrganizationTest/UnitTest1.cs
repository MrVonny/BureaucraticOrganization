using System;
using Xunit;
using BureaucraticOrganization;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BureaucraticOrganizationTest
{
    public class UnitTest1
    {

        private static List<Department> plainTest1 = new List<Department>()
        {
                new Department("CLion", new RuleEvent("A","B","PyCharm")),
                new Department("PyCharm", new RuleEvent("C","A","PhpStorm")),
                new Department("PhpStorm","D", new RuleEvent("G","A","DataGrid"), new RuleEvent("A","B","Rider")),
                new Department("Rider", new RuleEvent("D","B","PhpStorm")),
                new Department("DataGrid", new RuleEvent("K","B","PhpStorm"))
        };
        private static List<Department> plainTest2 = new List<Department>()
        {
                new Department("CLion", new RuleEvent("A","B","PyCharm")),
                new Department("PyCharm", new RuleEvent("C","A","PhpStorm")),
                new Department("PhpStorm","D", new RuleEvent("G","A","DataGrid"), new RuleEvent("A","B","Rider")),
                new Department("Rider", new RuleEvent("D","B","PhpStorm")),
                new Department("DataGrid", new RuleEvent("K","B","PhpStorm"))
        };
        private static List<Department> loopingTest1 = new List<Department>()
        {
                new Department("CLion", new RuleEvent("A","B","PyCharm")),
                new Department("PyCharm", new RuleEvent("C","A","PhpStorm")),
                new Department("PhpStorm","D", new RuleEvent("G","A","DataGrid"), new RuleEvent("A","B","Rider")),
                new Department("Rider", new RuleEvent("D","B","PhpStorm")),
                new Department("DataGrid", new RuleEvent("K","B","PhpStorm"))
        };
        private static List<Department> loopingTest2 = new List<Department>()
        {
                new Department("CLion", new RuleEvent("A","B","PyCharm")),
                new Department("PyCharm", new RuleEvent("C","A","PhpStorm")),
                new Department("PhpStorm","D", new RuleEvent("G","A","DataGrid"), new RuleEvent("A","B","Rider")),
                new Department("Rider", new RuleEvent("D","B","PhpStorm")),
                new Department("DataGrid", new RuleEvent("K","B","PhpStorm"))
        };


        [Theory]
        [InlineData("FromCodeTest_Result.json")]
        public async void FromCode_ShouldReturnCorrectResult(string filename)
        {
            
            var configuration = new OrganizationConfiguration("CLion","DataGrid", plainTest1);

            Organization organization = new Organization(configuration);

            var result = await organization.GetResultAsync("PhpStorm");

            Assert.Null(result.Exception);
            Assert.True(result.Successful);
            using (StreamReader file = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\TestFiles\", filename)))
            {
                Assert.Equal(JObject.Parse(result.ToJson()),JObject.Parse(await file.ReadToEndAsync()));
            }
        }

        [Fact]
        public void FromFileAndFromCode_ShouldReturnSameResult()
        {

        }
        [Fact]
        public void FromFile_ShouldReturnCorrectResult()
        {

        }

        [Theory]
        [InlineData("Looping1.json")]
        //[InlineData("Looping2.json")]
        public async void FromFile_Looping_ShouldReturnResultWithLooping(string filename)
        {
            Organization organization;
            BypassResult result;
            using (StreamReader file = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\TestFiles\", filename)))
            {
                organization = new Organization(file.ReadToEnd());               
            }
            result = await organization.GetResultAsync("CLion");
            var str = result.ToJson();
            Assert.True(result.Successful);
            Assert.True(result.IsLoop);
        }
        [Fact]
        public void FromFile_WrongFormat_ShouldThrowExecption()
        {

        }

        [Theory]
        [InlineData("PlainTest1.json")]
        //[InlineData("PlainTest2.json")]
        public void ConcurrentRequests_ShouldWork(string filename)
        {
            Organization organization;
            using (StreamReader file = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\TestFiles\", filename)))
            {
                organization = new Organization(file.ReadToEnd());
            }

            List<string> requests = organization.Configuration.Departments.Select(x => x.Id).ToList();

            //Создаём более 1 000 000 запросов
            while(requests.Count<1000000)
                requests.AddRange(requests);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.ForEach(requests, (department) =>
            {
                var result = organization.GetResult(department);
                Assert.True(result.Successful);
            });
            stopwatch.Stop();

            Console.WriteLine($"{requests.Count} запросов обработаны за {stopwatch.ElapsedMilliseconds} мс");
        }
    }
}
