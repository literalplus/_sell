using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using xytools;

namespace _Hotkey
{
    public class HotkeyManager
    {
        private IntPtr _windowHandle;
        public IntPtr WindowHandle
        {
            get
            {
                return _windowHandle;
            }
        }
        private string identifier;
        private int nextAtomId = 0;
        private Dictionary<IntPtr, RegisteredHotkey> hotkeys = new Dictionary<IntPtr, RegisteredHotkey>();

        public HotkeyManager(Window window, string identifier)
        {
            WindowInteropHelper wih = new WindowInteropHelper(window);
            this._windowHandle = wih.Handle;
            HwndSource hWndSource = HwndSource.FromHwnd(WindowHandle);
            hWndSource.AddHook(HotkeyProc);
            this.identifier = identifier;
        }

        public RegisteredHotkey registerHotkey(uint keyCode, uint modifiers)
        {
            RegisteredHotkey hotkey = new RegisteredHotkey(this, keyCode, modifiers);
            hotkeys.Add(hotkey.Atom, hotkey);
            return hotkey;
        }

        public short getNextAtom()
        {
            short atom = Win32.GlobalAddAtom(identifier + nextAtomId++);
            if (atom == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return atom;
        }

        /// ------- //////////////////////////////////////////////////////////////////////////////////////

        private IntPtr HotkeyProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WM_HOTKEY:
                    D.W("Hotkey pressed: " + wParam + "; " + msg);
                    if (hotkeys.ContainsKey(wParam))
                    {
                        RegisteredHotkey hotkey;
                        hotkeys.TryGetValue(wParam, out hotkey);
                        hotkey.press();
                    }

                    handled = true;
                    break;
            }

            return IntPtr.Zero;
        }

        /// ------- //////////////////////////////////////////////////////////////////////////////////////
    }

    public delegate void HotkeyHandler(RegisteredHotkey hotkey);

    public class RegisteredHotkey
    {
        private short _atom;
        private HotkeyManager hotkeyManager;
        private uint keyCode;
        private uint modifiers;
        public IntPtr Atom
        {
            get
            {
                return (IntPtr)_atom;
            }
        }
        public uint KeyCode
        {
            get
            {
                return keyCode;
            }
        }
        public event HotkeyHandler onPress;

        public RegisteredHotkey(HotkeyManager hotkeyManager, uint keyCode, uint modifiers)
        {
            _atom = hotkeyManager.getNextAtom();
            this.hotkeyManager = hotkeyManager;
            this.keyCode = keyCode;
            this.modifiers = modifiers;

            Win32.tryRegisterHotkey(hotkeyManager.WindowHandle, _atom, modifiers, keyCode);
        }

        public void press()
        {
            onPress(this);
        }
    }
}
