using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using WpfXScope.Device;
using WpfXScope.Models;

namespace WpfXScope.ViewModels
{
    public class MainWindowViewModel : NotifyObject
    {
        #region Private Fields
        private readonly ICommand _connectCommand;
        private readonly ICommand _forceCommand;
        private readonly ICommand _stopCommand;

        //private readonly DeviceModel _deviceModel;
        private int _channel1Pos;
        private bool _channel1Math;
        private bool _channel1Avrg;
        private bool _channel1Inv;
        private bool _channel1On;
        private int _samplingRate;
        private Brush _gridLineBrush;
        private ObservableCollection<Byte> _scopeData;
        private ChannelMathType _channel1MathType;
        private Brush _channel1Brush;
        private int _channel1Gain;

        private const int ChannelBottomPosition = -126;
        private Color _channel1Color;
        private TriggerMode _channelTriggerMode;
        private TriggerType _channelTriggerType;
        private bool _protocolIsCircular;
        private byte _channelTrigger;
        private bool _scopeIsStopped;
        private bool _drawScopeLines;

        private const int MCh1Gain = 12;
        private const int MCh1Pos = 29;
        private const int GPIO0SRate = 0;
        private const int GPIO5Trigger = 5;
        private const int GPIO1Ch1Option = 1;
// ReSharper disable InconsistentNaming
        private const int GPIOBMStatus = 11;
// ReSharper restore InconsistentNaming

        #endregion

        #region Events
        public event EventHandler<ConnectedEventArgs> Connected;
        public event EventHandler<UpdateScopeViewEventArgs> UpdateScopeView;
        #endregion

        #region Construction
        public MainWindowViewModel()
        {
            //_deviceModel = deviceModel;

            LineBrush = new SolidColorBrush(Colors.White);
            ScopeData = new ObservableCollection<byte>();

            _connectCommand = new RoutableCommand(a => ConnectClicked(), o => !DeviceModel.DeviceDetected);
            _forceCommand = new RoutableCommand(a => ForceClicked(), o => true);
            _stopCommand = new RoutableCommand(a => StopClicked(), o => DeviceModel.DeviceDetected);

            DeviceModel.BulkDataArrived += DeviceModelOnBulkDataArrived;
            Channel1Color = Colors.Green;

            var triggerSourceList = new List<TriggerSource>
                                        {
                                            new TriggerSource {Display = "CH1", Value = 0},
                                            new TriggerSource {Display = "CH2", Value = 1},
                                            new TriggerSource {Display = "Bit 0", Value = 2},
                                            new TriggerSource {Display = "Bit 1", Value = 3},
                                            new TriggerSource {Display = "Bit 2", Value = 4},
                                            new TriggerSource {Display = "Bit 3", Value = 5},
                                            new TriggerSource {Display = "Bit 4", Value = 6},
                                            new TriggerSource {Display = "Bit 5", Value = 7},
                                            new TriggerSource {Display = "Bit 6", Value = 8},
                                            new TriggerSource {Display = "Bit 7", Value = 9},
                                            new TriggerSource {Display = "EXT", Value = 10}
                                        };

            TriggerSources = new ObservableCollection<TriggerSource>(triggerSourceList);
            OnPropertyChanged("TriggerSources");
        }

        private void StopClicked()
        {
            DeviceModel.ScopeActive(IsScopeStopped);
        }

        private void ForceClicked()
        {
            DeviceModel.ForceTrigger();
        }

        public void RegisterForDeviceNotifications(IntPtr handle, ref IntPtr notificationHandle)
        {
            DeviceModel.RegisterForDeviceNotifications(handle, ref notificationHandle);
        }

        private void DeviceModelOnBulkDataArrived(object sender, BulkDataArrivedArgs bulkDataArrivedArgs)
        {
            // have some data
            // copy it over and check if oneshot flag is not set
            // if not, then requeue another call

            ScopeData = new ObservableCollection<byte>(bulkDataArrivedArgs.Data);
            OnUpdateScopeView();

            DeviceModel.ReadBulkData();
        }

        #endregion

        #region Properties

        #region Commands
        public ICommand ConnectCommand
        {
            get { return _connectCommand;  }
        }

        public ICommand ForceCommand
        {
            get { return _forceCommand;  }
        }

        public ICommand StopCommand
        {
            get { return _stopCommand; }
        }
        #endregion

        public ObservableCollection<TriggerSource> TriggerSources
        {
            get; private set; 
        }

        public bool IsConnected
        {
            get { return DeviceModel.DeviceDetected; }
        }

        public bool DrawScopeLines
        {
            get { return _drawScopeLines;  }
            set { SetField(ref _drawScopeLines, value, "DrawScopeLines"); }
        }

        public byte ChannelTrigger
        {
            get { return _channelTrigger; }
            set
            {
                SetField(ref _channelTrigger, value, "ChannelTrigger");
                DeviceModel.WriteByte(24, value);
            }
        }

