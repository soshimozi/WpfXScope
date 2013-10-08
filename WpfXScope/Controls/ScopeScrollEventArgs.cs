using System.Windows;

namespace WpfXScope.Controls
{
    public class ScopeScrollEventArgs : RoutedEventArgs
    {
        public int Position { get; set; }
    }
}