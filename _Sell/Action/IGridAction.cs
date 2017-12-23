using System.Windows;
using System.Windows.Input;

namespace _Sell.Action
{
    public interface IGridAction
    {
        Key HotKey { get; }
        string MainText { get; }
        string SubText { get; }

        void HandleClick(object sender, RoutedEventArgs args);
        bool HasHotKey();
    }
}