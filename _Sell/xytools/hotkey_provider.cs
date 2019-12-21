using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace _Hotkey
{
    internal static class Win32
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalAddAtom(string lpString);

        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalDeleteAtom(short nAtom);

        public static uint GetNumpadKeyCode(int number)
        {
            return Convert.ToUInt32(0x60 + number); //0x60 = Numpad 0
        }

        public static int GetNumpadKeyId(uint keyCode)
        {
            return Convert.ToInt32(keyCode) - 0x60; //0x60 = Numpad 0
        }

        public static bool tryRegisterHotkey(IntPtr hWnd, int id, uint fsModifiers, uint keyCode)
        {
            if (!Win32.RegisterHotKey(hWnd, id, fsModifiers, keyCode))
            {
                try
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                catch (Win32Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                return false;
            }
            else return true;
        }

        public const int MOD_ALT = 1;
        public const int MOD_CONTROL = 2;
        public const int MOD_SHIFT = 4;
        public const int MOD_WIN = 8;
        public const int MOD_NOREPEAT = 0x4000;

        //public const uint VK_KEY_C = 0x43;

        public const int WM_HOTKEY = 0x312;
    }
}
