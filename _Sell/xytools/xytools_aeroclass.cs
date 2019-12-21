using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace xytools
{

    public static class AeroTools
    {
        public static void RefreshAero(Window Win, int right = -1, int bottom = -1)
        {
            try
            {
                // Obtain the window handle for WPF application
                IntPtr mainWindowPtr = new WindowInteropHelper(Win).Handle;
                HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

                // Get System Dpi
                System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(mainWindowPtr);
                float DesktopDpiX = desktop.DpiX;
                float DesktopDpiY = desktop.DpiY;

                // Set Margins
                //MARGINS margins = new MARGINS();

                // Extend glass frame into client area
                // Note that the default desktop Dpi is 96dpi. The  margins are
                // adjusted for the system Dpi.

                if (bottom == -1) bottom = ((int)Win.ActualHeight) + 12;
                if (right == -1) right = ((int)Win.ActualWidth) + 12;

                int cxLeftWidth = Convert.ToInt32(2 * (DesktopDpiX / 96));
                int cxRightWidth = Convert.ToInt32(right * (DesktopDpiX / 96));
                int cyTopHeight = Convert.ToInt32(2 * (DesktopDpiX / 96));
                int cyBottomHeight = Convert.ToInt32(bottom * (DesktopDpiX / 96));

                DwmApi.MARGINS margins = new DwmApi.MARGINS(cxLeftWidth, cyTopHeight, cxRightWidth, cyBottomHeight);

                DwmApi.DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, margins);
            }
            // If not Vista, paint background white.
            catch (DllNotFoundException)
            {
                Application.Current.MainWindow.Background = Brushes.White;
            }
            catch (ArgumentException)
            {
                //Window closed
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int cxLeftWidth;      // width of left border that retains its size
        public int cxRightWidth;     // width of right border that retains its size
        public int cyTopHeight;      // height of top border that retains its size
        public int cyBottomHeight;   // height of bottom border that retains its size
    };
    internal class DwmApi
    {
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableBlurBehindWindow(
            IntPtr hWnd, DWM_BLURBEHIND pBlurBehind);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(
            IntPtr hWnd, MARGINS pMargins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableComposition(bool bEnable);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmGetColorizationColor(
            out int pcrColorization,
            [MarshalAs(UnmanagedType.Bool)]out bool pfOpaqueBlend);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern IntPtr DwmRegisterThumbnail(
            IntPtr dest, IntPtr source);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmUnregisterThumbnail(IntPtr hThumbnail);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmUpdateThumbnailProperties(
            IntPtr hThumbnail, DWM_THUMBNAIL_PROPERTIES props);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmQueryThumbnailSourceSize(
            IntPtr hThumbnail, out Size size);

        [StructLayout(LayoutKind.Sequential)]
        public class DWM_THUMBNAIL_PROPERTIES
        {
            public uint dwFlags;
            public RECT rcDestination;
            public RECT rcSource;
            public byte opacity;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fVisible;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fSourceClientAreaOnly;
            public const uint DWM_TNP_RECTDESTINATION = 0x00000001;
            public const uint DWM_TNP_RECTSOURCE = 0x00000002;
            public const uint DWM_TNP_OPACITY = 0x00000004;
            public const uint DWM_TNP_VISIBLE = 0x00000008;
            public const uint DWM_TNP_SOURCECLIENTAREAONLY = 0x00000010;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MARGINS
        {
            public int cxLeftWidth, cxRightWidth,
                       cyTopHeight, cyBottomHeight;

            public MARGINS(int left, int top, int right, int bottom)
            {
                cxLeftWidth = left; cyTopHeight = top;
                cxRightWidth = right; cyBottomHeight = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class DWM_BLURBEHIND
        {
            public uint dwFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fEnable;
            public IntPtr hRegionBlur;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fTransitionOnMaximized;

            public const uint DWM_BB_ENABLE = 0x00000001;
            public const uint DWM_BB_BLURREGION = 0x00000002;
            public const uint DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, right, bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left; this.top = top;
                this.right = right; this.bottom = bottom;
            }
        }
    }
}
