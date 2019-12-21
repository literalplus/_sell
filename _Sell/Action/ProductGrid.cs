using Optional.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

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

        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            private set
            {
                _currentPageIndex = value;
                CurrentPage.ApplyToGrid(_grid);
            }
        }

        public void Add(IGridAction action)
        {
            _pages.FirstOrNone(el => el.HasSpace())
                .ValueOr(CreateNewPage)
                .Add(action);
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
            GoToPageOrDefault(CurrentPageIndex + 1, 0);
        }

        private void GoToPageOrDefault(int nextPage, int defaultPage)
        {
            if (nextPage < 0 || nextPage >= PageCount)
            {
                CurrentPageIndex = defaultPage;
            }
            else
            {
                CurrentPageIndex = nextPage;
            }
        }

        public void ToPreviousPage()
        {
            GoToPageOrDefault(CurrentPageIndex - 1, PageCount - 1);
        }

        public void ToFirstPage()
        {
            GoToPageOrDefault(0, 0);
        }

        public void ToLastPage()
        {
            GoToPageOrDefault(PageCount - 1, PageCount - 1);
        }

        public bool HandleKeyPress(Key key)
        {
            return CurrentPage.HandleKeyPress(key);
        }
    }
}