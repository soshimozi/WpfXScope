using System;
using WpfXScope.ViewModels;

namespace WpfXScope.Models
{
    public static class DeviceSettingsManager
    {
        public static event EventHandler<SettingsChangedEventArgs> SettingsChanged;


        private static int  _horizontalFrequency;
        private static TriggerType _channelTriggerType;
        private static TriggerMode _channelTriggerMode;
        private static bool _protocolIsCircular;
        private static byte _channelTriggerSource;
        private static decimal _autoTriggerTime;
        private static int _channel1Offset;
        private static int _channel1Gain;
        private static bool _channel1On;
        private static bool _channel1Inv;
        private static ChannelMathType _channel1MathType;
        private static bool _channel1Math;
        private static bool _channel1Avrg;
        private static int _channel2Offset;
        private static int _channel2Gain;
        private static bool _channel2On;
        private static bool _channel2Inv;
        private static ChannelMathType _channel2MathType;
        private static bool _channel2Math;
        private static bool _channel2Avrg;
        private static bool _channelDOn;
        private static PullUpType _pullUpType;
        private static bool _channelDAscii;
        private static bool _channelDInv;
        private static bool _channelDThick;
        private static ushort _horizontalPosition;
        private static WaveformType _waveformType;
        private static uint _desiredFrequency;
        private static decimal _actualFrequency;
        private static int _amplitude;
        private static int _offset;
        private static int _dutyCycle;
        private static bool _isStopped;
        private static int _holdTime;

        public static bool IsStopped
        {
            get { return _isStopped;  }
            set
            {
                _isStopped = value;

                // if stopping call stop
                DeviceModel.ScopeActive(!_isStopped);
                OnSettingChanged("IsStopped");
            }
        }
        public static uint DesiredFrequency
        {
            get { return _desiredFrequency;  }
            set 
            { 
                _desiredFrequency = value;
                DeviceModel.UpdateFrequency(value * 100); 
                OnSettingChanged("DesiredFrequency");

                UpdateActualFrequency(value * 100);
            }
        }

        public static WaveformType WaveformType
        {
            get { return _waveformType;  }
            set
            {
                _waveformType = value; 
                DeviceModel.WriteByte(MWaveForm, (byte)value);
                OnSettingChanged("WaveformType");
            }
        }

        public static bool ChannelDOn
        {
            get { return _channelDOn; }
            set { 
                _channelDOn = value;
                UpdateChannelDControls();
                OnSettingChanged("ChannelDOn");
            }
        }

        public static bool ChannelDAscii
        {
            get { return _channelDAscii; }
            set
            {
                _channelDAscii = value;
                UpdateChannelDControls();
                OnSettingChanged("ChannelDAscii");
            }
        }

        public static bool ChannelDInv
        {
            get { return _channelDInv; }
            set
            {
                _channelDInv = value;
                UpdateChannelDControls();
                OnSettingChanged("ChannelDInv");
            }
        }

        public static bool ChannelDThick
        {
            get { return _channelDThick; }
            set
            {
                _channelDThick = value;
                UpdateChannelDControls();
                OnSettingChanged("ChannelDThick");
            }
        }

        public static PullUpType ChannelDPullUp
        {
            get { return _pullUpType; }
            set
            {
                _pullUpType = value;
                UpdateChannelDControls();
                OnSettingChanged("ChannelDPullUp");
            }
        }

        public static bool Channel2On
        {
            get { return _channel2On; }
            set
            {
                _channel2On = value;
                UpdateChannel2Controls();
                OnSettingChanged("Channel2On");
            }
        }

        public static bool Channel1On
        {
            get { return _channel1On; }
            set
            {
                _channel1On = value;
                UpdateChannel1Controls();
                OnSettingChanged("Channel1On");
            }
        }

        public static ushort HorizontalPosition
        {
            get { return _horizontalPosition; }
            set 
            { 
                _horizontalPosition = value;
                DeviceModel.WriteByte(MHPos, (byte)value);
                OnSettingChanged("HorizontalPosition");
            }
        }

        public static ChannelMathType Channel1MathType
        {
            get { return _channel1MathType; }
            set
            {
                _channel1MathType = value;
                UpdateChannel1Controls();
                OnSettingChanged("Channel1MathType");
            }
        }


        public static bool Channel1Math
        {
            get { return _channel1Math; }
            set
            {
                _channel1Math = value;
                UpdateChannel1Controls();
                OnSettingChanged("Channel1Math");
            }
        }

        public static bool Channel1Avrg
        {
            get { return _channel1Avrg; }
            set
            {
                _channel1Avrg = value;
                UpdateChannel1Controls();
                OnSettingChanged("Channel1Avrg");
            }
        }

