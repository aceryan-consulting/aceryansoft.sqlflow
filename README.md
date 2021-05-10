# aceryansoft.sqlflow
C# light and fluent wrapper to easily use multiple sql databases (Sql server,Oracle,PostgreSql,Mysql,Sybase) with the same code while targeting dotnet standard 2.0 , dotnet framework 4.6.1 and above.

aceryansoft.sqlflow is not a real ORM (Object-Relational-Mapping), it is just a small contribution to help others who are old enough to continue building and querying their own sql schemas.  


## Install 
Using nuget
``` powershell 
PM> Install-Package aceryansoft.sqlflow
```
Using .net cli
``` powershell 
dotnet add package aceryansoft.sqlflow
```

## Hello sql world
``` c#
using System;
using System.Collections.Generic;
using aceryansoft.sqlflow;


namespace aceryansoft.sqlflow.console
{
 
    class Program
    {
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
                { // -- Auto mapping of fields name, age,BirthDay to Properties of object Person
                    {"idperson", "id" }, // map idperson value to id property of object Person 
                    {"phone", "Contacts.PhoneNumber" }, 
                    {"city", "Contacts.Home.City" },  // map city value to property Person.Contacts.Home.City
                    {"postalcode", "Contacts.Home.PostalCode" }
                },
                queryParameters: new Dictionary<string, object>()
                {
                    {"@dateofbirth", new DateTime( 1980,01,20) } // pass @dateofbirth to the sql query
                });
            Console.WriteLine($"Hello sql world ! we found {peoples.Count} people(s)");
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
```

## Features

#### ExecuteScalar 
``` c#
var oracleExecuter = SqlFlow.Create("connection String").WithOracleExecuter();
var someValue = oracleExecuter.ExecuteScalar<int>("select top 1 age from Persons"); 
```

#### ExecuteNonQuery 
``` c#
var sybaseExecuter = SqlFlow.Create("connection String").WithSybaseExecuter();
sybaseExecuter.ExecuteNonQuery("update Persons set age = age+1 where dateofbirth <= @dateofbirth", 
    queryParameters: new Dictionary<string, object>()
    {
        {"@dateofbirth", new DateTime( 1980,01,20) } // pass @dateofbirth to the sql query
    });
```

#### ExecuteReader 

``` c#
using aceryansoft.sqlflow.Helpers;
```

``` c#
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
```


## Contributing
All contribution are welcome, please read the [Code of conduct](https://github.com/aceryan-consulting/aceryansoft.sqlflow/blob/develop/CODE_OF_CONDUCT.md) and contact the author.
 

## License
This project is licensed under the terms of the Apache-2.0 License. 
Please read [LICENSE](LICENSE.md) file for license rights and limitations.
 
