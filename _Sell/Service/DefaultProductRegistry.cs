using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using _Sell.Model;

namespace _Sell.Service
{
    public class DefaultProductRegistry : IProductRegistry
    {
        private static readonly List<Product> DefaultProducts = new List<Product>()
        {
            new Product(1, "Gulaschsuppe", new Price(3, 80)),
            new Product(2, "Heißer Löwe", new Price(3, 50)),
            new Product(3, "Kinderpunsch", new Price(2, 50)),
            new Product(4, "Löwe w/ Brüll", new Price(4, 50)),
            new Product(5, "Aufstrichbrot", new Price(1, 50)),
            new Product(6, "Softdrinks 0,5l", new Price(1, 50)),
            new Product(7, "Speckstangerl", new Price(2, 50)),
            new Product(8, "Autofahrerpunsch", new Price(3, 0), "Autofahrer-\npunsch"),
            new Product(9, "Glühwein", new Price(3, 50)),
            new Product(10, "Eierlikör", new Price(2, 50)),
            new Product(11, "Prosecco", new Price(3, 0)),
            new Product(12, "Tee", new Price(1, 50)),
            new Product(13, "Bier", new Price(2, 50)),
            new Product(14, "1/8 Wein", new Price(2, 20)),
            new Product(15, "Stamperl Schnaps", new Price(2, 50))
        };

        private readonly Dictionary<int, Product> _productsDictionary = new Dictionary<int, Product>();

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