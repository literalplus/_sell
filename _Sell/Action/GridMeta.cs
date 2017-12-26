namespace _Sell.Action
{
    public class GridMeta
    {
        public readonly int ColumnCount;
        public readonly int RowCount;
        public readonly GridHotKeys HotKeys;

        public GridMeta(int columnCount, int rowCount, GridHotKeys hotKeys)
        {
            ColumnCount = columnCount;
            RowCount = rowCount;
            HotKeys = hotKeys;
        }

        public int Size
        {
            get { return ColumnCount * RowCount; }
        }

        public int ToIndex(int row, int column)
        {
            return row * ColumnCount + column;
        }
    }
}