using System;

namespace WpfXScope.Models
{
    public class SettingsChangedEventArgs : EventArgs
    {
        public string Name
        {
            get; set;
        }
    }
}