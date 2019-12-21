using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace _Sell
{
    class NumberKeyManager
    {
        private readonly Dictionary<Button, int> ButtonValues = new Dictionary<Button, int>();

        public void RegisterAll(params Button[] buttons)
        {
            if (buttons.Length != 10)
            {
                throw new ArgumentException("need exactly 10 buttons");
            }
            for (int i = 0; i < buttons.Length; i++)
            {
                ButtonValues.Add(buttons[i], i);
            }
        }

        public int GetValueOrNegativeOne(Button source)
        {
            if (!ButtonValues.ContainsKey(source))
            {
                return -1;
            }
            else
            {
                return ButtonValues[source];
            }
        }

        public int GetValueOrNegativeOne(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9)
            {
                return key - Key.D0;
            }
            else if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                return key - Key.NumPad0;
            }
            else
            {
                return -1;
            }
        }
    }
}
