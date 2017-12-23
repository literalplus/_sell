using System.Windows;
using System.Windows.Input;

namespace _Sell.Action
{
    public class GenericGridAction : IGridAction
    {
        public const Key NoHotKey = Key.F24;
        private readonly System.Action _clickAction;

        public GenericGridAction(System.Action clickAction, string mainText, string subText = "", Key hotKey = NoHotKey)
        {
            _clickAction = clickAction;
            HotKey = hotKey;
            MainText = mainText;
            SubText = subText;
        }

        public Key HotKey { get; private set; }
        public string MainText { get; private set; }
        public string SubText { get; private set; }

        public void HandleClick(object sender, RoutedEventArgs args)
        {
            _clickAction.Invoke();
        }

        public bool HasHotKey()
        {
            return HotKey != NoHotKey;
        }
    }
}