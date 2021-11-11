using System;
using System.Linq;
using CsvLinqPadDriver.Tests.Utils;
using NUnit.Framework;

namespace CsvLinqPadDriver.Tests
{
    public class ModelReaderTests
    {
        [Test]
        public void ModelReader_ShouldProperlyGenerateSchema()
        {
            string path = TestData.Path<ModelReaderTests>();

            var schemaModel = ModelReader.GetSchemaModel(path).ToArray();

            var expectedModel = new FileModel[]
            {
                new(
                    new FileDescription("Normal.csv", 0, DateTime.Now),
                    new DataDescription(new [] { "Header1", "Header2" }, true)),
                new(
                    new FileDescription("OnlyHeader.csv", 0, DateTime.Now),
                    new DataDescription(new [] { "Header1", "Header2" }, false))
            };
            
            Assert.That(schemaModel, Has.Length.EqualTo(2));
            Assert.AreEqual(expectedModel[0].ClassName, schemaModel[0].ClassName);
            Assert.AreEqual(expectedModel[0].Headers, schemaModel[0].Headers);
            Assert.AreEqual(expectedModel[0].HasData, schemaModel[0].HasData);
            Assert.AreEqual(expectedModel[1].ClassName, schemaModel[1].ClassName);
            Assert.AreEqual(expectedModel[1].Headers, schemaModel[1].Headers);
            Assert.AreEqual(expectedModel[1].HasData, schemaModel[1].HasData);
        }
    }
}