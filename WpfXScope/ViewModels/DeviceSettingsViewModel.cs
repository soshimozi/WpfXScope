using System.Collections.Generic;
using System.Collections.ObjectModel;
using WpfXScope.Models;

namespace WpfXScope.ViewModels
{
    public class DeviceSettingsViewModel : NotifyObject
    {

        static readonly ImageData[] TriggerModeImageData = new[]
        {
            new ImageData
                        {
                            ImageSource = "/WpfXScope;component/Images/rising.png",
                            Text = "Rising Edge",
                            Id = (int)TriggerMode.RisingEdge
                        },
            new ImageData
                        {
                            ImageSource = "/WpfXScope;component/Images/falling.png",
                            Text = "Falling Edge",
                            Id = (int)TriggerMode.FallingEdge
                        },
            new ImageData
                        {
                            ImageSource = "/WpfXScope;component/Images/positive.png",
                            Text = "Positive Slope",
                            Id = (int)TriggerMode.PositiveSlope
                        },
            new ImageData
                        {
                            ImageSource = "/WpfXScope;component/Images/negative.png",
                            Text = "Negative Slope",
                            Id = (int)TriggerMode.NegativeSlope
                        },
            new ImageData
                        {
                            ImageSource = "/WpfXScope;component/Images/window.png",
                            Text = "Window",
                            Id = (int)TriggerMode.Window
                        },
            new ImageData
                        {
                            ImageSource = "/WpfXScope;component/Images/dual.png",
                            Text = "Dual Edge",
                            Id = (int)TriggerMode.DualEdge
                        }




        };

        private readonly List<TriggerSource>  _triggerSourceList = new List<TriggerSource>
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

        public DeviceSettingsViewModel()
        {
            DeviceSettingsManager.SettingsChanged += DeviceSettingsManagerOnSettingsChanged;
            TriggerSources = new ObservableCollection<TriggerSource>(_triggerSourceList);
            OnPropertyChanged("TriggerSources");

            TriggerImages = new ObservableCollection<ImageData>(TriggerModeImageData);
        }

        private void DeviceSettingsManagerOnSettingsChanged(object sender, SettingsChangedEventArgs settingsChangedEventArgs)
        {
            OnPropertyChanged(settingsChangedEventArgs.Name);
        }

        private ImageData _selectedTriggerTypeImage;
        public ImageData SelectedTriggerTypeImage
        {
            get { return _selectedTriggerTypeImage; }
            set { SetField(ref _selectedTriggerTypeImage, value, "SelectedImage"); }
        }

        //private int _triggerModeImageId;
        //public int TriggerModeImageId
        //{
        //    get { return _triggerModeImageId; }
        //    set { _triggerModeImageId = value; }
        //}


        public ObservableCollection<ImageData> TriggerImages { get; private set; }

        public byte ChannelTriggerSource
        {
            get { return DeviceSettingsManager.ChannelTriggerSource; }
            set { DeviceSettingsManager.ChannelTriggerSource = value; }
        }

        public ObservableCollection<TriggerSource> TriggerSources
        {
            get; private set;
        }

        public static bool ChannelDOn
        {
            get { return DeviceSettingsManager.ChannelDOn; }
            set { DeviceSettingsManager.ChannelDOn = value; }
        }

        public static bool Channel2On
        {
            get { return DeviceSettingsManager.Channel2On; }
            set { DeviceSettingsManager.Channel2On = value; }
        }

        public static bool Channel1On
        {
            get { return DeviceSettingsManager.Channel1On; }
            set { DeviceSettingsManager.Channel1On = value; }
        }

        public ChannelMathType Channel2MathType
        {
            get { return DeviceSettingsManager.Channel2MathType; }
            set { DeviceSettingsManager.Channel2MathType = value; }
        }


        public bool Channel2Math
        {
            get { return DeviceSettingsManager.Channel2Math; }
            set { DeviceSettingsManager.Channel2Math = value; }
        }

        public ChannelMathType Channel1MathType
        {
            get { return DeviceSettingsManager.Channel1MathType; }
            set { DeviceSettingsManager.Channel1MathType = value; }
        }


        public bool Channel1Math
        {
            get { return DeviceSettingsManager.Channel1Math; }
            set { DeviceSettingsManager.Channel1Math = value; }
        }

        public bool Channel2Avrg
        {
            get { return DeviceSettingsManager.Channel2Avrg; }
            set { DeviceSettingsManager.Channel2Avrg = value; }
        }

        public bool Channel1Avrg
        {
            get { return DeviceSettingsManager.Channel1Avrg; }
            set { DeviceSettingsManager.Channel1Avrg = value; }
        }

        public bool Channel2Inv
        {
            get { return DeviceSettingsManager.Channel2Inv; }
            set { DeviceSettingsManager.Channel2Inv = value; }
        }

        public bool Channel1Inv
        {
            get { return DeviceSettingsManager.Channel1Inv;  }
            set { DeviceSettingsManager.Channel1Inv = value;  }
        }

        public int Channel2Offset
        {
            get { return DeviceSettingsManager.Channel2Offset; }
            set { DeviceSettingsManager.Channel2Offset = value; }

        }

        public int Channel2Gain
        {
            get { return DeviceSettingsManager.Channel2Gain; }
            set { DeviceSettingsManager.Channel2Gain = value; }
        }


        public int Channel1Offset
        {
            get { return DeviceSettingsManager.Channel1Offset; }
            set { DeviceSettingsManager.Channel1Offset = value; }
            
        }

        public int Channel1Gain
        {
            get { return DeviceSettingsManager.Channel1Gain; }
            set { DeviceSettingsManager.Channel1Gain = value; }
        }

        public decimal AutoTriggerTimeout
        {
            get { return DeviceSettingsManager.AutoTriggerTimeout; }
            set { DeviceSettingsManager.AutoTriggerTimeout = value; }
        }

        public bool IsProtocolCircular
        {
            get { return DeviceSettingsManager.IsProtocolCircular; }
            set { DeviceSettingsManager.IsProtocolCircular = value; }
        }

        public int TriggerHold
        {
            get { return DeviceSettingsManager.TriggerHold; }
            set { DeviceSettingsManager.TriggerHold = value; }
        }

        public int ChannelTriggerMode
        {
            get { return (int)DeviceSettingsManager.ChannelTriggerMode; }
            set { DeviceSettingsManager.ChannelTriggerMode = (TriggerMode)value; }
        }

        public TriggerType ChannelTriggerType
        {
            get { return DeviceSettingsManager.ChannelTriggerType;  }
            set { DeviceSettingsManager.ChannelTriggerType = value; }
        }

        public int HorizontalFrequency
        {
            get { return DeviceSettingsManager.HorizontalFrequency; }
            set { DeviceSettingsManager.HorizontalFrequency = value; }
        }
    }
}
