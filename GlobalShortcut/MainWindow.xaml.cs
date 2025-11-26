using DevWinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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

            // Get window handle
            HWND hWnd = new(WindowNative.GetWindowHandle(this));

            // Set up window message monitor
            WindowMessageMonitor monitor = new(this);
            monitor.WindowMessageReceived += OnWindowMessageReceived;

            PInvoke.RegisterHotKey(hWnd, 0x0000, HOT_KEY_MODIFIERS.MOD_NOREPEAT, 0x75); // F6
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
        }

        private void OnMainShortcutSecondaryButtonClick(object sender, ContentDialogButtonClickEventArgs e)
        {
            // "Secondary button clicked!";
        }

        private void OnMainShortcutCloseButtonClick(object sender, ContentDialogButtonClickEventArgs e)
        {
            // "Close button clicked!";
        }
    }
}
