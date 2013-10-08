using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfXScope.Controls
{
    /// <summary>
    /// Interaction logic for OnOffIndicator.xaml
    /// </summary>
// ReSharper disable RedundantExtendsListEntry
    public partial class OnOffIndicator : UserControl
// ReSharper restore RedundantExtendsListEntry
    {
        public OnOffIndicator()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty OffStateContentProperty =
            DependencyProperty.Register(
                "OffStateContent",
                typeof(string),
                typeof(OnOffIndicator),
                new FrameworkPropertyMetadata(null)
                );

        public string OffStateContent
        {
            get
            {
                return (string)GetValue(OffStateContentProperty);
            }
            set
            {
                SetValue(OffStateContentProperty, value);
            }
        }

        public static readonly DependencyProperty OnStateContentProperty =
            DependencyProperty.Register(
                "OnStateContent",
                typeof(string),
                typeof(OnOffIndicator),
                new FrameworkPropertyMetadata(null)
                );

        public string OnStateContent
        {
            get
            {
                return (string)GetValue(OnStateContentProperty);
            }
            set
            {
                SetValue(OnStateContentProperty, value);   
            }
        }


        public static readonly DependencyProperty OnStateProperty =
            DependencyProperty.Register(
                "OnState",
                typeof(bool),
                typeof(OnOffIndicator),
                new FrameworkPropertyMetadata(false)
                );

        public bool OnState
        {
            get { return (bool)GetValue(OnStateProperty); }
            set { SetValue(OnStateProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                "Stroke",
                typeof (Brush),
                typeof (OnOffIndicator),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray))
                );
        
        public Brush Stroke
        {
            get { return (Brush) GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value);}
        }

        public static readonly DependencyProperty OffStateBackgroundProperty =
            DependencyProperty.Register(
                "OffStateBackground",
                typeof(Brush),
                typeof(OnOffIndicator),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White))
                );

        public Brush OffStateBackground
        {
            get { return (Brush)GetValue(OffStateBackgroundProperty); }
            set { SetValue(OffStateBackgroundProperty, value); }
        }


        public static readonly DependencyProperty OnStateBackgroundProperty =
            DependencyProperty.Register(
                "OnStateBackground",
                typeof(Brush),
                typeof(OnOffIndicator),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black))
                );

        public Brush OnStateBackground
        {
            get { return (Brush)GetValue(OnStateBackgroundProperty); }
            set { SetValue(OnStateBackgroundProperty, value); }
        }

        public static readonly DependencyProperty OutterBorderThicknessProperty =
            DependencyProperty.Register(
                "OuterBorderThickness",
                typeof(int),
                typeof(OnOffIndicator),
                new FrameworkPropertyMetadata(0)
                );

        public int OuterBorderThickness
        {
            get { return (int)GetValue(OutterBorderThicknessProperty); }
            set { SetValue(OutterBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register(
                "Radius",
                typeof(int),
                typeof(OnOffIndicator),
                new FrameworkPropertyMetadata(0)
                );



        public int Radius
        {
            get { return (int)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
    }
}
