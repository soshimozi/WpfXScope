using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfXScope.Models;

namespace WpfXScope.ViewModels
{
    public class HomeViewModel : NotifyObject
    {
        private readonly ICommand _connectCommand;
        private readonly ICommand _stopCommand;
        private readonly ICommand _autoCommand;

        private ObservableCollection<Byte> _scopeData;

        public HomeViewModel()
        {

            ScopeData = new ObservableCollection<byte>();
            DeviceModel.BulkDataArrived += DeviceModelOnBulkDataArrived;
            DeviceModel.DeviceAttached += DeviceModelOnDeviceAttached;
            DeviceModel.DeviceDetached += DeviceModelOnDeviceDetached;
            DeviceSettingsManager.SettingsChanged += DeviceSettingsModelOnSettingsChanged;

            _connectCommand = new RoutableCommand(a => ConnectClicked(), o => !IsConnected);
            _autoCommand = new RoutableCommand(a => AutoClicked(), o => IsConnected);

            if(Properties.Settings.Default.AutoConnect)
                Connect();
        }

        private void AutoClicked()
        {
            DeviceSettingsManager.Auto();
        }

        private void ConnectClicked()
        {
            Connect();
        }

        private void DeviceModelOnDeviceDetached(object sender, EventArgs eventArgs)
        {
            DeviceModel.ForceDisconnect();


            OnPropertyChanged("IsConnected");

            var stopCommand = _stopCommand as RoutableCommand;
            if (stopCommand != null) stopCommand.RaiseCanExecuteChanged();

            var autoCommand = _autoCommand as RoutableCommand;
            if (autoCommand != null) autoCommand.RaiseCanExecuteChanged();

            var connectCommand = _connectCommand as RoutableCommand;
            if (connectCommand != null) connectCommand.RaiseCanExecuteChanged();

            //OnPropertyChanged("StopCommand");
            //OnPropertyChanged("AutoCommand");
            //OnPropertyChanged("ConnectCommand");
        }

        private void DeviceModelOnDeviceAttached(object sender, EventArgs eventArgs)
        {
            if(!DeviceModel.DeviceDetected)
            {
                Connect();
            }
        }

        private void DeviceSettingsModelOnSettingsChanged(object sender, SettingsChangedEventArgs settingsChangedEventArgs)
        {
            OnPropertyChanged(settingsChangedEventArgs.Name);
            if(settingsChangedEventArgs.Name == "IsStopped")
                OnPropertyChanged("IsScopeStopped");
        }


        #region Commands
        public ICommand ConnectCommand
        {
            get { return _connectCommand; }
        }

        public ICommand AutoCommand
        {
            get { return _autoCommand;  }
        }
        #endregion

        public bool IsScopeStopped
        {
            get { return DeviceSettingsManager.IsStopped; }
            set
            {
                DeviceSettingsManager.IsStopped = value;   
                OnPropertyChanged("IsStopped");
            }
        }

        public bool IsConnected
        {
            get { return DeviceModel.DeviceDetected; }
        }

        public int HorizontalFrequency
        {
            get { return DeviceSettingsManager.HorizontalFrequency; }
        }

        public int Channel1Offset
        {
            get { return DeviceSettingsManager.Channel1Offset;  }
        }

        public int Channel2Offset
        {
            get { return DeviceSettingsManager.Channel2Offset; }
        }


        public int Channel1Gain
        {
            get { return DeviceSettingsManager.Channel1Gain; }
        }

        public int Channel2Gain
        {
            get { return DeviceSettingsManager.Channel2Gain; }
        }

        public bool Channel1On
        {
            get { return DeviceSettingsManager.Channel1On; }
        }

        public bool Channel2On
        {
            get { return DeviceSettingsManager.Channel2On; }
        }

        public bool ChannelDOn
        {
            get { return DeviceSettingsManager.ChannelDOn; }
        }

        public ObservableCollection<byte> ScopeData
        {
            get { return _scopeData; }
            set
            {
                SetField(ref _scopeData, value, "ScopeData");
            }
        }

        public int HorizontalPosition
        {
            get { return DeviceSettingsManager.HorizontalPosition; }
            set { DeviceSettingsManager.HorizontalPosition = (ushort)value; }
        }

        public void Connect()
        {
            var version = 0;
            DeviceModel.USBConnect(ref version);
            if (!DeviceModel.DeviceDetected) return;

            OnPropertyChanged("IsConnected");

            var stopCommand = _stopCommand as RoutableCommand;
            if (stopCommand != null) stopCommand.RaiseCanExecuteChanged();

            var autoCommand = _autoCommand as RoutableCommand;
            if (autoCommand != null) autoCommand.RaiseCanExecuteChanged();

            var connectCommand = _connectCommand as RoutableCommand;
            if (connectCommand != null) connectCommand.RaiseCanExecuteChanged(); 
            
            // then queue call to read bulk data
            DeviceModel.ReadBulkData();
        }

        private void DeviceModelOnBulkDataArrived(object sender, BulkDataArrivedArgs bulkDataArrivedArgs)
        {
            // have some data
            // copy it over and check if oneshot flag is not set
            // if not, then requeue another call
            ScopeData = new ObservableCollection<byte>(bulkDataArrivedArgs.Data);
            DeviceModel.ReadBulkData();
        }

    }

}
