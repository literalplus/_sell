using System;
using System.Windows;
using System.Windows.Input;
using _Sell.Model;

namespace _Sell.Action
{
    public class ProductGridAction : IGridAction
    {
        public const Key NoHotKey = Key.F24;
        private readonly Action<Product> _clickAction;

        public ProductGridAction(Product product, Action<Product> clickAction, Key hotKey = NoHotKey)
        {
            _clickAction = clickAction;
            Product = product;
            HotKey = hotKey;
            MainText = product.DisplayName;
            SubText = product.Price.ToString();
        }

        public Product Product { get; private set; }
        public Key HotKey { get; private set; }
        public string MainText { get; private set; }
        public string SubText { get; private set; }

        public void HandleClick(object sender, RoutedEventArgs args)
        {
            _clickAction.Invoke(Product);
        }

        public bool HasHotKey()
        {
            return HotKey != NoHotKey;
        }
    }
}