        public bool IsProtocolCircular
        {
            get { return _protocolIsCircular; }
            set
            {
                SetField(ref _protocolIsCircular, value, "IsProtocolCircular");
                UpdateTrigger();
            }
        }

        public TriggerType ChannelTriggerType
        {
            get { return _channelTriggerType; }
            set
            {
                SetField(ref _channelTriggerType, value, "ChannelTriggerType");
                UpdateTrigger();

                if(value == TriggerType.Single)
                {
                    // going to single trigger mode so toggle stop
                    IsScopeStopped = true;
                }
            }

        }

        public bool IsScopeStopped
        {
            get { return _scopeIsStopped; }
            set
            {
                SetField(ref _scopeIsStopped, value, "IsScopeStopped");
            }
        }

        public TriggerMode ChannelTriggerMode
        {
            get { return _channelTriggerMode; }
            set
            {
                SetField(ref _channelTriggerMode, value, "ChannelTriggerMode");
                UpdateTrigger();
            }
        }

        public Color Channel1Color
        {
            get { return _channel1Color;  }
            set { SetField(ref _channel1Color, value, "Channel1Color"); Channel1Brush = new SolidColorBrush(value); }
        }

        public Brush Channel1Brush
        {
            get { return _channel1Brush;  }
            set { SetField(ref _channel1Brush, value, "Channel1Brush"); }
        }


        public int Channel1Gain
        {
            get { return _channel1Gain; }
            set
            {
                SetField(ref _channel1Gain, value, "Channel1Gain");
                DeviceModel.WriteByte(12, (byte)value);
            }
        }

        public int Channel1Pos
        {
            get { return _channel1Pos; }
            set
            {
                SetField(ref _channel1Pos, value, "Channel1Pos");
                DeviceModel.WriteByte(29, (byte)(ChannelBottomPosition - value));
            }
        }

        public bool Channel1Math
        {
            get { return _channel1Math; }
            set
            {
                SetField(ref _channel1Math, value, "Channel1Math");
                UpdateControls();
            }
        }

        public bool Channel1Avrg
        {
            get { return _channel1Avrg; }
            set 
            { 
                SetField(ref _channel1Avrg, value, "Channel1Avrg");
                UpdateControls();
            }
        }

        public bool Channel1Inv
        {
            get { return _channel1Inv; }
            set 
            { 
                SetField(ref _channel1Inv, value, "Channel1Inv");
                UpdateControls();
            }
        }

        public ChannelMathType Channel1MathType
        {
            get { return _channel1MathType; }
            set 
            { 
                SetField(ref _channel1MathType, value, "Channel1MathType");
                UpdateControls();
            }
        }

        public bool Channel1On
        {
            get { return _channel1On; }
            set 
            { 
                SetField(ref _channel1On, value, "Channel1On"); 
                UpdateControls();
            }
        }

        public int SamplingRate
        {
            get { return _samplingRate; }
            set
            {
                SetField(ref _samplingRate, value, "SamplingRate");
                DeviceModel.WriteByte(0, (byte)value);
            }
        }

        public Brush LineBrush
        {
            get { return _gridLineBrush; }
            set
            {
                SetField(ref _gridLineBrush, value, "LineBrush");
            }
        }

        public ObservableCollection<byte> ScopeData
        {
            get { return _scopeData; }
            set
            {
                SetField(ref _scopeData, value, "ScopeData");
            }
        }
        #endregion

        #region Private and Protected Methods
        private void UpdateTrigger()
        {
            //byte field = 0;

            //switch (ChannelTriggerType)
            //{
            //    case TriggerType.Normal:
            //        field += 1;
            //        break;

            //    case TriggerType.Single:
            //        field += 2;
            //        break;

            //    case TriggerType.Auto:
            //        field += 4;
            //        break;
            //}

            //switch (ChannelTriggerMode)
            //{
            //    case TriggerMode.RisingEdge:
            //    case TriggerMode.PositiveSlope:
            //        field += 8;
            //        break;

            //    case TriggerMode.FallingEdge:
            //    case TriggerMode.NegativeSlope:
            //        field += 32;
            //        break;

            //    case TriggerMode.Window:
            //        field += 64;
            //        break;
            //}

            //if (ChannelTriggerMode == TriggerMode.RisingEdge || ChannelTriggerMode == TriggerMode.FallingEdge)
            //    field += 128;

            byte field = 0;

            switch (ChannelTriggerType) // Trigger
            {
                case TriggerType.Normal:
                    field += 1;           
                    break;
                case TriggerType.Single:
                    field += 2;
                    break;
                case TriggerType.Auto:
                    field += 4;
                    break;
            }

            if ((ChannelTriggerMode == TriggerMode.FallingEdge) || (ChannelTriggerMode == TriggerMode.NegativeSlope)) field += 8;    // Trigger direction
            if (IsProtocolCircular) field += 16; // Sniffer circular buffer
            if ((ChannelTriggerMode == TriggerMode.PositiveSlope) || ChannelTriggerMode == TriggerMode.NegativeSlope) field += 32;   // Slope
            if (ChannelTriggerMode == TriggerMode.Window) field += 64;   // Window
            if ((ChannelTriggerMode == TriggerMode.RisingEdge) || (ChannelTriggerMode == TriggerMode.FallingEdge)) field += 128;   // Edge

            DeviceModel.WriteByte(5, field);
        }


