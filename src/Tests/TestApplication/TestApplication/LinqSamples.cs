using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApplication
{

    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
    }

    public class Order
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class Customer
    {
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public Order[] Orders { get; set; }
    }

    public class LinqSamples
    {
        private List<Product> productList;
        private List<Customer> customerList;

        [Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
        public void Linq1()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var lowNums =
                from num in numbers
                where num < 5
                select num;

            Debug.WriteLine("Numbers < 5:");
            foreach (var x in lowNums)
            {
                Debug.WriteLine(x);
            }
        }

        [Description("This sample uses the where clause to find all products that are out of stock.")]
        public void Linq2()
        {
            List<Product> products = GetProductList();

            var soldOutProducts =
                from prod in products
                where prod.UnitsInStock == 0
                select prod;

            Debug.WriteLine("Sold out products:");
            foreach (var product in soldOutProducts)
            {
                Debug.WriteLine("{0} is sold out!", product.ProductName);
            }
        }

        [Description("This sample uses the where clause to find all products that are in stock and " +
                 "cost more than 3.00 per unit.")]
        public void Linq3()
        {
            List<Product> products = GetProductList();

            var expensiveInStockProducts =
                from prod in products
                where prod.UnitsInStock > 0 && prod.UnitPrice > 3.00
                select prod;

            Debug.WriteLine("In-stock products that cost more than 3.00:");
            foreach (var product in expensiveInStockProducts)
            {
                Debug.WriteLine("{0} is in stock and costs more than 3.00.", product.ProductName);
            }
        }

        //[Description("This sample uses the where clause to find all customers in Washington " +
        //         "and then it uses a foreach loop to iterate over the orders collection that belongs to each customer.")]
        //public void Linq4()
        //{
        //    List<Customer> customers = GetCustomerList();

        //    var waCustomers =
        //        from cust in customers
        //        where cust.Region == "WA"
        //        select cust;

        //    Debug.WriteLine("Customers from Washington and their orders:");
        //    foreach (var customer in waCustomers)
        //    {
        //        Debug.WriteLine("Customer {0}: {1}", customer.CustomerID, customer.CompanyName);
        //        foreach (var order in customer.Orders)
        //        {
        //            Debug.WriteLine("  Order {0}: {1}", order.OrderID, order.OrderDate);
        //        }
        //    }
        //}

        [Description("This sample demonstrates an indexed where clause that returns digits whose name is " +
                "shorter than their value.")]
        public void Linq5()
        {
            string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var shortDigits = digits.Where((digit, index) => digit.Length < index);

            Debug.WriteLine("Short digits:");
            foreach (var d in shortDigits)
            {
                Debug.WriteLine("The word {0} is shorter than its value.", d);
            }
        }

        public void Linq6()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var numsPlusOne =
                from n in numbers
                select n + 1;

            Debug.WriteLine("Numbers + 1:");
            foreach (var i in numsPlusOne)
            {
                Debug.WriteLine(i);
            }
        }

        public void Linq7()
        {
            List<Product> products = GetProductList();

            var productNames =
                from p in products
                select p.ProductName;

            Debug.WriteLine("Product Names:");
            foreach (var productName in productNames)
            {
                Debug.WriteLine(productName);
            }
        }

        public void Linq8()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            string[] strings = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var textNums =
                from n in numbers
                select strings[n];

            Debug.WriteLine("Number strings:");
            foreach (var s in textNums)
            {
                Debug.WriteLine(s);
            }
        }

        public void Linq9()
        {
            string[] words = { "aPPLE", "BlUeBeRrY", "cHeRry" };

            var upperLowerWords =
                from w in words
                select new { Upper = w.ToUpper(), Lower = w.ToLower() };

            foreach (var ul in upperLowerWords)
            {
                Debug.WriteLine("Uppercase: {0}, Lowercase: {1}", ul.Upper, ul.Lower);
            }
        }

        public void Linq10()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            string[] strings = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var digitOddEvens =
                from n in numbers
                select new { Digit = strings[n], Even = (n % 2 == 0) };

            foreach (var d in digitOddEvens)
            {
                Debug.WriteLine("The digit {0} is {1}.", d.Digit, d.Even ? "even" : "odd");
            }
        }

        public void Linq11()
        {
            List<Product> products = GetProductList();

            var productInfos =
                from p in products
                select new { p.ProductName, p.Category, Price = p.UnitPrice };

            Debug.WriteLine("Product Info:");
            foreach (var productInfo in productInfos)
            {
                Debug.WriteLine("{0} is in the category {1} and costs {2} per unit.", productInfo.ProductName, productInfo.Category, productInfo.Price);
            }
        }

        public void Linq12()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var numsInPlace = numbers.Select((num, index) => new { Num = num, InPlace = (num == index) });

            Debug.WriteLine("Number: In-place?");
            foreach (var n in numsInPlace)
            {
                Debug.WriteLine("{0}: {1}", n.Num, n.InPlace);
            }
        }

        public void Linq13()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var lowNums =
                from n in numbers
                where n < 5
                select digits[n];

            Debug.WriteLine("Numbers < 5:");
            foreach (var num in lowNums)
            {
                Debug.WriteLine(num);
            }
        }

        public void Linq14()
        {
            // (SELECT MANY WITH "TCOLLECTION")

            // NOT SUPPORTED

            /*
            int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
            int[] numbersB = { 1, 3, 5, 7, 8 };

            var pairs =
                from a in numbersA
                from b in numbersB
                where a < b
                select new { a, b };

            Debug.WriteLine("Pairs where a < b:");
            foreach (var pair in pairs)
            {
                Debug.WriteLine("{0} is less than {1}", pair.a, pair.b);
            }
             */
        }

        public void Linq29()
        {
            string[] words = { "cherry", "apple", "blueberry" };

            var sortedWords =
                from w in words
                orderby w.Length
                select w;

            Debug.WriteLine("The sorted list of words (by length):");
            foreach (var w in sortedWords)
            {
                Debug.WriteLine(w);
            }
        }

        public void Linq30()
        {
            List<Product> products = GetProductList();

            var sortedProducts =
                from p in products
                orderby p.ProductName
                select p;

            Debug.WriteLine("Sorted products:");
            foreach (var p in sortedProducts)
            {
                Debug.WriteLine(p.ProductName);
            }
        }

        public void Linq31()
        {
            string[] words = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry" };

            var sortedWords = words.OrderBy(a => a, new ComparerBasedOnLastLetter());

            Debug.WriteLine("\r\nSorted by last letter:");
            foreach (var w in sortedWords)
            {
                Debug.WriteLine(w);
            }
        }

        public class ComparerBasedOnLastLetter : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return string.Compare(x.Substring(x.Length - 1), y.Substring(y.Length - 1), StringComparison.OrdinalIgnoreCase);
            }
        }

        [Description("This sample uses ToArray to immediately evaluate a sequence into an array.")]
        public void Linq54()
        {
            double[] doubles = { 1.7, 2.3, 1.9, 4.1, 2.9 };

            var sortedDoubles =
                from d in doubles
                orderby d descending
                select d;
            var doublesArray = sortedDoubles.ToArray();

            Debug.WriteLine("Every other double from highest to lowest:");
            for (int d = 0; d < doublesArray.Length; d += 2)
            {
                Debug.WriteLine(doublesArray[d]);
            }
        }

        [Description("This sample uses ToList to immediately evaluate a sequence into a List<T>.")]
        public void Linq55()
        {
            string[] words = { "cherry", "apple", "blueberry" };

            var sortedWords =
                from w in words
                orderby w
                select w;
            var wordList = sortedWords.ToList();

            Debug.WriteLine("The sorted word list:");
            foreach (var w in wordList)
            {
                Debug.WriteLine(w);
            }
        }

        [Description("This sample uses ToDictionary to immediately evaluate a sequence and a " +
                    "related key expression into a dictionary.")]
        public void Linq56()
        {
            // "TODICTIONARY"
            // NOT SUPPORTED

            /*
            var scoreRecords = new[] { new {Name = "Alice", Score = 50},
                                        new {Name = "Bob"  , Score = 40},
                                        new {Name = "Cathy", Score = 45}
                                    };

            var scoreRecordsDict = scoreRecords.ToDictionary(sr => sr.Name);

            Debug.WriteLine("Bob's score: {0}", scoreRecordsDict["Bob"]);
            */
        }

        [Description("This sample uses OfType to return only the elements of the array that are of type double.")]
        public void Linq57()
        {

            // "OFTYPE" with numbers
            // NOT SUPPORTED

            /*
            object[] numbers = { null, 1.0, "two", 3, "four", 5, "six", 7.0 };

            var doubles = numbers.OfType<double>();

            Debug.WriteLine("Numbers stored as doubles:");
            foreach (var d in doubles)
            {
                Debug.WriteLine(d);
            }
             */
        }

        public void Linq78()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            double numSum = numbers.Sum();

            Debug.WriteLine("The sum of the numbers is {0}.", numSum);
        }


        public void Linq79()
        {
            // SUM WITH SELECTOR
            // NOT SUPPORTED

            /*
            string[] words = { "cherry", "apple", "blueberry" };

            double totalChars = words.Sum(w => w.Length);

            Debug.WriteLine("There are a total of {0} characters in these words.", totalChars);
            */
        }

        public void LinqCustom1()
        {
            List<Product> products = GetProductList();

            var result =
                from p in products
                where p.ProductID > 70
                orderby p.ProductName
                select p.ProductName;
            var resultList = result.ToList();

            Debug.WriteLine("Custom1:");
            Debug.WriteLine(string.Join(", ", result));
            //foreach (var r in resultList)
            //{
            //    Debug.WriteLine(r);
            //}
        }

        public List<Product> GetProductList()
        {
            if (productList == null)
                createLists();

            return productList;
        }

        //public List<Customer> GetCustomerList()
        //{
        //    if (customerList == null)
        //        createLists();

        //    return customerList;
        //}

        private void createLists()
        {
            // Product data created in-memory using collection initializer:
            productList =
                new List<Product> {
                    new Product { ProductID = 1, ProductName = "Chai", Category = "Beverages", UnitPrice = 18, UnitsInStock = 39 },
                    new Product { ProductID = 2, ProductName = "Chang", Category = "Beverages", UnitPrice = 19, UnitsInStock = 17 },
                    new Product { ProductID = 3, ProductName = "Aniseed Syrup", Category = "Condiments", UnitPrice = 10, UnitsInStock = 13 },
                    new Product { ProductID = 4, ProductName = "Chef Anton's Cajun Seasoning", Category = "Condiments", UnitPrice = 22, UnitsInStock = 53 },
                    new Product { ProductID = 5, ProductName = "Chef Anton's Gumbo Mix", Category = "Condiments", UnitPrice = 21, UnitsInStock = 0 },
                    new Product { ProductID = 6, ProductName = "Grandma's Boysenberry Spread", Category = "Condiments", UnitPrice = 25, UnitsInStock = 120 },
                    new Product { ProductID = 7, ProductName = "Uncle Bob's Organic Dried Pears", Category = "Produce", UnitPrice = 30, UnitsInStock = 15 },
                    new Product { ProductID = 8, ProductName = "Northwoods Cranberry Sauce", Category = "Condiments", UnitPrice = 40, UnitsInStock = 6 },
                    new Product { ProductID = 9, ProductName = "Mishi Kobe Niku", Category = "Meat/Poultry", UnitPrice = 97, UnitsInStock = 29 },
                    new Product { ProductID = 10, ProductName = "Ikura", Category = "Seafood", UnitPrice = 31, UnitsInStock = 31 },
                    new Product { ProductID = 11, ProductName = "Queso Cabrales", Category = "Dairy Products", UnitPrice = 21, UnitsInStock = 22 },
                    new Product { ProductID = 12, ProductName = "Queso Manchego La Pastora", Category = "Dairy Products", UnitPrice = 38, UnitsInStock = 86 },
                    new Product { ProductID = 13, ProductName = "Konbu", Category = "Seafood", UnitPrice = 6, UnitsInStock = 24 },
                    new Product { ProductID = 14, ProductName = "Tofu", Category = "Produce", UnitPrice = 23, UnitsInStock = 35 },
                    new Product { ProductID = 15, ProductName = "Genen Shouyu", Category = "Condiments", UnitPrice = 15, UnitsInStock = 39 },
                    new Product { ProductID = 16, ProductName = "Pavlova", Category = "Confections", UnitPrice = 17, UnitsInStock = 29 },
                    new Product { ProductID = 17, ProductName = "Alice Mutton", Category = "Meat/Poultry", UnitPrice = 39, UnitsInStock = 0 },
                    new Product { ProductID = 18, ProductName = "Carnarvon Tigers", Category = "Seafood", UnitPrice = 62, UnitsInStock = 42 },
                    new Product { ProductID = 19, ProductName = "Teatime Chocolate Biscuits", Category = "Confections", UnitPrice = 9, UnitsInStock = 25 },
                    new Product { ProductID = 20, ProductName = "Sir Rodney's Marmalade", Category = "Confections", UnitPrice = 81, UnitsInStock = 40 },
                    new Product { ProductID = 21, ProductName = "Sir Rodney's Scones", Category = "Confections", UnitPrice = 10, UnitsInStock = 3 },
                    new Product { ProductID = 22, ProductName = "Gustaf's Knäckebröd", Category = "Grains/Cereals", UnitPrice = 21, UnitsInStock = 104 },
                    new Product { ProductID = 23, ProductName = "Tunnbröd", Category = "Grains/Cereals", UnitPrice = 9, UnitsInStock = 61 },
                    new Product { ProductID = 24, ProductName = "Guaraná Fantástica", Category = "Beverages", UnitPrice = 4, UnitsInStock = 20 },
                    new Product { ProductID = 25, ProductName = "NuNuCa Nuß-Nougat-Creme", Category = "Confections", UnitPrice = 14, UnitsInStock = 76 },
                    new Product { ProductID = 26, ProductName = "Gumbär Gummibärchen", Category = "Confections", UnitPrice = 31, UnitsInStock = 15 },
                    new Product { ProductID = 31, ProductName = "Gorgonzola Telino", Category = "Dairy Products", UnitPrice = 12, UnitsInStock = 0 },
                    new Product { ProductID = 32, ProductName = "Mascarpone Fabioli", Category = "Dairy Products", UnitPrice = 32, UnitsInStock = 9 },
                    new Product { ProductID = 33, ProductName = "Geitost", Category = "Dairy Products", UnitPrice = 2, UnitsInStock = 112 },
                    new Product { ProductID = 34, ProductName = "Sasquatch Ale", Category = "Beverages", UnitPrice = 14, UnitsInStock = 111 },
                    new Product { ProductID = 35, ProductName = "Steeleye Stout", Category = "Beverages", UnitPrice = 18, UnitsInStock = 20 },
                    new Product { ProductID = 36, ProductName = "Inlagd Sill", Category = "Seafood", UnitPrice = 19, UnitsInStock = 112 },
                    new Product { ProductID = 37, ProductName = "Gravad lax", Category = "Seafood", UnitPrice = 26, UnitsInStock = 11 },
                    new Product { ProductID = 38, ProductName = "Côte de Blaye", Category = "Beverages", UnitPrice = 263, UnitsInStock = 17 },
                    new Product { ProductID = 39, ProductName = "Chartreuse verte", Category = "Beverages", UnitPrice = 18, UnitsInStock = 69 },
                    new Product { ProductID = 42, ProductName = "Singaporean Hokkien Fried Mee", Category = "Grains/Cereals", UnitPrice = 14, UnitsInStock = 26 },
                    new Product { ProductID = 43, ProductName = "Ipoh Coffee", Category = "Beverages", UnitPrice = 46, UnitsInStock = 17 },
                    new Product { ProductID = 44, ProductName = "Gula Malacca", Category = "Condiments", UnitPrice = 19, UnitsInStock = 27 },
                    new Product { ProductID = 45, ProductName = "Rogede sild", Category = "Seafood", UnitPrice = 9, UnitsInStock = 5 },
                    new Product { ProductID = 46, ProductName = "Spegesild", Category = "Seafood", UnitPrice = 12, UnitsInStock = 95 },
                    new Product { ProductID = 47, ProductName = "Zaanse koeken", Category = "Confections", UnitPrice = 9, UnitsInStock = 36 },
                    new Product { ProductID = 49, ProductName = "Maxilaku", Category = "Confections", UnitPrice = 20, UnitsInStock = 10 },
                    new Product { ProductID = 50, ProductName = "Valkoinen suklaa", Category = "Confections", UnitPrice = 16, UnitsInStock = 65 },
                    new Product { ProductID = 51, ProductName = "Manjimup Dried Apples", Category = "Produce", UnitPrice = 53, UnitsInStock = 20 },
                    new Product { ProductID = 52, ProductName = "Filo Mix", Category = "Grains/Cereals", UnitPrice = 7, UnitsInStock = 38 },
                    new Product { ProductID = 54, ProductName = "Tourtière", Category = "Meat/Poultry", UnitPrice = 7, UnitsInStock = 21 },
                    new Product { ProductID = 55, ProductName = "Pâté chinois", Category = "Meat/Poultry", UnitPrice = 24, UnitsInStock = 115 },
                    new Product { ProductID = 56, ProductName = "Gnocchi di nonna Alice", Category = "Grains/Cereals", UnitPrice = 38, UnitsInStock = 21 },
                    new Product { ProductID = 57, ProductName = "Ravioli Angelo", Category = "Grains/Cereals", UnitPrice = 19, UnitsInStock = 36 },
                    new Product { ProductID = 58, ProductName = "Escargots de Bourgogne", Category = "Seafood", UnitPrice = 13, UnitsInStock = 62 },
                    new Product { ProductID = 59, ProductName = "Raclette Courdavault", Category = "Dairy Products", UnitPrice = 55, UnitsInStock = 79 },
                    new Product { ProductID = 60, ProductName = "Camembert Pierrot", Category = "Dairy Products", UnitPrice = 34, UnitsInStock = 19 },
                    new Product { ProductID = 61, ProductName = "Sirop d'érable", Category = "Condiments", UnitPrice = 28, UnitsInStock = 113 },
                    new Product { ProductID = 64, ProductName = "Wimmers gute Semmelknödel", Category = "Grains/Cereals", UnitPrice = 33, UnitsInStock = 22 },
                    new Product { ProductID = 66, ProductName = "Louisiana Hot Spiced Okra", Category = "Condiments", UnitPrice = 17, UnitsInStock = 4 },
                    new Product { ProductID = 67, ProductName = "Laughing Lumberjack Lager", Category = "Beverages", UnitPrice = 14, UnitsInStock = 52 },
                    new Product { ProductID = 68, ProductName = "Scottish Longbreads", Category = "Confections", UnitPrice = 12, UnitsInStock = 6 },
                    new Product { ProductID = 69, ProductName = "Gudbrandsdalsost", Category = "Dairy Products", UnitPrice = 36, UnitsInStock = 26 },
                    new Product { ProductID = 70, ProductName = "Outback Lager", Category = "Beverages", UnitPrice = 15, UnitsInStock = 15 },
                    new Product { ProductID = 71, ProductName = "Flotemysost", Category = "Dairy Products", UnitPrice = 21, UnitsInStock = 26 },
                    new Product { ProductID = 73, ProductName = "Röd Kaviar", Category = "Seafood", UnitPrice = 15, UnitsInStock = 101 },
                    new Product { ProductID = 74, ProductName = "Longlife Tofu", Category = "Produce", UnitPrice = 10, UnitsInStock = 4 },
                    new Product { ProductID = 76, ProductName = "Lakkalikööri", Category = "Beverages", UnitPrice = 18, UnitsInStock = 57 },
                    new Product { ProductID = 77, ProductName = "Original Frankfurter grüne Soße", Category = "Condiments", UnitPrice = 13, UnitsInStock = 32 }
                };
            /*
            // Customer/Order data read into memory from XML file using XLinq:
            customerList = (
                from e in XDocument.Load("Customers.xml").
                          Root.Elements("customer")
                select new Customer
                {
                    CustomerID = (string)e.Element("id"),
                    CompanyName = (string)e.Element("name"),
                    Address = (string)e.Element("address"),
                    City = (string)e.Element("city"),
                    Region = (string)e.Element("region"),
                    PostalCode = (string)e.Element("postalcode"),
                    Country = (string)e.Element("country"),
                    Phone = (string)e.Element("phone"),
                    Fax = (string)e.Element("fax"),
                    Orders = (
                        from o in e.Elements("orders").Elements("order")
                        select new Order
                        {
                            OrderID = (int)o.Element("id"),
                            OrderDate = (DateTime)o.Element("orderdate"),
                            Total = (decimal)o.Element("total")
                        })
                        .ToArray()
                })
                .ToList();
             */
        }
    }
}
