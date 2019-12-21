namespace _Sell.Action
{
    public interface IGridAction
    {
        string MainText { get; }
        string SubText { get; }

        void HandleClick();
    }
}