        private void UpdateControls()
        {
            byte field = 0;
            if (Channel1On) field += 1;
            if (Channel1Inv) field += 16;
            if (Channel1Avrg) field += 32;
            if (Channel1Math) field += 64;
            if (Channel1MathType == ChannelMathType.Subtract)
                field += 128;

            DeviceModel.WriteByte(1, field);
        }

        protected void OnUpdateScopeView(bool force = false)
        {
            var handler = UpdateScopeView;
            if (handler != null)
            {
                handler(this, new UpdateScopeViewEventArgs { Force =  force});
            }
        }

        protected void OnConnected(int firmwareVersion)
        {
            var handler = Connected;
            if (handler != null)
            {
                handler(this, new ConnectedEventArgs {FirmwareVersion = firmwareVersion});
            }
        }
        private void ConnectClicked()
        {
            // connect
            var version = 0;
            DeviceModel.USBConnect(ref version);

            if (!DeviceModel.DeviceDetected) return;

            OnPropertyChanged("IsConnected");

            OnConnected(version);

            // read settings
            var settingsData = DeviceModel.ReadSettings();

            if (settingsData != null) ConvertSettings(settingsData);

            // then queue call to read bulk data
            DeviceModel.ReadBulkData();
        }

        private void ConvertSettings(byte[] settingsData)
        {
            // GPIO0 srate
            SamplingRate = settingsData[GPIO0SRate];   // Sampling rate

            // GPIO1 CH1 Option
            var data = settingsData[GPIO1Ch1Option];
            ProcessChannel1Option(data);

            data = settingsData[GPIO5Trigger];
            ProcessTriggerOption(data);

            // M 29 Channel 1 position
            data = (byte)(ChannelBottomPosition - (sbyte)settingsData[MCh1Pos]);
            Channel1Pos = (sbyte)data;

            // M 12 Channel 1 Gain
            data = settingsData[MCh1Gain];
            Channel1Gain = data;

            // GPIOB MStatus
            data = settingsData[GPIOBMStatus];
            IsScopeStopped = ((data & 16) != 0);
        }

        private void ProcessTriggerOption(byte data)
        {
            if ((data & 1) != 0) _channelTriggerType = TriggerType.Normal;
            else if ((data & 2) != 0) _channelTriggerType = TriggerType.Single;
            else if ((data & 4) != 0) _channelTriggerType = TriggerType.Auto;
            else _channelTriggerType = TriggerType.Free;

            if ((data & 128) != 0)
            {      // edge
                _channelTriggerMode = (data & 8) != 0 ? TriggerMode.FallingEdge : TriggerMode.RisingEdge;
            }
            else if ((data & 32) != 0)
            { // slope
                _channelTriggerMode = (data & 8) != 0 ? TriggerMode.NegativeSlope : TriggerMode.PositiveSlope;
            }
            else if ((data & 64) != 0) _channelTriggerMode = TriggerMode.Window;
            else _channelTriggerMode = TriggerMode.DualEdge;

            _protocolIsCircular = ((data & 16) != 0);

            OnPropertyChanged("ChannelTriggerType");
            OnPropertyChanged("ChannelTriggerMode");
            OnPropertyChanged("IsProtocolCircular");

        }

        private void ProcessChannel1Option(byte data)
        {
            _channel1On = ((data & 1) != 0);
            _channel1Inv = ((data & 16) != 0);
            _channel1Avrg = ((data & 32) != 0);
            _channel1Math = ((data & 64) != 0);
            _channel1MathType = (data & 128) != 0 ? ChannelMathType.Subtract : ChannelMathType.Multiply;

            OnPropertyChanged("Channel1On");
            OnPropertyChanged("Channel1Inv");
            OnPropertyChanged("Channel1Avrg");
            OnPropertyChanged("Channel1Math");
            OnPropertyChanged("Channel1MathType");
        }

        #endregion

        public void OnDeviceChange(IntPtr wParam, IntPtr lParam)
        {
            var eventType = wParam.ToInt32();
            if (eventType == DeviceManagement.DBT_DEVICEARRIVAL)
            {
                // we have an arrival event
                ConnectClicked();
            }
            else if (eventType == DeviceManagement.DBT_DEVICEREMOVECOMPLETE)
            {
                // we have a disconnect event
                DeviceModel.ForceDisconnect();
                ScopeData.Clear();
                OnPropertyChanged("ScopeData");

                OnUpdateScopeView(true);
            }
        }
    }

    public class UpdateScopeViewEventArgs : EventArgs
    {
        public bool Force
        {
            get; set; 
        }
    }

    public class ConnectedEventArgs : EventArgs
    {
        public int FirmwareVersion
        {
            get; set; 
        }
    }
}
