using System.Collections.ObjectModel;
using WpfXScope.Models;

namespace WpfXScope.ViewModels
{
    public class WaveformGenerationViewModel : NotifyObject
    {
        private static readonly FrequencyRange[] Frequencies = new[]
                                                                   {
                                                                       new FrequencyRange { Description = "1-10hz", Range = 1},
                                                                       new FrequencyRange { Description = "10-100hz", Range = 2},
                                                                       new FrequencyRange { Description = "100hz-1khz", Range = 3},
                                                                       new FrequencyRange { Description = "1khz-10khz", Range = 4},
                                                                       new FrequencyRange { Description = "10khz-100hz", Range = 5}
                                                                   };

        static readonly ImageData [] ImageData = new []
        {
            new ImageData
                        {
                            ImageSource = "/WpfXScope;component/Images/noise.png",
                            Text = "Noise",
                            Id = (int)WaveformType.Noise
                        },
            new ImageData
                        {
                            ImageSource = "/WpfXScope;component/Images/sine.png",
                            Text = "Sine",
                            Id = (int)WaveformType.Sine
                        },
            new ImageData
                        {
                            ImageSource = "/WpfXScope;component/Images/square.png",
                            Text = "Square",
                            Id = (int)WaveformType.Square
                        },
            new ImageData
                        {
                            ImageSource = "/WpfXScope;component/Images/triangle.png",
                            Text = "Triangle",
                            Id = (int)WaveformType.Triangle
                        },
            new ImageData
                        {
                            ImageSource = "/WpfXScope;component/Images/expo.png",
                            Text = "Exponential",
                            Id = (int)WaveformType.Exponential
                        },
            new ImageData
                        {
                            ImageSource = "",
                            Text = "Custom",
                            Id = (int)WaveformType.Custom
                        }




        };
    
        
     
        public WaveformGenerationViewModel()
        {
            Images = new ObservableCollection<ImageData>(ImageData);
            OnPropertyChanged("Images");

            FrequencyRanges = new ObservableCollection<FrequencyRange>(Frequencies);
            OnPropertyChanged("FrequencyRanges");

        }

        public ObservableCollection<ImageData> Images { get; private set; }
        public ObservableCollection<FrequencyRange> FrequencyRanges { get; private set; }

        public int FrequencyRange
        {
            get { return CalculateFrequencyRange(); }
            set
            {
                UpdateSelectedFrequency(value, FrequencyMultiplier);
                OnPropertyChanged("FrequencyRange");
                OnPropertyChanged("FrequencyMultiplier");

            }
        }

        public decimal DutyCycle
        {
            get { return CalculateDutyCycle(); }
            set
            {
                SetDutyCycle(value);
                OnPropertyChanged("DutyCycle");
            }
        }

        private void SetDutyCycle(decimal value)
        {
            var data = (byte)(value * 100000 * 128 / 5000064);
            if (data == 0) data = 1;
            DeviceSettingsManager.DutyCycle = data;
        }

        private decimal CalculateDutyCycle()
        {
            return (decimal) ((DeviceSettingsManager.DutyCycle)*(50.00064/128));
        }

        private void UpdateSelectedFrequency(int range, decimal multiplier)
        {
            switch(range)
            {
                case 5:
                    DeviceSettingsManager.DesiredFrequency = (uint)multiplier*1000;
                    break;
                case 4:
                    DeviceSettingsManager.DesiredFrequency = (uint)multiplier * 100;
                    break;
                case 3:
                    DeviceSettingsManager.DesiredFrequency = (uint) multiplier*10;
                    break;
                case 2:
                    DeviceSettingsManager.DesiredFrequency = (uint)multiplier;
                    break;
                case 1:
                    DeviceSettingsManager.DesiredFrequency = (uint) multiplier/10;
                    break;
            }

            OnPropertyChanged("SelectedFrequency");
            OnPropertyChanged("ActualFrequency");

        }

        public int FrequencyMultiplier
        {
            get { return CalculateFrequencyMultiplier(); }
            set
            {
                UpdateSelectedFrequency(FrequencyRange, value);
                OnPropertyChanged("FrequencyMultiplier");
                OnPropertyChanged("FrequencyRange");

            }
        }

        private int CalculateFrequencyMultiplier()
        {
            var f = DeviceSettingsManager.DesiredFrequency;

            var range = CalculateFrequencyRange();
            switch (range)
            {
                case 5:
                    return (int)f / 1000;
                case 4:
                    return (int)f / 100;
                case 3:
                    return (int)f / 10;
                case 2:
                    return (int)f;
                case 1:
                    return (int)f * 10;

                default:
                    return 0;
            }            
        }

        private int CalculateFrequencyRange()
        {
            var f = DeviceSettingsManager.DesiredFrequency;

            if (f >= 10000)
                return 5;
            if (f >= 1000)
                return 4;
            if (f >= 100)
                return 3;
            return f >= 10 ? 2 : 1;
        }

        public decimal Amplitude
        {
            get { return (decimal)(DeviceSettingsManager.Amplitude / 32.0); }
            set { DeviceSettingsManager.Amplitude = (int)(value*32); }
        }

        private ImageData _selectedImage;
        public ImageData SelectedImage
        {
            get { return _selectedImage; }
            set { SetField(ref _selectedImage, value, "SelectedImage"); }
        }

        public int ImageId
        {
            get { return (int)DeviceSettingsManager.WaveformType; }
            set { DeviceSettingsManager.WaveformType = (WaveformType) value; }
        }

        public decimal SelectedFrequency
        {
            get { return DeviceSettingsManager.DesiredFrequency; }
            set
            {
                DeviceSettingsManager.DesiredFrequency = (uint)value;

                OnPropertyChanged("SelectedFrequency");
                OnPropertyChanged("ActualFrequency");

                OnPropertyChanged("FrequencyRange");
                OnPropertyChanged("FrequencyMultiplier");
            }
        }

        public decimal ActualFrequency
        {
            get { return DeviceSettingsManager.ActualFrequency >= 1000 ? DeviceSettingsManager.ActualFrequency / 1000 : DeviceSettingsManager.ActualFrequency; }
        }

        public decimal Offset
        {
            get
            {
                return (-DeviceSettingsManager.Offset * (.50016M / 32));
            }
            set
            {
                DeviceSettingsManager.Offset = (sbyte)(-value * 100000 * 32 / 50016); 
                OnPropertyChanged("Offset");
            }
        }
    }
}
