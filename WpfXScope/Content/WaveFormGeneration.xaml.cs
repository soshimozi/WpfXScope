using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfXScope.Controls;
using WpfXScope.ViewModels;

namespace WpfXScope.Content
{
    /// <summary>
    /// Interaction logic for WaveFormGeneration.xaml
    /// </summary>
    public partial class WaveFormGeneration : UserControl
    {
        public WaveFormGeneration()
        {
            InitializeComponent();

            this.DataContext = new WaveformGenerationViewModel();
        }
    }

    //public class WaveForm
    //{
    //    public string ImagePath { get; set; }
    //    public string Name { get; set; }
    //    public int Waveform { get; set; }
    //}
}
