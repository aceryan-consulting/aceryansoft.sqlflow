using System;
using System.Collections.Generic;
using aceryansoft.sqlflow;
using aceryansoft.sqlflow.Helpers;

namespace aceryansoft.sqlflow.console
{
 
    class Program
    {
        // Hello sql world 
        static void Main(string[] args)
        {
            var sqlServerExecuter = SqlFlow.Create("connection String").WithSqlServerExecuter();
            // select other SGBD .WithMySqlExecuter() .WithOracleExecuter() .WithPostGreSqlExecuter() .WithSybaseExecuter()
            var peopleQuery = @"select pe.idperson 
                                ,pe.name, pe.age  
                                , pe.dateofbirth 'BirthDay'
                                , co.phone , co.city, co.postalcode
                                from Persons pe 
                                inner join Contacts co on pe.idperson= co.idperson
                                where pe.dateofbirth <= @dateofbirth ";

            var peoples = sqlServerExecuter.ExecuteReaderAndMap<Person>(peopleQuery,
                propertiesMapping: new Dictionary<string, string>()
                { // -- Automatic map fields name, age,BirthDay to Properties of object Person
                    {"idperson", "id" }, // map idperson value to id property of object Person 
                    {"phone", "Contacts.PhoneNumber" }, 
                    {"city", "Contacts.Home.City" },  // map city value to property Person.Contacts.Home.City
                    {"postalcode", "Contacts.Home.PostalCode" }
                },
                queryParameters: new Dictionary<string, object>()
                {
                    {"@dateofbirth", new DateTime( 1980,01,20) } // pass @dateofbirth to the sql query
                });
            Console.WriteLine($"Hello sql hello world ! we found {peoples.Count} people(s)");
        }

        static void Main2(string[] args)
        {
            var oracleExecuter = SqlFlow.Create("connection String").WithOracleExecuter();
            var someValue = oracleExecuter.ExecuteScalar<int>("select top 1 age from Persons"); 
        }

        static void Main3(string[] args)
        {
            var sybaseExecuter = SqlFlow.Create("connection String").WithSybaseExecuter();
            sybaseExecuter.ExecuteNonQuery("update Persons set age = age+1 where dateofbirth <= @dateofbirth", 
                queryParameters: new Dictionary<string, object>()
                {
                    {"@dateofbirth", new DateTime( 1980,01,20) } // pass @dateofbirth to the sql query
                });
        }

        
        static void Main4(string[] args)
        {
            var peoples = new List<Person>();
            var postgreExecuter = SqlFlow.Create("connection String").WithPostGreSqlExecuter();
            postgreExecuter.ExecuteReader(
                @"select pe.idperson 
                                ,pe.name, pe.age  
                                , pe.dateofbirth 'BirthDay'
                                , co.phone , co.city, co.postalcode
                                from Persons pe 
                                inner join Contacts co on pe.idperson= co.idperson
                                where pe.dateofbirth <= @dateofbirth ",
                reader=>
                {
                    peoples.Add(new Person()
                    {
                        Id = reader.GetValue<long>("idperson"),
                        Age = reader.GetValue<int>("age"),
                        Name = reader.GetValue<string>("name"),
                        BirthDay = reader.GetValue<DateTime>("BirthDay", DateTime.Now.AddDays(-10)),
                        Contacts = new Contact()
                        {
                            PhoneNumber = reader.GetValue<string>("phone"),
                            Home = new Address()
                            {
                                City = reader.GetValue<string>("city"),
                                PostalCode = reader.GetValue<int>("postalcode"),
                            }
                        }
                    });
                },
                queryParameters: new Dictionary<string, object>()
                {
                    {"@dateofbirth", new DateTime( 1980,01,20) } // pass @dateofbirth to the sql query
                });
            Console.WriteLine($"Read sql records with custom logic, we found {peoples.Count} people(s)");
        }

