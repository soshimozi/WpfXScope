using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfXScope.Controls
{
    /// <summary>
    /// Interaction logic for ColorChooser.xaml
    /// </summary>
    public partial class ColorChooser : UserControl
    {

        public static readonly DependencyProperty AccentColorsProperty =
            DependencyProperty.Register(
                "AccentColors",
                typeof (ObservableCollection<Color>),
                typeof (ColorChooser),
                new FrameworkPropertyMetadata(new ObservableCollection<Color>())
                );

        public ObservableCollection<Color> AccentColors
        {
            get { return (ObservableCollection<Color>) GetValue(AccentColorsProperty); }
            set { SetValue(AccentColorsProperty, value); }
        }

        public static readonly DependencyProperty SelectedAccentColorProperty =
            DependencyProperty.Register(
                "SelectedAccentColor",
                typeof (Color),
                typeof (ColorChooser),
                new FrameworkPropertyMetadata(Colors.Black)
                );

        public Color SelectedAccentColor
        {
            get { return (Color) GetValue(SelectedAccentColorProperty); }
            set { SetValue(SelectedAccentColorProperty, value); }
        }

        //AccentColors
        public ColorChooser()
        {
            InitializeComponent();
        }

    }
}
