using _Sell.Model;
using System.Collections.Generic;

namespace _Sell.Service
{
    public interface IProductRegistry
    {
        Product GetProduct(int id);
        ICollection<Product> Products { get; }
        bool HasProduct(int id);
    }
}