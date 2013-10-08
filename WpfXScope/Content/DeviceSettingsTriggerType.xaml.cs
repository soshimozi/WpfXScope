using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfXScope.ViewModels;

namespace WpfXScope.Content
{
    /// <summary>
    /// Interaction logic for DeviceSettingsTriggerType.xaml
    /// </summary>
    public partial class DeviceSettingsTriggerType : UserControl
    {
        public DeviceSettingsTriggerType()
        {
            InitializeComponent();
            this.DataContext = new DeviceSettingsViewModel();
        }
    }
}
