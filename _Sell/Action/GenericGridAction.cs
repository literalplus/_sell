namespace _Sell.Action
{
    public class GenericGridAction : IGridAction
    {
        private readonly System.Action _clickAction;

        public GenericGridAction(System.Action clickAction, string mainText, string subText = "")
        {
            _clickAction = clickAction;
            MainText = mainText;
            SubText = subText;
        }

        public string MainText { get; private set; }
        public string SubText { get; private set; }

        public void HandleClick()
        {
            _clickAction.Invoke();
        }
    }
}