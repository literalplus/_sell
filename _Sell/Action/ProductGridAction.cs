using _Sell.Model;
using System;

namespace _Sell.Action
{
    public class ProductGridAction : IGridAction
    {
        private readonly Action<Product> _clickAction;

        public ProductGridAction(Product product, Action<Product> clickAction)
        {
            _clickAction = clickAction;
            Product = product;
            MainText = product.DisplayName;
            SubText = product.Price.ToString();
        }

        public Product Product { get; private set; }
        public string MainText { get; private set; }
        public string SubText { get; private set; }

        public void HandleClick()
        {
            _clickAction.Invoke(Product);
        }
    }
}