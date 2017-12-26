using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Optional;
using Optional.Collections;

namespace _Sell.Action
{
    public class ProductGrid
    {
        public static readonly GridMeta DefaultGridMeta = new GridMeta(3, 4, GridHotKeys.ThreeByFour);
        public readonly GridMeta Meta;
        private readonly Grid _grid;
        private readonly List<ProductGridPage> _pages = new List<ProductGridPage>();
        private int _currentPageIndex;

        public ProductGrid(GridMeta meta, Grid grid)
        {
            Meta = meta;
            _grid = grid;
            CreateNewPage();
        }

        public int PageCount
        {
            get { return _pages.Count; }
        }

        public ProductGridPage CurrentPage
        {
            get { return _pages[_currentPageIndex]; }
        }

        private int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                _currentPageIndex = value;
                CurrentPage.ApplyToGrid(_grid);
            }
        }

        public void Add(IGridAction action)
        {
            // must always exist - initial page always present, Add() creates new page on overflow:
            var firstWithSpace = _pages.First(el => el.HasSpace());
            if (firstWithSpace.GetSpaceLeft() <= 1)
            {
                firstWithSpace.Add(new NextPageGridAction(this));
                firstWithSpace = CreateNewPage();
            }
            firstWithSpace.Add(action);
            CurrentPage.ApplyToGrid(_grid);
        }

        private ProductGridPage CreateNewPage()
        {
            var created = new ProductGridPage(Meta);
            _pages.Add(created);
            return created;
        }

        public void ToNextPage()
        {
            var nextIndex = CurrentPageIndex + 1;
            if (nextIndex < 0 || nextIndex >= PageCount)
            {
                CurrentPageIndex = 0;
            }
            else
            {
                CurrentPageIndex = nextIndex;
            }
        }

        public void HandleKeyPress(Key key)
        {
            CurrentPage.HandleKeyPress(key);
        }
    }
}