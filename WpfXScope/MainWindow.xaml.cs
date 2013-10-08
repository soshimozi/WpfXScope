using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;
using WpfXScope.Device;
using WpfXScope.Models;
using WpfXScope.Properties;
using WpfXScope.ViewModels;

namespace WpfXScope
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.Save();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this) as HwndSource;
            if (source != null) source.AddHook(WndProc);
        }

        ///  <summary>
        ///  Overrides WndProc to enable checking for and handling
        ///  WM_DEVICECHANGE messages.
        ///  </summary>
        /// <param name="hwnd"> </param>
        /// <param name="msg"> </param>
        /// <param name="wParam"> </param>
        /// <param name="lParam"> </param>
        /// <param name="handled"> </param> 
        ///  
        protected IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // The OnDeviceChange routine processes WM_DEVICECHANGE messages.
            if (msg == DeviceManagement.WmDevicechange)
            {
                //var viewmodel = DataContext as MainWindowViewModel;
                //if (viewmodel != null) viewmodel.OnDeviceChange(wParam, lParam);
                //DeviceModel.ForceDisconnect();

                DeviceModel.ProcessDeviceMessage(wParam.ToInt32(), lParam.ToInt32());
            }

            return IntPtr.Zero;
        }

        private void ModernWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var windowHandle = new WindowInteropHelper(this).Handle;

            var notificationHandle = IntPtr.Zero;
            DeviceModel.RegisterForDeviceNotifications(windowHandle, ref notificationHandle);
        }
    }
}
