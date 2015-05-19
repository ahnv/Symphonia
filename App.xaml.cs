using Symphonia;
using Symphonia.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
// using ThemeControllerHelper;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;


namespace Symphonia
{
    public sealed partial class App : Application
    {
        private ApplicationDataContainer localsettings = ApplicationData.Current.LocalSettings;
        private TransitionCollection transitions;
        public static newtrackscount newtracks { get; set; }
        public static now_playing nowplaying { get; set; }
        public static MusicData musicdata { get; set; }
        public static Boolean appstart { get; set; }
        public static Boolean ignorebackbutton { get; set; }
        public static Boolean istrail { get; set; }
        public static play_music playmusic { get; set; }

        public static async void ShowError(String message)
        {
            try
            {
                MessageDialog messageDialog = new MessageDialog(message) { Title = "Error" };
                await messageDialog.ShowAsync();
                
            }
            catch (Exception exception) { }    
        }
  
        public App()
        {
            this.Suspending += this.OnSuspending;
        }
        private void CloseApp(IUICommand command)
        {
            base.Exit();
        }

        private async Task InitiateSettings()
        {
            DateTime dateTime = new DateTime();
//            await StatusBar.GetForCurrentView().HideAsync();
            this.localsettings.Values["foreground_running"]= true;
            if(!this.localsettings.Values.ContainsKey("first_time_use"))
            {
                IPropertySet values = this.localsettings.Values;
                DateTime now = DateTime.Now;
                ((IDictionary<String, Object>)values)["first_time_use"] = now.ToString();
            }
            if (!this.localsettings.Values.ContainsKey("show_nowplaying_when_tapping_song"))
            {
                this.localsettings.Values["show_nowplaying_when_tapping_song"] = true;
            }
            if (!this.localsettings.Values.ContainsKey("hasrated"))
            {
                this.localsettings.Values["hasrated"] = false;
            }
            if (!this.localsettings.Values.ContainsKey("timeslaunched"))
            {
                this.localsettings.Values["timeslaunched"] = -1;
            }

            if (!CurrentApp.LicenseInformation.IsTrial)
            {
                App.istrail = false;
            }
            else
            {
                App.istrail = true;
                DateTime.TryParse((String)this.localsettings.Values["first_time_use"], out dateTime);
                if ((DateTime.Now - dateTime).Days > 4)
                {
                   // await this.AskToBuy();
                }
            }
            
            //Boolean flag = (Boolean)ApplicationSettingsHelper.ReadResetSettingsValue("hasrated");
            //Int32 num8 = (Int32)ApplicationSettingsHelper.ReadResetSett   ingsValue("timeslaunched");
            //ApplicationSettingsHelper.SaveSettingsValue("timeslaunched", num8 + 1);
            // if (!flag && (num8 == 10 || num8 == 40))
            // {
              //  this.AskForReview();
            // }*/
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            try
            {
                if (App.playmusic == null) { App.playmusic = new play_music(); }
                if (!App.playmusic.running) { App.playmusic.Subscribe(); }
                if (App.nowplaying == null) { App.nowplaying = new now_playing(); }
                if (!App.nowplaying.running) { App.nowplaying.Subscribe(); }
                if (App.newtracks == null) { App.newtracks = new newtrackscount(); }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                String[] str = new String[] { "Error while initiating date in OnLaunched: ", exception.Message.ToString(), Environment.NewLine, Environment.NewLine, "Please report this error to dhiman.ahnv@gmail.com" };
                App.ShowError(String.Concat(str));
            }
            App.ignorebackbutton = false;
            Boolean flag = false;
            Boolean flag1 = false;

            try
            {
                if (App.musicdata != null) { 
                    flag = await App.musicdata.CheckForNewMusic(); }
                else
                {
                    Int32 num = await readwrite.ReadMusicData();
                    if(num == 1) { flag1 = true; }
                    else if (num == 2) { flag = true; }
                }
            }
            catch (Exception exception3)
            {
                Exception exception2 = exception3;
                String[] strArray = new String[] { "Error while load musicdata in OnLaunched: ", exception2.Message.ToString(), Environment.NewLine, Environment.NewLine, "Please report this error to dhiman.ahnv@gmail.com" };
                App.ShowError(String.Concat(strArray));
            }
            try
			{
				await this.InitiateSettings();
			}
            catch (Exception exception5)
            {
                Exception exception4 = exception5;
                String[] str1 = new String[] { "Error while initiating settings in OnLaunched: ", exception4.Message.ToString(), Environment.NewLine, Environment.NewLine, "Please report this error to dhiman.ahnv@gmail.com" };
                App.ShowError(String.Concat(str1));
            }
           
            Frame content = Window.Current.Content as Frame;
            try
            {
                if (content == null)
                {
                    content = new Frame();
                    SuspensionManager.RegisterFrame(content, "rootFrameKey", null);
                    if (e.PreviousExecutionState != ApplicationExecutionState.Terminated) { App.appstart = true; } 
                    else
                    {   try { await SuspensionManager.RestoreAsync(null); }
                        catch (Exception exception6) { } 
                    }
                    Window.Current.Content = content;
                }
            }
            catch (Exception exception8)
            {
                Exception exception7 = exception8;
                String[] strArray1 = new String[] { "Error in 'rootFrame == null' in OnLaunched: ", exception7.Message.ToString(), Environment.NewLine, Environment.NewLine, "Please report this error to dhiman.ahnv@gmail.com" };
                App.ShowError(String.Concat(strArray1));
            }
            try
            {
                if(content.Content == null)
                {
                    if (flag) { 
                        content.Navigate(typeof(addingmusic),false); flag = false;}
                    else if (!flag1) { 
                        content.Navigate(typeof(MainPage));}
                    else { 
                        content.Navigate(typeof(addingmusic),true); flag = false; }
                }
            }
            catch (Exception exception10)
			{
				Exception exception9 = exception10;
				String[] str2 = new String[] { "Error in 'rootFrame.content == null' in OnLaunched: ", exception9.Message.ToString(), Environment.NewLine, Environment.NewLine, "Please report this error to dhiman.ahnv@gmail.com" };
				App.ShowError(String.Concat(str2));
			}
            try { if (flag) {	content.Navigate(typeof(addingmusic), false);	flag = false; } }
            catch (Exception exception12)
			{
				Exception exception11 = exception12;
				String[] strArray2 = new String[] { "Error in 'checkfornewmusic' in OnLaunched: ", exception11.Message.ToString(), Environment.NewLine, Environment.NewLine, "Please report this error to dhiman.ahnv@gmail.com" };
				App.ShowError(String.Concat(strArray2));
			}
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Window.Current.Activate();
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            App.playmusic.UnSubscribe();
            App.playmusic = null;
            App.nowplaying.UnSubscribe();
            App.nowplaying = null;
            await SuspensionManager.SaveAsync();
            ApplicationSettingsHelper.SaveSettingsValue("appstate", "Suspended");
			deferral.Complete();
        }
    }
}