        public static bool Channel1Inv
        {
            get { return _channel1Inv; }
            set
            {
                _channel1Inv = value;
                UpdateChannel1Controls();
                OnSettingChanged("Channel1Inv");
            }
        }


        public static int Channel1Gain
        {
            get { return _channel1Gain; }
            set
            {
                _channel1Gain = value;
                DeviceModel.WriteByte(MCh1Gain, (byte)value);
                OnSettingChanged("Channel1Gain");
            }
        }

        public static int Channel1Offset
        {
            get { return _channel1Offset; }
            set
            {
                _channel1Offset = value; 
                DeviceModel.WriteByte(MCh1Pos, (byte)(ChannelBottomPosition - value));
                OnSettingChanged("Channel1Offset");
            }
        }

        public static ChannelMathType Channel2MathType
        {
            get { return _channel2MathType; }
            set
            {
                _channel2MathType = value;
                UpdateChannel2Controls();
                OnSettingChanged("Channel2MathType");
            }
        }
        
        public static bool Channel2Math
        {
            get { return _channel2Math; }
            set
            {
                _channel2Math = value;
                UpdateChannel2Controls();
                OnSettingChanged("Channel2Math");
            }
        }

        public static bool Channel2Avrg
        {
            get { return _channel2Avrg; }
            set
            {
                _channel2Avrg = value;
                UpdateChannel2Controls();
                OnSettingChanged("Channel2Avrg");
            }
        }

        public static bool Channel2Inv
        {
            get { return _channel2Inv; }
            set
            {
                _channel2Inv = value;
                UpdateChannel2Controls();
                OnSettingChanged("Channel2Inv");
            }
        }

        public static int Channel2Offset
        {
            get { return _channel2Offset; }
            set
            {
                _channel2Offset = value;
                DeviceModel.WriteByte(MCh2Pos, (byte)(ChannelBottomPosition - value));
                OnSettingChanged("Channel2Offset");
            }
        }

        public static int Channel2Gain
        {
            get { return _channel2Gain; }
            set
            {
                _channel2Gain = value;
                DeviceModel.WriteByte(MCh2Gain, (byte)value);
                OnSettingChanged("Channel2Gain");
            }
        }


        public static int HorizontalFrequency
        {
            get { return _horizontalFrequency; }
            set
            {
                _horizontalFrequency = value; 
                DeviceModel.WriteByte(GPIO0SRate, (byte)value); 
                OnSettingChanged("HorizontalFrequency");
            }
        }

        public static bool IsProtocolCircular
        {
            get { return _protocolIsCircular; }
            set
            {
                _protocolIsCircular = value;
                UpdateTrigger();
                OnSettingChanged("IsProtocolCircular");
            }
        }

        public static TriggerType ChannelTriggerType
        {
            get { return _channelTriggerType; }
            set
            {
                _channelTriggerType = value;
                UpdateTrigger();
                OnSettingChanged("ChannelTriggerType");
            }
        }

        public static TriggerMode ChannelTriggerMode
        {
            get { return _channelTriggerMode; }
            set
            {
                _channelTriggerMode = value;
                UpdateTrigger();
                OnSettingChanged("ChannelTriggerMode");
            }
            
        }

        public static byte ChannelTriggerSource
        {
            get { return _channelTriggerSource; }
            set 
            { 
                _channelTriggerSource = value;
                DeviceModel.WriteByte(MTSource, value);
                OnSettingChanged("ChannelTriggerSource");
            }

        }

        public static decimal AutoTriggerTimeout
        {
            get { return _autoTriggerTime; }
            set
            {
                _autoTriggerTime = value;
                DeviceModel.WriteByte(MTriggerTimeout, (byte)((value / 40.96M) - 1));
                OnSettingChanged("AutoTriggerTimeout");
            }
        }


        private static void OnSettingChanged(string name)
        {
            var handler = SettingsChanged;
            if(handler != null)
            {
                handler(null, new SettingsChangedEventArgs { Name = name });
            }
        }

        public static decimal ActualFrequency
        {
            get { return _actualFrequency;  }
            set
            {
                _actualFrequency = value;
                OnSettingChanged("ActualFrequency");
            }
        }

        public static int Amplitude
        {
            get { return _amplitude; }
            set
            {
                _amplitude = value;

                DeviceModel.WriteByte(MAmplitude, (byte)(sbyte)-value);
                OnSettingChanged("Amplitude");
            }
        }

        public static int Offset
        {
            get { return _offset;  }
            set
            {
                _offset = value;
                DeviceModel.WriteByte(MOffset, (byte)value);
                OnSettingChanged("Offset");
            }
        }

