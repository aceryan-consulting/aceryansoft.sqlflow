using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using NSubstitute;
using System;
using System.Collections.Generic;
using aceryansoft.sqlflow.Helpers;
using System.Linq;
using aceryansoft.sqlflow.Model;

namespace aceryansoft.sqlflow.tests
{
    public class Customer
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public decimal Amount { get; set; }
        public int TraderId { get; set; }
        public float Note { get; set; }
        public DateTime CreationDate { get; set; }
    }

    [TestClass]
    public class SqlFlowIntegrationTests
    {
        private string _localConnectionString = $"Server=(localdb)\\MSSQLLocalDB;AttachDBFilename=|DataDirectory|\\DbIntegrationTests.mdf;Trusted_Connection=true;MultipleActiveResultSets=true";
        
        [TestInitialize]
        public void TestInitialize()
        {
            var dataDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\netcoreapp3.1",""), "Databases");
            _localConnectionString = _localConnectionString.Replace("|DataDirectory|", dataDir); 
            var sqlserverExecuter = SqlFlow.Create(_localConnectionString).WithSqlServerExecuter();
            sqlserverExecuter.ExecuteNonQuery("delete from Customers");
        }

        [TestMethod]
        public void ShouldRunStoredProcedureAndRetrieveResults()
        {
            var sqlserverExecuter = SqlFlow.Create(_localConnectionString).WithSqlServerExecuter(); 
            var customerId = InsertCustomerData(sqlserverExecuter, "yan corp", 12000, 51003, 15.2F, DateTime.Now);
            var foundCustomer = sqlserverExecuter.ExecuteScalar<int>($"select count(*) from Customers where id={customerId}");
            Check.That(foundCustomer).IsEqualTo(1);
        }

        [TestMethod]
        public void ShouldReadDataFromReaderHandler()
        {
            var sqlserverExecuter = SqlFlow.Create(_localConnectionString).WithSqlServerExecuter(); 
            var customerId1 = InsertCustomerData(sqlserverExecuter, "corp 1", 1500, 5103, 9, DateTime.Now.AddDays(-1));
            var customerId2 = InsertCustomerData(sqlserverExecuter, "corp 2", 180, 5103, 11, DateTime.Now);
            var customerId3 = InsertCustomerData(sqlserverExecuter, "corp 3", 0, 5107, 17, DateTime.Now);
            var queryResults = new List<Customer>();
            sqlserverExecuter.ExecuteReader($"select * from Customers where id in ({customerId1},{customerId2},{customerId3})", (reader) =>
            {
                queryResults.Add(new Customer()
                {
                    Id = reader.GetValue<int>("id"),
                    CompanyName = reader.GetValue<string>("name"),
                    Amount = reader.GetValue<decimal>("amount"),
                    Note = reader.GetValue<float>("notation"),
                    TraderId = reader.GetValue<int>("traderid"),
                    CreationDate = reader.GetValue<DateTime>("creationdate")
                });
            });
            Check.That(queryResults.Count).IsEqualTo(3);

            var cust1 = queryResults.FirstOrDefault(x => x.Id == customerId1);
            Check.That(cust1.CompanyName).IsEqualTo("corp 1");
            Check.That(cust1.Amount).IsEqualTo(1500);
            Check.That(cust1.TraderId).IsEqualTo(5103);
            Check.That(cust1.Note).IsEqualTo(9); 
        }

        [TestMethod]
        public void ShouldMapDataWithQueryFieldNames()
        {
            var sqlserverExecuter = SqlFlow.Create(_localConnectionString).WithSqlServerExecuter();
            var customerId1 = InsertCustomerData(sqlserverExecuter, "corp 1", 1500, 5103, 9, DateTime.Now.AddDays(-1));
            var customerId2 = InsertCustomerData(sqlserverExecuter, "corp 2", 180, 5103, 11, DateTime.Now);
            var customerId3 = InsertCustomerData(sqlserverExecuter, "corp 3", 0, 5107, 17, DateTime.Now);
             
            var queryResults =  sqlserverExecuter.ExecuteReaderAndMap<Customer>(
                $"select id, name 'CompanyName',amount,notation 'note', traderid,creationdate from Customers where id in ({customerId1},{customerId2},{customerId3})");
            Check.That(queryResults.Count).IsEqualTo(3);

            var cust1 = queryResults.FirstOrDefault(x => x.Id == customerId1);
            Check.That(cust1.CompanyName).IsEqualTo("corp 1");
            Check.That(cust1.Amount).IsEqualTo(1500);
            Check.That(cust1.TraderId).IsEqualTo(5103);
            Check.That(cust1.Note).IsEqualTo(9);
        }


        [TestMethod]
        public void ShouldMapDataWithQueryFieldNamesAndPropertyMapping()
        {
            var sqlserverExecuter = SqlFlow.Create(_localConnectionString).WithSqlServerExecuter();
            var customerId1 = InsertCustomerData(sqlserverExecuter, "corp 1", 1500, 5103, 9, DateTime.Now.AddDays(-1));
            var customerId2 = InsertCustomerData(sqlserverExecuter, "corp 2", 180, 5103, 11, DateTime.Now);
            var customerId3 = InsertCustomerData(sqlserverExecuter, "corp 3", 0, 5107, 17, DateTime.Now);

            var queryResults = sqlserverExecuter.ExecuteReaderAndMap<Customer>(
                $"select id, name 'CompanyName',amount,notation, traderid,creationdate from Customers where id in ({customerId1},{customerId2},{customerId3})"
                ,new Dictionary<string, string>() { { "notation", "note" } });
            Check.That(queryResults.Count).IsEqualTo(3);

            var cust1 = queryResults.FirstOrDefault(x => x.Id == customerId1);
            Check.That(cust1.CompanyName).IsEqualTo("corp 1");
            Check.That(cust1.Amount).IsEqualTo(1500);
            Check.That(cust1.TraderId).IsEqualTo(5103);
            Check.That(cust1.Note).IsEqualTo(9);

            var cust2 = queryResults.FirstOrDefault(x => x.Id == customerId2);
            Check.That(cust2.Note).IsEqualTo(11);
        }

        public class Person
        {
            public string Name { get; set; }
            public Address Location { get; set; }
        }

        public class Address
        {
            public string City { get; set; }
            public int PostalCode { get; set; }
        }


        [TestMethod]
        public void ShouldMapInnerPropertyWithPropertyMapping()
        {
            var sqlserverExecuter = SqlFlow.Create(_localConnectionString).WithSqlServerExecuter();
            var query = $@"
select 'yan' as name, 'paris' as city, 75000 as postalcode
union all 
select 'pierre' as name, 'poissy' as city, 78300 as postalcode
";
            var queryResults = sqlserverExecuter.ExecuteReaderAndMap<Person>(query
                , new Dictionary<string, string>() { { "city", "Location.City" }, { "postalcode", "Location.PostalCode" } });

            var pierreRecord = queryResults.FirstOrDefault(x => x.Name == "pierre");
            Check.That(queryResults.Count).IsEqualTo(2);
            Check.That(pierreRecord).IsNotNull();
            Check.That(pierreRecord.Location).IsNotNull();
            Check.That(pierreRecord.Location.City).IsEqualTo("poissy"); 
            Check.That(pierreRecord.Location.PostalCode).IsEqualTo(78300);  
        }

        [TestMethod]
        public void ShouldUseNoQueryAndUpdateRecordInDataBase()
        {
            var sqlserverExecuter = SqlFlow.Create(_localConnectionString).WithSqlServerExecuter();
            var customerId1 = InsertCustomerData(sqlserverExecuter, "corp 1", 1500, 5103, 9, DateTime.Now.AddDays(-1));
            sqlserverExecuter.ExecuteNonQuery("update Customers set traderid=1285 where id=@custumerId"
                , new Dictionary<string, object>() { { "custumerId", customerId1 } });

            var queryResults = sqlserverExecuter.ExecuteReaderAndMap<Customer>(
                $"select id, name 'CompanyName',amount,notation 'note', traderid,creationdate from Customers where id = {customerId1}");
            Check.That(queryResults.Count).IsEqualTo(1);
             
            var cust1 = queryResults.FirstOrDefault(x => x.Id == customerId1);
            Check.That(cust1.CompanyName).IsEqualTo("corp 1"); 
            Check.That(cust1.TraderId).IsEqualTo(1285);  
        }


        [TestMethod]
        public void ShouldBulkInsertDataAndCheckExpectedValues()
        {
            var sqlserverExecuter = SqlFlow.Create(_localConnectionString).WithSqlServerExecuter();
            var cust1 = new Customer() { CompanyName="corp1_bulk_test",Note = 12,TraderId=11,Amount=150.45M,CreationDate=DateTime.Now };
            var cust2 = new Customer() { CompanyName= "corp2_bulk_test", Note = 13,TraderId=17,Amount=120,CreationDate=DateTime.Now };
            var cust3 = new Customer() { CompanyName= "corp3_bulk_test", Note = 14,TraderId=15,Amount=170,CreationDate=DateTime.Now };

            var columnMapping = new Dictionary<string, string>()
            {
                {"Id","id" }, // will be ignore by the bulk insert
                {"CompanyName","name" },
                {"Amount","amount" },
                {"Note","notation" },
                {"TraderId","traderid" },
                {"CreationDate","creationdate" }
            };
            sqlserverExecuter.BulkInsert<Customer>("Customers", new List<Customer>() { cust1, cust2, cust3 }, columnMapping);

            var queryResults = sqlserverExecuter.ExecuteReaderAndMap<Customer>(
                $"select id, name 'CompanyName',amount,notation 'note', traderid,creationdate from Customers where name like '%_bulk_test%' ");
            Check.That(queryResults.Count).IsEqualTo(3);

            var customer1 = queryResults.FirstOrDefault(x => x.CompanyName == cust1.CompanyName); 
            Check.That(cust1.Amount).IsEqualTo(150.45M);
            Check.That(cust1.TraderId).IsEqualTo(11);
            Check.That(cust1.Note).IsEqualTo(12);
        }

        private decimal InsertCustomerData(ISqlExecuter sqlExecuter ,string companyName,decimal amount, int traderId,float notation, DateTime creationDate)
        {
            var newCustomerParameters = new Dictionary<string, object>()
            {
                {"@name", companyName },
                {"@amount", amount },
                {"@notation", notation },
                {"@traderid", traderId },
                {"@creationdate", creationDate }
            };
            return sqlExecuter.ExecuteScalar<decimal>("sp_insert_customers", newCustomerParameters, true);
        }

        [TestMethod]
        public void ShouldExecuteNonQueryWithOutParameterAndRetrieveParameterValue()
        {
            var sqlserverExecuter = SqlFlow.Create(_localConnectionString).WithSqlServerExecuter();
            int newCustomerId = 0;

            sqlserverExecuter.ExecuteNonQuery(
                @"Insert into Customers( name , amount , notation , traderid , creationdate )
                             values(@name, @amount, @notation, @traderid, @creationdate);
                  select @customerId=@@IDENTITY"
                , new Dictionary<string, object>() {
                    {"@name", "corp model x" },
                    {"@amount", 1503 },
                    {"@notation", 18.2 },
                    {"@traderid", 169 },
                    {"@creationdate", DateTime.Now },
                    {"@customerId", new QueryParameter<int>() // generic required to set the db parameter size and avoid System.InvalidOperationException: String[xxx]: the Size property has an invalid size of 0. 
                        { 
                            IsOuputParameter = true,
                            GetOutputParameterValue = (val)=>{newCustomerId = (int) val; }
                        }
                    }

                });
            Check.That(newCustomerId).IsNotEqualTo(0);
        }


        [TestMethod]
        public void ShouldStoreProcedureWithOutParameterAndRetrieveParameterValue()
        {
            var sqlserverExecuter = SqlFlow.Create(_localConnectionString).WithSqlServerExecuter();
            int newCustomerId = 0;

            sqlserverExecuter.ExecuteNonQuery(
                "sp_insert_customers_with_output_parameter"
                , new Dictionary<string, object>() {
                    {"@name", "corp model y" },
                    {"@amount", 1503 },
                    {"@notation", 18.2 },
                    {"@traderid", 169 },
                    {"@creationdate", DateTime.Now },
                    {"@customerId", new QueryParameter<int>()  
                        {
                            IsOuputParameter = true,
                            GetOutputParameterValue = (val)=>{newCustomerId = (int) val; }
                        }
                    }
                },isStoreProcedure:true);
            Check.That(newCustomerId).IsNotEqualTo(0);
        }

         

        public class SampleUser
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public decimal Height { get; set; }
        }

        public class SampleCity
        {
            public string Name { get; set; }
            public int PostalCode { get; set; } 
        }

        [TestMethod]
        public void ShouldExecuteMultipleResultsSetQueriesAndReadResults()
        {
            var sqlserverExecuter = SqlFlow.Create(_localConnectionString).WithSqlServerExecuter();
            var Users = new List<SampleUser>();
            var Cities = new List<SampleCity>();
            var multiResultSetQuery = @"
                select 'yannick' as username, 24 as age, 1.8 as height
                union 
                select 'pierre' as username, 34 as age, 1.85 as height
                union 
                select 'anne' as username, 7 as age, 2 as height; 

                select 'paris' as city, 75001 as postalcode 
                union
                select 'nanterre' as city, 92000 as postalcode";

            sqlserverExecuter.ExecuteReaderOnMultipleResultsSet(
                multiResultSetQuery,(reader, index)=>
                {
                    if (index == 0)
                    {
                        Users.Add(new SampleUser()
                        {
                            Name = reader.GetValue<string>("username"),
                            Age = reader.GetValue<int>("age"),
                            Height = reader.GetValue<decimal>("height"),
                        });
                    }
                    else if (index == 1)
                    {
                        Cities.Add(new SampleCity()
                        {
                            Name = reader.GetValue<string>("city"),
                            PostalCode = reader.GetValue<int>("postalcode") 
                        });
                    }
                });
            Check.That(Users.Count).IsEqualTo(3);
            Check.That(Cities.Count).IsEqualTo(2);

            var yan = Users.FirstOrDefault(x => x.Name == "yannick");
            Check.That(yan).IsNotNull();
            Check.That(yan.Height).IsEqualTo(1.8);
        }

    }
}
