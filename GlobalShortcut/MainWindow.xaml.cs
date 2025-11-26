using DevWinUI;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.System;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GlobalShortcut
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private static int _clicks; // count of primary button presses

        public MainWindow()
        {
            InitializeComponent();
            MainShortcut.Keys = [new KeyVisualInfo { Key = VirtualKey.F6, KeyName = "F6" }];

            // Set up window message monitor
            WindowMessageMonitor monitor = new(this);
            monitor.WindowMessageReceived += OnWindowMessageReceived;
        }

        private void OnWindowMessageReceived(object? sender, WindowMessageEventArgs e)
        {
            if (e.Message.MessageId == 0x0312) // WM_HOTKEY event
            {
                _clicks += 1;
                PressesTextBlock.Text = "Number of hotkey presses: " + _clicks;
            }
        }

        private void OnMainShortcutPrimaryButtonClick(object sender, ContentDialogButtonClickEventArgs e)
        {
            MainShortcut.UpdatePreviewKeys();
            MainShortcut.CloseContentDialog();

            var keyInfos = MainShortcut.Keys.Cast<KeyVisualInfo>();

            Debug.WriteLine("New hotkey saved: " + string.Join(" + ", MainShortcut.Keys));

            VirtualKeyModifiers modifiers = VirtualKeyModifiers.None;
            VirtualKey triggerKey = VirtualKey.None;

            foreach (var item in keyInfos)
            {
                if (item.Key.HasValue)
                {
                    var modFlag = GetModifierKey(item.Key.Value);

                    if (modFlag != VirtualKeyModifiers.None)
                    {
                        modifiers |= modFlag;
                    }
                    else
                    {
                        triggerKey = item.Key.Value;
                    }
                }
            }

            if (triggerKey != VirtualKey.None)
            {
                // Get window handle
                HWND hWnd = new(WindowNative.GetWindowHandle(this));

                // Register the hotkey
                _ = PInvoke.UnregisterHotKey(hWnd, 0x0000);
                _ = PInvoke.RegisterHotKey(hWnd, 0x0000, HOT_KEY_MODIFIERS.MOD_NOREPEAT | (HOT_KEY_MODIFIERS)modifiers, (uint)triggerKey);
            }
        }

        private void OnMainShortcutSecondaryButtonClick(object sender, ContentDialogButtonClickEventArgs e)
        {
            // "Secondary button clicked!";
        }

        private void OnMainShortcutCloseButtonClick(object sender, ContentDialogButtonClickEventArgs e)
        {
            // "Close button clicked!";
        }

        private static VirtualKeyModifiers GetModifierKey(VirtualKey key) => key switch
        {
            VirtualKey.Control or VirtualKey.LeftControl or VirtualKey.RightControl => VirtualKeyModifiers.Control,
            VirtualKey.Menu or VirtualKey.LeftMenu or VirtualKey.RightMenu => VirtualKeyModifiers.Menu,
            VirtualKey.Shift or VirtualKey.LeftShift or VirtualKey.RightShift => VirtualKeyModifiers.Shift,
            VirtualKey.LeftWindows or VirtualKey.RightWindows => VirtualKeyModifiers.Windows,
            _ => VirtualKeyModifiers.None,
        };
    }
}
