using System;
using Xunit;
using BureaucraticOrganization;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BureaucraticOrganizationTest
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("FromCodeTest_Result.json")]
        public async void FromCode_ShouldReturnCorrectResult(string filename)
        {
            List<Department> departments = new List<Department>()
            {
                new Department("CLion", new RuleEvent("A","B","PyCharm")),
                new Department("PyCharm", new RuleEvent("C","A","PhpStorm")),
                new Department("PhpStorm","D", new RuleEvent("G","A","DataGrid"), new RuleEvent("A","B","Rider")),
                new Department("Rider", new RuleEvent("D","B","PhpStorm")),
                new Department("DataGrid", new RuleEvent("K","B","PhpStorm"))
            };
            OrganizationConfiguration configuration = new OrganizationConfiguration("CLion","DataGrid",departments);
            Organization organization = new Organization(configuration);
            var result = await organization.GetResultAsync("PhpStorm");
            var p = result.ToJson();

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
        public async void FromFile_Looping_ShouldReturnResultWithExecption(string filename)
        {
            Organization organization;
            using (StreamReader file = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\TestFiles\", filename)))
            {
                organization = new Organization(file.ReadToEnd());
                BypassResult result = await organization.GetResultAsync("CLion");
                Assert.Equal(typeof(OperationCanceledException), result.Exception.GetType());
            }
        }
        [Fact]
        public void FromFile_WrongFormat_ShouldThrowExecption()
        {

        }
    }
}
