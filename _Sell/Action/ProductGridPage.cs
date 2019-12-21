using Optional;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace _Sell.Action
{
    public class ProductGridPage
    {
        //private IGridAction[][] _actions = {new IGridAction[3], new IGridAction[3], new IGridAction[3], new IGridAction[3]};
        private readonly GridMeta _meta;

        private readonly List<IGridAction> _actions = new List<IGridAction>();
        private readonly List<UIElement> _elements = new List<UIElement>();

        public ProductGridPage(GridMeta meta)
        {
            _meta = meta;
        }

        public int Count
        {
            get { return _actions.Count; }
        }

        public void ApplyToGrid(Grid grid)
        {
            if (_elements.Count != _actions.Count)
            {
                Render();
            }
            grid.Children.Clear();
            _elements.ForEach(el => grid.Children.Add(el));
        }

        private void Render()
        {
            _elements.Clear();
            for (var i = 0; i < _actions.Count; i++)
            {
                var row = i / _meta.ColumnCount;
                var column = i % _meta.ColumnCount;
                var action = _actions[i];
                var button = RenderButton(action, _meta.HotKeys.KeyForIndex(i));
                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);
                _elements.Add(button);
            }
        }

        private Button RenderButton(IGridAction action, Option<Key> hotKey)
        {
            var button = new Button();
            var textBlock = new TextBlock
            {
                Text = action.MainText + "\n\n"
            };
            textBlock.Inlines.Add(new TextBlock
            {
                Text = action.SubText,
                FontWeight = FontWeights.Bold
            });
            hotKey.MatchSome(key => textBlock.Inlines.Add(new TextBlock
            {
                Text = "  " + key,
                FontStyle = FontStyles.Italic
            }));
            button.Content = textBlock;
            button.Click += (o, a) => action.HandleClick();
            button.Template = Application.Current.Resources["tplCubeButton"] as ControlTemplate;
            button.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xA0, 0x00));
            return button;
        }

        public bool HandleKeyPress(Key key)
        {
            var indexForKey = _meta.HotKeys.IndexForKey(key);
            indexForKey.MatchSome(index => _actions[index].HandleClick());
            return indexForKey.HasValue;
        }

        public int GetSpaceLeft()
        {
            return _meta.Size - Count;
        }

        public bool HasSpace()
        {
            return GetSpaceLeft() > 0;
        }

        public void Add(IGridAction action)
        {
            if (!HasSpace())
            {
                throw new InvalidOperationException("no space left");
            }
            _actions.Add(action);
        }
    }
}