# 📦 Code Snippets Overview

This document contains representative code snippets from the CUFI Mobile project, organized by category. Each snippet includes a short explanation of what it does and why it’s useful.

## 🔧 Controls

### AppointmentsCollectionView.xaml.cs
```csharp
using cufi.mobile.Models.Em3;

namespace cufi.mobile.Controls
{
    public partial class AppointmentsCollectionView : ContentView
    {
        public EventContentViewModel Model { get; set; }

        public AppointmentsCollectionView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            var model = (EventContentViewModel)BindingContext;
            if (model != null)
            {
                Model = model;
            }

            base.OnBindingContextChanged();
        }
    }
}
```

### CongressCollectionView.xaml
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<ContentView
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:model="clr-namespace:cufi.mobile.Models.Em3"
    xmlns:ff="clr-namespace:FFImageLoading.Maui;assembly=FFImageLoading.Compat.Maui"
    x:Class="cufi.mobile.Controls.CongressCollectionView">
    <StackLayout
        x:DataType="model:EventContentViewModel"
        Padding="20,20,20,0"
        HorizontalOptions="Center"
        BackgroundColor="White">

        <StackLayout
            HorizontalOptions="CenterAndExpand"
            VerticalOptions="StartAndExpand"
            BackgroundColor="{StaticResource Primary}">
            <Label
                Text="{Binding FormalName}"
                HorizontalOptions="Center"
                VerticalOptions="Center"
                TextColor="White"
                Margin="5,5"
                FontFamily="SSPRegular"
                FontSize="{OnIdiom Phone=14, Tablet=22}" />
            <Image
                HeightRequest="{OnIdiom Phone=112, Tablet=225}"
                WidthRequest="{OnIdiom Phone=138, Tablet=275}"
                Aspect="AspectFit"
                Margin="0"
                Source="{Binding PictureUrl}"
                BackgroundColor="{StaticResource Primary}"/>

            <Label
                x:Name="chamberLabel"
                HorizontalOptions="Center"
                VerticalOptions="Center"
                TextColor="White"
                Margin="5,5,5,0"
                FontFamily="SSPRegular"
                FontSize="{OnIdiom Phone=14, Tablet=22}" />
            <Label
                x:Name="stateLabel"
                HorizontalOptions="Center"
                VerticalOptions="Center"
                TextColor="White"
                Margin="5,0"
                FontFamily="SSPRegular"
                FontSize="{OnIdiom Phone=14, Tablet=22}" />
            <Label
                Text="{Binding Office}"
                HorizontalOptions="Center"
                VerticalOptions="Center"
                HorizontalTextAlignment="Center"
                TextColor="White"
                Margin="5,0"
                FontFamily="SSPRegular"
                LineBreakMode="WordWrap"
                MaxLines="0"
                FontSize="{OnIdiom Phone=14, Tablet=22}" />
            <Label
                Text="{Binding Phone}"
                HorizontalOptions="Center"
                VerticalOptions="Center"
                TextColor="White"
                Margin="5,0,5,5"
                FontFamily="SSPRegular"
                FontSize="{OnIdiom Phone=14, Tablet=22}" />
        </StackLayout>
    </StackLayout>
</ContentView>
```

## 🛠️ Utilities

### StandardSettings.cs
```csharp
// (Truncated excerpt for brevity; see original for full code)
public static class StandardSettings
{
    const string CurrentEventKey = "CurrentEvent";
    public static Event CurrentEvent
    {
        get => JsonConvert.DeserializeObject<Event>(Preferences.Get(CurrentEventKey, string.Empty));
        set => Preferences.Set(CurrentEventKey, JsonConvert.SerializeObject(value));
    }

    // ... other properties and methods ...

    public static string GetBadgeIdForEventCode(string eventCode)
    {
        return RegisteredEvents.ContainsKey(eventCode) ? RegisteredEvents[eventCode] : string.Empty;
    }
}
```

## 📱 Service Utilities

### UnauthorizedTypeEnum.cs
```csharp
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace em3.admin.mobile.Models.Services.Utilities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UnauthorizedTypeEnum
    {
        Unauthorized = 1,
        Awaiting2FA = 2,
        Failed2FA = 3,
        Forbidden = 4,
        InActive = 5,
        WrongCredentials = 6
    }
}
```

## 📦 Models

### BaseCUFIMobileViewModel.cs
```csharp
using SQLite;

namespace cufi.mobile.Models
{
    public interface IBaseCUFIMobileViewModel
    {
        int ParentId { get; set; }
        int Id { get; set; }
        string Title { get; set; }
        DateTime? Date { get; set; }
        DateTime Modified { get; set; }
        DateTime ExpirationDate { get; set; }
    }

    public class BaseCUFIMobileViewModel : IBaseCUFIMobileViewModel
    {
        public int ParentId { get; set; }
        [PrimaryKey]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public DateTime Modified { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
```

### ContentViewModel.cs
```csharp
using Newtonsoft.Json;
using SQLite;

namespace cufi.mobile.Models
{
    public class ContentViewModel : BaseCUFIMobileViewModel, IBaseCUFIMobileViewModel
    {
        public ContentViewModel() { }

        public string ExternalResourceUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string YouTubeVideoID { get; set; }
        public string SubText { get; set; }
        public string SubTitle { get; set; }
        public string ActionButtonText { get; set; }
        public string ViewTemplate { get; set; } = string.Empty;
        public string Route { get; set; }
        public double Width { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; }
        public string HiResUrl { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public string EventGuid { get; set; }
        public string EventCode { get; set; }

        [JsonIgnore, Ignore]
        public string DisplayTitle => Title?.ToUpper();
        [JsonIgnore, Ignore]
        public Command RouteNavCommand { get; set; }
    }
}
```
