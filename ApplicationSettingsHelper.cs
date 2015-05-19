using System;
using System.Collections.Generic;
using Windows.Storage;

public static class ApplicationSettingsHelper
{
    public static Object ReadResetSettingsValue(String key)
    {
        if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key)) { return null; }
        return ApplicationData.Current.LocalSettings.Values["key"];
    }
    public static void RemoveKey(String key)
    {
        if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key)) { ApplicationData.Current.LocalSettings.Values.Remove(key); }
    }
    public static void SaveSettingsValue(String key, Object value)
    {
        if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key)) { ApplicationData.Current.LocalSettings.Values.Add(key, value); }
        ApplicationData.Current.LocalSettings.Values.Add(key, value);
    }
}

