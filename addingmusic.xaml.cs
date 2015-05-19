using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;

namespace Symphonia
{
    public sealed partial class addingmusic : Page
    {
        private Boolean navigated;
        private CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        private DisplayRequest request = new DisplayRequest();
        public addingmusic()
        {
            this.InitializeComponent();
            base.DataContext = App.newtracks;
            base.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!this.navigated)
            {
                if (!(Boolean)e.Parameter)
                {
                    this.StartScanning(null, null);
                }
                this.navigated = true;
            }
        }

        private async void StartScanning(Object sender, TappedRoutedEventArgs e)
        {
            this.mainpivot.SelectedIndex = 1;
            this.request.RequestActive();
            this.navigated = true;
            await App.musicdata.PopulateAsync();
            this.request.RequestRelease();
            await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => base.Frame.Navigate(typeof(MainPage)));
        }
    }
}
