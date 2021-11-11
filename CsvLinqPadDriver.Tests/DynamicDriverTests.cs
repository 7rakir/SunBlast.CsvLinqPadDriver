using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvLinqPadDriver.Tests.Utils;
using LINQPad.Extensibility.DataContext;
using Moq;
using NUnit.Framework;

namespace CsvLinqPadDriver.Tests
{
    public class DynamicDriverTests
    {
        [Test]
        public void BuildAssembly_GeneratedCodeIsQueryable()
        {
            // Arrange
            string path = TestData.Path<DynamicDriverTests>();

            var connection = new Mock<IConnectionInfo>();
            connection.Setup(c => c.DisplayName).Returns(path);

            var assemblyName = new AssemblyName("AssemblyName")
            {
                CodeBase = Path.Combine(path, "assembly-file")
            };

            string nameSpace = "MyNameSpace";
            string typeName = "UserContextClass";

            // Act
            new DynamicDriver().GetSchemaAndBuildAssembly(connection.Object, assemblyName, ref nameSpace, ref typeName);

            // Assert
            dynamic contextClass = Assembly
                .LoadFrom(assemblyName.CodeBase)
                .CreateInstance($"{nameSpace}.{typeName}")!;

            var data = (IEnumerable<dynamic>)contextClass.Data;
            var secondRow = data.Skip(1).First();

            var expected = new Dictionary<string, object?>()
            {
                { "Header1", "2" },
                { "", "Empty2" },
                { "Header2", "Input2" },
            };

            CollectionAssert.AreEquivalent(expected, GetProperties(secondRow));
        }

        private static Dictionary<string, object?> GetProperties<T>(T input) where T : class
        {
            return input.GetType().GetFields().ToDictionary(f => f.Name, f => f.GetValue(input));
        }
    }
}