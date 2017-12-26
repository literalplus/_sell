using System;
using System.Windows;
using System.Windows.Input;
using _Sell.Model;

namespace _Sell.Action
{
    public class NextPageGridAction : IGridAction
    {
        private readonly ProductGrid _grid;
        public NextPageGridAction(ProductGrid grid)
        {
            _grid = grid;
            MainText = "->";
            SubText = "Mehr...";
        }

        public string MainText { get; private set; }
        public string SubText { get; private set; }

        public void HandleClick()
        {
            _grid.ToNextPage();
        }
    }
}