using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using WpfXScope.Device;
using WpfXScope.Models;
using WpfXScope.Properties;
using WpfXScope.ViewModels;
using FirstFloor.ModernUI.Windows.Controls;

namespace WpfXScope
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            var context = new MainWindowViewModel();

            DataContext = context;

            context.UpdateScopeView += ContextOnUpdateScopeView;
        }

        private void ContextOnUpdateScopeView(object sender, UpdateScopeViewEventArgs eventArgs)
        {
            UpdateScope(eventArgs.Force);
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
            if (msg == DeviceManagement.WM_DEVICECHANGE)
            {
                var viewmodel = DataContext as MainWindowViewModel;
                if (viewmodel != null) viewmodel.OnDeviceChange(wParam, lParam);
            }

            return IntPtr.Zero;
        }

        private long _lastTick = 0;
        private void UpdateScope(bool force)
        {
            //// don't update faster than every 50 miliseconds
            //if (!force && DateTime.Now.Ticks - _lastTick <= (TimeSpan.TicksPerMillisecond*62)) return;

            //Dispatcher.Invoke(new Action(ScopeControl.Redraw), DispatcherPriority.Normal,
            //                           new object[] {});

            ////ScopeControl.InvalidateVisual();
            //_lastTick = DateTime.Now.Ticks;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewmodel = DataContext as MainWindowViewModel;
            if (viewmodel == null) return;

            var windowHandle = new WindowInteropHelper(this).Handle;

            var notificationHandle = IntPtr.Zero;
            viewmodel.RegisterForDeviceNotifications(windowHandle, ref notificationHandle);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.Save();
        }

    }
}
