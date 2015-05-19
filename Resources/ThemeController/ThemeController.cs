using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Windows.UI.Xaml;

namespace ThemeControllerHelper
{
    class ThemeController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String p)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }
        public ThemeCollection Themes
        {
            get
            {
                return (ThemeCollection)Application.Current.Resources["Themes"];
            }
        }
        public String DefaultTheme { get; set; }
        private Theme activeTheme;
        private Theme ActiveTheme
        {
            get 
            { 
                if (this.activeTheme == null)
                {
                    ThemeCollection themes = this.Themes;
                    this.activeTheme = themes.FirstOrDefault<Theme>(Theme => Theme.ThemeName == this.DefaultTheme);
                }
                return this.activeTheme;
            }
            set
            {
                if (this.activeTheme != value) { this.activeTheme = value; this.NotifyPropertyChanged("ActiveTheme"); }
            }
        }
    }
}