        public static int DutyCycle
        {
            get { return _dutyCycle; }
            set
            {
                _dutyCycle = value;
                DeviceModel.WriteByte(MDutyCycle, (byte)value);
                OnSettingChanged("DutyCycle");
            }
        }

        public static int TriggerHold
        {
            get { return _holdTime; }
            set
            {
                _holdTime = value;
                DeviceModel.WriteByte(MTriggerHold, (byte)value);
                OnSettingChanged("TriggerHold");
            }
        } 

        private const int ChannelBottomPosition = -126;

        private const int MTriggerTimeout = 28;
        private const int GPIO0SRate = 0;
        private const int MCh1Pos = 29;
        private const int MCh2Pos = 30;
// ReSharper disable InconsistentNaming
        private const int MTSource = 24;
// ReSharper restore InconsistentNaming
        private const int MCh1Gain = 12;
        private const int MCh2Gain = 13;
        private const int GPIO5Trigger = 5;
        private const int GPIO1Ch1Option = 1;
        private const int GPIO1Ch2Option = 2;
        private const int GPIO1ChDOption = 3;
        private const int MWaveForm = 37;
        private const int MTriggerHold = 21;
// ReSharper disable InconsistentNaming
        private const int GPIOBMStatus = 11;
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        private const int MHPos = 14;
// ReSharper restore InconsistentNaming
        private const int MAmplitude = 36;
        private const int MDutyCycle = 38;
        private const int MOffset = 39;

        public static void FromDeviceData(byte[] settings)
        {
            _channel1Offset = (sbyte)(ChannelBottomPosition - (sbyte)settings[MCh1Pos]);
            _channel2Offset = (sbyte)(ChannelBottomPosition - (sbyte)settings[MCh2Pos]);

            _horizontalFrequency = settings[GPIO0SRate];
            _autoTriggerTime = ((decimal)settings[MTriggerTimeout] + 1) * 40.96M;
            _channelTriggerSource = settings[MTSource];

            _channel1Gain = settings[MCh1Gain];
            _channel2Gain = settings[MCh2Gain];

            _horizontalPosition = settings[MHPos];

            _waveformType = (WaveformType)settings[MWaveForm];

            _isStopped = ((settings[GPIOBMStatus] & 16) != 0);

            // M 36 Amplitude range: [-128,0]
            _amplitude = (byte)(-settings[MAmplitude]);

            // 38 Duty cycle range: [1,255]
            _dutyCycle = settings[MDutyCycle];
            if (_dutyCycle == 0) _dutyCycle++;

            // M 39 Offset
            _offset = settings[MOffset];

            // M 21 Trigger Hold
            _holdTime = settings[MTriggerHold];
            
            // 40 Desired frequency
            _desiredFrequency = ((16777216 * ((UInt32)settings[43])) +
                    (65536 * ((UInt32)settings[42])) +
                    (256 * ((UInt32)settings[41])) +
                    (1 * ((UInt32)settings[40]))) / 100;

            UpdateActualFrequency(_desiredFrequency * 100);

            ProcessChannel1Option(settings[GPIO1Ch1Option]);
            ProcessChannel2Option(settings[GPIO1Ch2Option]);
            ProcessChannelDOption(settings[GPIO1ChDOption]);

            ProcessTriggerOption(settings[GPIO5Trigger]);

            OnSettingChanged("Amplitude");
            OnSettingChanged("Offset");
            OnSettingChanged("DutyCycle");

            OnSettingChanged("Channel1Gain");
            OnSettingChanged("Channel2Gain");

            OnSettingChanged("ChannelTriggerSource");
            OnSettingChanged("Channel1Offset");
            OnSettingChanged("Channel2Offset");
            OnSettingChanged("HorizontalFrequency");
            OnSettingChanged("AutoTriggerTimeout");

            OnSettingChanged("HorizontalPosition");
            OnSettingChanged("WaveformType");

            OnSettingChanged("DesiredFrequency");
            OnSettingChanged("ActualFrequency");

            OnSettingChanged("IsStopped");
        }

        private static void UpdateActualFrequency(uint value)
        {
            var desiredF = value;
            byte i, cycles;
            uint flevel = 1600000;

            // Find Period and number of cycles depending on the desired frequency
            for (i = 0, cycles = 64; i < 6; i++)
            {
                if (desiredF > flevel) break;
                flevel = flevel >> 1;
                cycles = (byte)(cycles >> 1);
            }
            var period = (UInt16)(((6250000 * cycles) / desiredF) - 1);
            if (period < 31) period = 31;
            decimal actualF = (decimal)(cycles * 50 * (125000000L / (period + 1))) / 100000;
            ActualFrequency = actualF;
        }

