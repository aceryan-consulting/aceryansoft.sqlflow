using aceryansoft.sqlflow.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aceryansoft.sqlflow.tests
{
    [TestClass]
    public class QueryHelperTests
    {

        private List<Customer> GetCustomers()
        {
            var customer1 = new Customer()
            {
                Id = 1,
                Amount = 15.2M,
                CompanyName = "aceryan",
                CreationDate = DateTime.Now,
                Note = 154,
                TraderId = 1
            };
            var customer2 = new Customer()
            {
                Id = 2,
                Amount = 20,
                CompanyName = "tesla",
                CreationDate = DateTime.Now,
                Note = 11,
                TraderId = 2
            };
            return new List<Customer>() { customer1,customer2 };
        }

        [TestMethod]
        public void ShouldGenerateInsertIntoQueryForSelectedFields()
        {
            var customers = GetCustomers();
            var columnsMapping = new Dictionary<string, string>()
            {
                {"Id", "id"},
                {"Amount", "amount"},
                {"CompanyName", "company_name"},
                {"TraderId", "trader_id"},
            };
            var sortedColumnMapping = new SortedDictionary<string, string>(columnsMapping);
            var objValueResolver = QueryHelper.BuildObjectValueProvider<Customer>(sortedColumnMapping);
            var insertIntoQuery = QueryHelper.BuildInsertIntoQuery(customers, "customers", sortedColumnMapping, objValueResolver);

            Check.That(insertIntoQuery.query).IsEqualTo("insert into customers ( amount , company_name , id , trader_id ) values  ( @amount0 , @company_name0 , @id0 , @trader_id0 )  ,  ( @amount1 , @company_name1 , @id1 , @trader_id1 )  ; ");

            var expectedParameters = columnsMapping.Count * customers.Count;
            Check.That(insertIntoQuery.queryParameters.Count).IsEqualTo(expectedParameters);
            Check.That(insertIntoQuery.queryParameters["@id0"]).IsEqualTo(1);
            Check.That(insertIntoQuery.queryParameters["@id1"]).IsEqualTo(2);
            Check.That(insertIntoQuery.queryParameters["@company_name0"]).IsEqualTo("aceryan");
            Check.That(insertIntoQuery.queryParameters["@company_name1"]).IsEqualTo("tesla");
        }

        [TestMethod]
        public void ShouldGenerateInsertIntoQueryForAllFields()
        {
            var customers = GetCustomers();
            var columnsMapping = typeof(Customer).GetProperties().ToDictionary(x=>x.Name, x=>x.Name.ToLower());
            var sortedColumnMapping = new SortedDictionary<string, string>(columnsMapping);
            var objValueResolver = QueryHelper.BuildObjectValueProvider<Customer>(sortedColumnMapping);
            var insertIntoQuery = QueryHelper.BuildInsertIntoQuery(customers, "customers", sortedColumnMapping, objValueResolver);
            // test can fail if class Customer is modified, update if required 
            Check.That(insertIntoQuery.query).IsEqualTo("insert into customers ( amount , companyname , creationdate , id , note , traderid ) values  ( @amount0 , @companyname0 , @creationdate0 , @id0 , @note0 , @traderid0 )  ,  ( @amount1 , @companyname1 , @creationdate1 , @id1 , @note1 , @traderid1 )  ; ");

            var expectedParameters = columnsMapping.Count * customers.Count;
            Check.That(insertIntoQuery.queryParameters.Count).IsEqualTo(expectedParameters);
            Check.That(insertIntoQuery.queryParameters["@id0"]).IsEqualTo(1);
            Check.That(insertIntoQuery.queryParameters["@id1"]).IsEqualTo(2);
            Check.That(insertIntoQuery.queryParameters["@note0"]).IsEqualTo(154);
            Check.That(insertIntoQuery.queryParameters["@note1"]).IsEqualTo(11);
        }
    }
}
