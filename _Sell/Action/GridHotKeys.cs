using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Optional;
using Optional.Collections;

namespace _Sell.Action
{
    public class GridHotKeys
    {
        public static readonly GridHotKeys ThreeByFour = new GridHotKeys(new[]
        {
            Key.Q, Key.W, Key.E,
            Key.A, Key.S, Key.D,
            Key.Y, Key.X, Key.C,
            Key.R, Key.F, Key.V
        });

        private readonly Key[] _keys;
        private readonly IDictionary<Key, int> _keyIndices;

        public GridHotKeys(Key[] keys)
        {
            _keys = keys;
            _keyIndices = _keys.Select((key, index) => new {key, index})
                .ToDictionary(t => t.key, t => t.index);
        }

        public int Length
        {
            get { return _keys.Length; }
        }

        public ICollection<Key> Keys
        {
            get { return _keys.ToList(); }
        }

        public Option<Key> KeyForIndex(int index)
        {
            return _keys.ElementAtOrNone(index);
        }

        public Option<int> IndexForKey(Key key)
        {
            return _keyIndices.GetValueOrNone(key);
        }
    }
}