        private static void UpdateChannelDControls()
        {
            byte field = 0;
            if (ChannelDOn) field += (1 << 0);    // CHD Option
            if (ChannelDPullUp != PullUpType.None) field += (1 << 1);
            if (ChannelDPullUp == PullUpType.PullDown) field += (1 << 2);
            if (ChannelDThick) field += (1 << 3);
            if (ChannelDInv) field += (1 << 4);
            if (ChannelDAscii) field += (1 << 7);

            DeviceModel.WriteByte(GPIO1ChDOption, field);
        }

        private static void UpdateChannel2Controls()
        {
            byte field = 0;
            if (Channel2On) field += 1;
            if (Channel2Inv) field += 16;
            if (Channel2Avrg) field += 32;
            if (Channel2Math) field += 64;
            if (Channel2MathType == ChannelMathType.Subtract)
                field += 128;

            DeviceModel.WriteByte(GPIO1Ch2Option, field);
        }

        private static void UpdateChannel1Controls()
        {
            byte field = 0;
            if (Channel1On) field += 1;
            if (Channel1Inv) field += 16;
            if (Channel1Avrg) field += 32;
            if (Channel1Math) field += 64;
            if (Channel1MathType == ChannelMathType.Subtract)
                field += 128;

            DeviceModel.WriteByte(GPIO1Ch1Option, field);
        }

        private static void ProcessChannelDOption(byte data)
        {
            _channelDOn = ((data & 1) != 0);
            if ((data & 2) != 0)
            {
                ChannelDPullUp = (data & 4) != 0 ? PullUpType.PullDown : PullUpType.PullUp;
            }
            else ChannelDPullUp = PullUpType.None;

            _channelDThick = ((data & 8) != 0);
            _channelDInv = ((data & 16) != 0);
            _channelDAscii = ((data & 128) != 0);

            OnSettingChanged("ChannelDOn");
            OnSettingChanged("ChannelDInv");
            OnSettingChanged("ChannelDThick");
            OnSettingChanged("ChannelDPullUp");
        }

        private static void ProcessChannel2Option(byte data)
        {
            _channel2On = ((data & 1) != 0);
            _channel2Inv = ((data & 16) != 0);
            _channel2Avrg = ((data & 32) != 0);
            _channel2Math = ((data & 64) != 0);
            _channel2MathType = (data & 128) != 0 ? ChannelMathType.Subtract : ChannelMathType.Multiply;

            OnSettingChanged("Channel2On");
            OnSettingChanged("Channel2Inv");
            OnSettingChanged("Channel2Avrg");
            OnSettingChanged("Channel2Math");
            OnSettingChanged("Channel2MathType");
        }

        private static void ProcessChannel1Option(byte data)
        {
            _channel1On = ((data & 1) != 0);
            _channel1Inv = ((data & 16) != 0);
            _channel1Avrg = ((data & 32) != 0);
            _channel1Math = ((data & 64) != 0);
            _channel1MathType = (data & 128) != 0 ? ChannelMathType.Subtract : ChannelMathType.Multiply;

            OnSettingChanged("Channel1On");
            OnSettingChanged("Channel1Inv");
            OnSettingChanged("Channel1Avrg");
            OnSettingChanged("Channel1Math");
            OnSettingChanged("Channel1MathType");
        }

        private static void UpdateTrigger()
        {
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

            DeviceModel.WriteByte(GPIO5Trigger, field);
        }

        private static void ProcessTriggerOption(byte data)
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

            OnSettingChanged("ChannelTriggerType");
            OnSettingChanged("ChannelTriggerMode");
            OnSettingChanged("IsProtocolCircular");

        }


        public static void Auto()
        {
            DeviceModel.ScopeActive(true);

            _isStopped = false;
            OnSettingChanged("IsStopped");

            DeviceModel.AutoSettings();

            // we have to assume everything changed
            var settings = DeviceModel.ReadSettings();
            FromDeviceData(settings);

            //OnSettingChanged("Amplitude");
            //OnSettingChanged("Offset");
            //OnSettingChanged("DutyCycle");

            //OnSettingChanged("Channel1Gain");
            //OnSettingChanged("Channel2Gain");

            //OnSettingChanged("ChannelTriggerSource");
            //OnSettingChanged("Channel1Offset");
            //OnSettingChanged("Channel2Offset");
            //OnSettingChanged("HorizontalFrequency");
            //OnSettingChanged("AutoTriggerTimeout");

            //OnSettingChanged("HorizontalPosition");
            //OnSettingChanged("WaveformType");

            //OnSettingChanged("DesiredFrequency");
            //OnSettingChanged("ActualFrequency");

        }
    }
}
