// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoneyConverterTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient.Tests
{
    using System.Text.Json;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.SqlClient;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// Tests the <see cref="MoneyConverter"/> class.
    /// </summary>
    [TestClass]
    public class MoneyConverterTests
    {
        /// <summary>
        /// Tests the Read method.
        /// </summary>
        [TestMethod]
        public void Read_JsonStringValueForMoney_SqlMoneyValueMatchesExpected()
        {
            const string Input = "{\"MoneyElementId\":334,\"Value\":44323.33}";
            var serializationOptions = new JsonSerializerOptions();
            serializationOptions.Converters.Add(new MoneyConverter());
            var actual = JsonSerializer.Deserialize<MoneyElementRow>(Input, serializationOptions);
            Assert.AreEqual(44323.33M, actual?.Value);
        }

        /// <summary>
        /// Tests the Write method.
        /// </summary>
        [TestMethod]
        public void Write_MoneyElementEntity_MoneyValueMatchesExpected()
        {
            var item = new MoneyElementRow { MoneyElementId = 334, Value = 44323.33M };
            var serializationOptions = new JsonSerializerOptions();
            serializationOptions.Converters.Add(new MoneyConverter());
            var jsonDocument = JsonDocument.Parse(JsonSerializer.Serialize(item, serializationOptions));
            Assert.AreEqual(item.Value, jsonDocument.RootElement.GetProperty("Value").GetDecimal());
        }
    }
}