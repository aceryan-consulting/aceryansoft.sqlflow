using aceryansoft.sqlflow.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aceryansoft.sqlflow.tests
{
    public class Street
    {
        public string Name { get; set; }
        public int Number { get; set; }
    }

    public class City
    {
        public string Name { get; set; }
        public int PostalCode { get; set; }
    }

    public class Address
    {
        public Street Street { get; set; }
        public City City { get; set; }
        public List<string> Schools { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
        public double Age { get; set; }
        public Address Location { get; set; }
    }

    [TestClass]
    public class ReflectionHelperUnitTests
    {
        [TestMethod]
        public void ShouldUpdateInnerPropertyValuesWhenFound()
        {
            var usr = new User() { Name = "yan" };
            ReflectionHelper.SetInnerPropertyValue(usr, 15, "Location.Street.Number");
            ReflectionHelper.SetInnerPropertyValue(usr, "nelson", "Location.Street.Name");

            Check.That(usr.Location).IsNotNull();
            Check.That(usr.Location.Street).IsNotNull();
            Check.That(usr.Location.Street.Number).IsEqualTo(15);
            Check.That(usr.Location.Street.Name).IsEqualTo("nelson");
        }

        [TestMethod]
        public void ShouldUpdateInnerPropertyObjectWhenFound()
        {
            var usr = new User() { Name = "yan" };
            var newCity = new City() { Name = "paris", PostalCode = 75000 };
            ReflectionHelper.SetInnerPropertyValue(usr, newCity, "Location.City");

            Check.That(usr.Location).IsNotNull();
            Check.That(usr.Location.City).IsNotNull();
            Check.That(usr.Location.City.Name).IsEqualTo(newCity.Name);
            Check.That(usr.Location.City.PostalCode).IsEqualTo(newCity.PostalCode);
        }

        [TestMethod]
        public void ShouldUpdateInnerPropertyCollectionWhenFound()
        {
            var usr = new User() { Name = "yan" }; 
            ReflectionHelper.SetInnerPropertyValue(usr, new List<string>() { "galileo"}, "Location.Schools");

            Check.That(usr.Location).IsNotNull();
            Check.That(usr.Location.Schools).IsNotNull();
            Check.That(usr.Location.Schools.Count).IsEqualTo(1);
            Check.That(usr.Location.Schools.Where(x=>x== "galileo").Count()).IsEqualTo(1); 

        }
        [TestMethod]
        public void ShouldThrowExceptionWhenCalledWithoutPropertySeparator()
        {
            var usr = new User() { Name = "yan" };
            Check.ThatCode(() =>
            {
                ReflectionHelper.SetInnerPropertyValue(usr, 75000, "Location,City", '.');
            }).Throws<ArgumentException>();     
        }

        [TestMethod]
        public void ShouldNotUpdateObjectIfPropertyPathIsInvalid()
        {
            var usr = new User() { Name = "yan" }; 
            ReflectionHelper.SetInnerPropertyValue(usr, 15, "Unknown.City");

            Check.That(usr.Name).IsEqualTo(usr.Name);
            Check.That(usr.Location).IsNull();            
        }
    }
}
