using System.Windows.Controls;
using WpfXScope.ViewModels;

namespace WpfXScope.Pages
{
    /// <summary>
    /// Interaction logic for DeviceSettingsTrigger.xaml
    /// </summary>
    public partial class DeviceSettingsTrigger : UserControl
    {
        public DeviceSettingsTrigger()
        {
            InitializeComponent();

            DataContext = new DeviceSettingsViewModel();
        }
    }
}
