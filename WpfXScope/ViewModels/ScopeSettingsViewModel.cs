using System.Collections.ObjectModel;
using System.Windows.Media;

namespace WpfXScope.ViewModels
{
    public class ScopeSettingsViewModel : NotifyObject
    {
        private readonly Color[] _accentColors = new[]{
            Color.FromRgb(0xa4, 0xc4, 0x00),   // lime
            Color.FromRgb(0x60, 0xa9, 0x17),   // green
            Color.FromRgb(0x00, 0x8a, 0x00),   // emerald
            Color.FromRgb(0x00, 0xab, 0xa9),   // teal
            Color.FromRgb(0x1b, 0xa1, 0xe2),   // cyan
            Color.FromRgb(0x00, 0x50, 0xef),   // cobalt
            Color.FromRgb(0x6a, 0x00, 0xff),   // indigo
            Color.FromRgb(0xaa, 0x00, 0xff),   // violet
            Color.FromRgb(0xf4, 0x72, 0xd0),   // pink
            Color.FromRgb(0xd8, 0x00, 0x73),   // magenta
            Color.FromRgb(0xa2, 0x00, 0x25),   // crimson
            Color.FromRgb(0xe5, 0x14, 0x00),   // red
            Color.FromRgb(0xfa, 0x68, 0x00),   // orange
            Color.FromRgb(0xf0, 0xa3, 0x0a),   // amber
            Color.FromRgb(0xe3, 0xc8, 0x00),   // yellow
            Color.FromRgb(0x82, 0x5a, 0x2c),   // brown
            Color.FromRgb(0x6d, 0x87, 0x64),   // olive
            Color.FromRgb(0x64, 0x76, 0x87),   // steel
            Color.FromRgb(0x76, 0x60, 0x8a),   // mauve
            Color.FromRgb(0x87, 0x79, 0x4e), // taupe
            Color.FromRgb(0, 0, 0), // black
            Colors.BlueViolet,
            Colors.Brown,
            Colors.RosyBrown,
            Colors.CadetBlue,
            Colors.Chartreuse,
            Colors.DarkBlue,
            Colors.DarkGreen,
            Colors.Gray,
            Colors.DarkGray
        };

        public ScopeSettingsViewModel()
        {
            AccentColors = new ObservableCollection<Color>(_accentColors);
            OnPropertyChanged("AccentColors");
        }

        public ObservableCollection<Color> AccentColors
        {
            get; private set; 
        }


        public Color SelectedAccentColor
        {
            get { return Properties.Settings.Default.ScopeBackground; }
            set
            {
                Properties.Settings.Default.ScopeBackground = value;
                OnPropertyChanged("SelectedAccentColor");
            }
        }
    }
}
