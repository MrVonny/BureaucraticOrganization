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
    public class Tests
    {

        private static List<Department> plainDeps1 = new List<Department>()
        {
                new Department("CLion", new RuleEvent("A","B","PyCharm")),
                new Department("PyCharm", new RuleEvent("C","A","PhpStorm")),
                new Department("PhpStorm","D", new RuleEvent("G","A","DataGrid"), new RuleEvent("A","B","Rider")),
                new Department("Rider", new RuleEvent("D","B","PhpStorm")),
                new Department("DataGrid", new RuleEvent("K","B","PhpStorm"))
        };
        private static List<Department> plainDeps2 = new List<Department>()
        {
                new Department("CLion", new RuleEvent("A","B","PyCharm")),
                new Department("PyCharm", new RuleEvent("C","A","PhpStorm")),
                new Department("PhpStorm","D", new RuleEvent("G","A","DataGrid"), new RuleEvent("A","B","Rider")),
                new Department("Rider", new RuleEvent("D","B","PhpStorm")),
                new Department("DataGrid", new RuleEvent("K","B","PhpStorm"))
        };
        private static List<Department> loopingDeps1 = new List<Department>()
        {
                new Department("CLion", new RuleEvent("A","B","PyCharm")),
                new Department("PyCharm", new RuleEvent("C","A","PhpStorm")),
                new Department("PhpStorm","D", new RuleEvent("G","A","DataGrid"), new RuleEvent("A","B","Rider")),
                new Department("Rider", new RuleEvent("D","B","PhpStorm")),
                new Department("DataGrid", new RuleEvent("K","B","PhpStorm"))
        };
        private static List<Department> loopingDeps2 = new List<Department>()
        {
                new Department("CLion", new RuleEvent("A","B","PyCharm")),
                new Department("PyCharm", new RuleEvent("C","A","PhpStorm")),
                new Department("PhpStorm","D", new RuleEvent("G","A","DataGrid"), new RuleEvent("A","B","Rider")),
                new Department("Rider", new RuleEvent("D","B","PhpStorm")),
                new Department("DataGrid", new RuleEvent("K","B","PhpStorm"))
        };

        private static OrganizationConfiguration plainConf1 = new OrganizationConfiguration("CLion", "DataGrid", plainDeps1);
        private static OrganizationConfiguration plainConf2 = new OrganizationConfiguration("", "", plainDeps2);
        private static OrganizationConfiguration loopingConf1 = new OrganizationConfiguration("", "", loopingDeps1);
        private static OrganizationConfiguration loopingConf2 = new OrganizationConfiguration("", "", loopingDeps2);


        [Theory]
        [InlineData("FromCodeTest_Result.json")]
        public async void FromCode_ShouldReturnCorrectResult(string filename)
        {
            
            Organization organization = new Organization(plainConf1);

            var result = await organization.GetResultAsync("PhpStorm");
            var str = result.ToJson();
            Assert.Null(result.Exception);
            Assert.True(result.Successful);
            using (StreamReader file = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\TestFiles\", filename)))
            {
                Assert.Equal(JObject.Parse(result.ToJson()),JObject.Parse(await file.ReadToEndAsync()));
            }
        }

        //ToDo: дописать
        [Fact]
        public async void FromFileAndFromCode_ShouldReturnSameResult()
        {
            throw new NotImplementedException();
            using (StreamReader file = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\TestFiles\PlainTest1.json")))
            {
                var organization1 = new Organization(plainConf1);
                var organization2 = new Organization(await file.ReadLineAsync());
                Assert.Equal(await organization1.GetResultAsync("PhpStorm"), await organization2.GetResultAsync("PhpStorm"));
            }

            using (StreamReader file = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\TestFiles\PlainTest2.json")))
            {
                var organization1 = new Organization(plainConf1);
                var organization2 = new Organization(await file.ReadLineAsync());
                Assert.Equal(await organization1.GetResultAsync(""), await organization2.GetResultAsync(""));
            }

            using (StreamReader file = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\TestFiles\Looping1.json")))
            {
                var organization1 = new Organization(plainConf1);
                var organization2 = new Organization(await file.ReadLineAsync());
                Assert.Equal(await organization1.GetResultAsync(""), await organization2.GetResultAsync(""));
            }

            using (StreamReader file = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\TestFiles\Looping2.json")))
            {
                var organization1 = new Organization(plainConf1);
                var organization2 = new Organization(await file.ReadLineAsync());
                Assert.Equal(await organization1.GetResultAsync(""), await organization2.GetResultAsync(""));
            }

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

        [Theory]
        [InlineData("WrongFormat1.json")]
        [InlineData("WrongFormat2.json")]
        public void FromFile_WrongFormat_ShouldThrowExecption(string filename)
        {
            using (StreamReader file = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\TestFiles\", filename)))
            {
                Assert.ThrowsAny<Exception>(() => new Organization(file.ReadToEnd()));
            }

        }

        [Theory]
        [InlineData("PlainTest1.json")]
        [InlineData("PlainTest2.json")]
        public void ConcurrentRequests_ShouldWork(string filename)
        {
            Organization organization;
            using (StreamReader file = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\TestFiles\", filename)))
            {
                organization = new Organization(file.ReadToEnd());
            }

            List<string> requests = organization.Configuration.Departments.Select(x => x.Id).ToList();

            //Создаём более 100 000 запросов
            while(requests.Count<1E5)
                requests.AddRange(requests);

            Parallel.ForEach(requests, (department) =>
            {
                var result = organization.GetResult(department);
                Assert.True(result.Successful);
            });

        }
    }
}
