using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using _Sell.Model;

namespace _Sell.Service
{
    public interface IProductRegistry
    {
        Product GetProduct(int id);
        ICollection<Product> Products { get; }
        bool HasProduct(int id);
    }
}