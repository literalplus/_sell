using _Sell.Model;
using System.Collections.Generic;

namespace _Sell.Service
{
    public class DefaultProductRegistry : IProductRegistry
    {
        private readonly Dictionary<int, Product> _productsDictionary = new Dictionary<int, Product>();

        private static readonly List<Product> DefaultProducts = new List<Product>()
        {
            new Product(1, "Heißer Löwe", new Price(4, 0)),
            new Product(2, "Kinderpunsch", new Price(3, 0)),
            new Product(3, "Autofahrerpunsch", new Price(3, 50), "Autofahrer-\npunsch"),
            new Product(4, "Löwe w/ Brüll", new Price(5, 0)),
            new Product(5, "Glühwein", new Price(3, 50)),
            
            new Product(10, "Softdrinks 0,5l", new Price(2, 50)),
            new Product(11, "Wasser Fl.", new Price(2, 0)),
            new Product(12, "Bier 1/2", new Price(3, 0)),
            new Product(13, "Bier 1/3", new Price(2, 0)),
            
            new Product(20, "Prosecco", new Price(4, 0)),
            new Product(21, "Spritzer 1/4", new Price(2, 0)),
            new Product(22, "Stamperl Schnaps", new Price(2, 0), "Stamperl\nSchnaps"),
            new Product(23, "Tee", new Price(1, 50)),
            //new Product(18, "Red Bull", new Price(3, 0))
        };

        public DefaultProductRegistry()
        {
            DefaultProducts.ForEach(p => _productsDictionary.Add(p.Id, p));
        }

        public ICollection<Product> Products
        {
            get { return DefaultProducts; }
        }

        public Product GetProduct(int id)
        {
            return _productsDictionary[id];
        }

        public bool HasProduct(int id)
        {
            return _productsDictionary.ContainsKey(id);
        }
    }
}