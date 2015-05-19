using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Windows.UI.Core;

public class newtrackscount : INotifyPropertyChanged
{
    private CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

    private Int32 _totaltracks;

    private Int32 _addedtracks;

    public Int32 addedtracks
    {
        get
        {
            return this._addedtracks;
        }
        set
        {
            if (this._addedtracks != value)
            {
                this._addedtracks = value;
            }
            this.NotifyPropertyChanged("addedtracks");
        }
    }

    public Int32 totaltracks
    {
        get
        {
            return this._totaltracks;
        }
        set
        {
            if (this._totaltracks != value)
            {
                this._totaltracks = value;
            }
            this.NotifyPropertyChanged("totaltracks");
        }
    }

    public newtrackscount()
    {
    }

    private async void NotifyPropertyChanged(String p)
    {
        PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
        if (this.PropertyChanged != null)
        {
            CoreDispatcher coreDispatcher = this.dispatcher;
            await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.PropertyChanged(this, new PropertyChangedEventArgs(p)));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}