        static void Main5(string[] args)
        {
            var peoples = new List<Person>();
            var mySqlExecuter = SqlFlow.Create("connection String").WithMySqlExecuter();

            var spParameters = new Dictionary<string, object>()
            { 
                {"@age", 21 },
                {"@name", "yannick" },
                {"@birthday", DateTime.Now.AddYears(-21) } 
            };

           var newpersonId = mySqlExecuter.ExecuteScalar<decimal>(
                query: @"sp_insert_persons"
                , queryParameters: spParameters
                ,isStoreProcedure:true); // set isStoreProcedure=true to execute query as store procedure         
            Console.WriteLine($"execute store procedure and get new person id = {newpersonId}");
            //store procedure can also be called with ExecuteReader, ExecuteNonQuery, ExecuteReaderAndMap
        }

        static void Main6(string[] args)
        { 
            var oracleExecuter = SqlFlow.Create("connection String").WithOracleExecuter();
            oracleExecuter.RunTransaction((sqlexecuter, dbConnexion, dbTransaction) =>
            {
                var storeprocParams = new Dictionary<string, object>()
                { 
                    {"@age", 21 },
                    {"@name", "yannick" },
                    {"@birthday", DateTime.Now.AddYears(-21) }
                };

                var newpersonId = sqlexecuter.ExecuteScalar<decimal>(
                     query: @"sp_insert_persons"
                     , queryParameters: storeprocParams
                     , isStoreProcedure: true);

                var spcontactParams = new Dictionary<string, object>()
                {
                    {"@idperson", newpersonId },
                    {"@phone", "007" },
                    {"@city", "paris" },
                    {"@postalcode", 78300 } 
                };

                sqlexecuter.ExecuteNonQuery(
                     query: @"sp_insert_contact"
                     , queryParameters: spcontactParams
                     , isStoreProcedure: true);
            }); 
        }

        static void Main7(string[] args)
        {
            var oracleExecuter = SqlFlow.Create("connection String").WithOracleExecuter(); // .WithSqlServerExecuter()

            var peoples = new List<Person>()
            {
                new Person(){Name="yan", Age=21},
                new Person(){Name="pierre", Age=51},
                new Person(){Name="philippe", Age=43},
                new Person(){Name="marc", Age=27},
                new Person(){Name="edouard", Age=62, Contacts = new Contact()
                {
                  Home = new Address()
                  {
                      City="dakar",
                      PostalCode=27
                  },
                  PhoneNumber = "06"
              } },
            };
            var propertyMapping = new Dictionary<string, string>() {{ "Name","user_name" } , { "Age", "age" }  };
            oracleExecuter.BulkInsert<Person>("Persons", peoples, propertyMapping, batchSize: 500);
        }

        static void Main8(string[] args)
        {
            var postGreSqlExecuter = SqlFlow.Create("connection String").WithPostGreSqlExecuter(); // .WithSqlServerExecuter() .WithMySqlExecuter()

            var peoples = new List<Person>()
            {
                new Person(){Name="yan", Age=21},
                new Person(){Name="pierre", Age=51},
                new Person(){Name="philippe", Age=43},
                new Person(){Name="marc", Age=27},
                new Person(){Name="edouard", Age=62, Contacts = new Contact()
                {
                    Home = new Address()
                    {
                        City="dakar",
                        PostalCode=27
                    },
                    PhoneNumber = "06"
                } },
            };
            var propertyMapping = new Dictionary<string, string>() { { "Name", "user_name" }, { "Age", "age" } };
            postGreSqlExecuter.BatchInsertRows("Persons", peoples, propertyMapping, batchSize: 500);
        }
    }

    public class Person
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime BirthDay { get; set; }
        public Contact Contacts { get; set; }

    }

    public class Contact
    {
        public String PhoneNumber { get; set; }
        public Address Home { get; set; }
    }

    public class Address
    {
        public string City { get; set; }
        public int PostalCode { get; set; }
    }

}
