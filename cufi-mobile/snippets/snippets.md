# 📦 Code Snippets Overview

This document contains representative code snippets from the CUFI Mobile project, organized by category. Each snippet includes a short explanation of what it does and why it’s useful.

---

## 🔧 Controls

### AppointmentsCollectionView.xaml.cs

> ✅ A custom content view that binds to `EventContentViewModel`. It sets the local `Model` when the binding context changes, which is handy for giving the code-behind access to bound data while still supporting MVVM.

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

---

### CongressCollectionView.xaml

> 🎨 A highly visual data template showing member info (like name, image, office, phone). Uses responsive `OnIdiom` layout hints and FFImageLoading for performance. Great example of rich content binding in MAUI.

```xml
<ContentView ...>
  <StackLayout x:DataType="model:EventContentViewModel" ...>
    <StackLayout ... BackgroundColor="{StaticResource Primary}">
      <Label Text="{Binding FormalName}" ... />
      <Image Source="{Binding PictureUrl}" ... />
      <Label x:Name="chamberLabel" ... />
      <Label x:Name="stateLabel" ... />
      <Label Text="{Binding Office}" ... />
      <Label Text="{Binding Phone}" ... />
    </StackLayout>
  </StackLayout>
</ContentView>
```

---

## 🛠️ Utilities

### StandardSettings.cs

> ⚙️ A central place to manage shared app state, like current event/session/badge. It wraps the `Preferences` API in static properties, making it much easier to read and write persistent values. Great for cross-page continuity.

```csharp
public static class StandardSettings
{
    const string CurrentEventKey = "CurrentEvent";
    public static Event CurrentEvent
    {
        get => JsonConvert.DeserializeObject<Event>(Preferences.Get(CurrentEventKey, string.Empty));
        set => Preferences.Set(CurrentEventKey, JsonConvert.SerializeObject(value));
    }

    // ... other properties and helpers ...

    public static string GetBadgeIdForEventCode(string eventCode)
    {
        return RegisteredEvents.ContainsKey(eventCode) ? RegisteredEvents[eventCode] : string.Empty;
    }
}
```

---

## 📱 Service Utilities

### UnauthorizedTypeEnum.cs

> 🚫 An enum for identifying the reason a login or auth check failed. This is JSON-convertible via `StringEnumConverter`, which helps make API responses cleaner and more understandable than just using numeric codes.

```csharp
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
```

---

## 📦 Models

### BaseCUFIMobileViewModel.cs

> 🧱 The foundational data structure for your view models. It includes core metadata (ID, title, timestamps) and is tied to SQLite storage via `[PrimaryKey]`. Interfaces allow for flexible reuse and testing.

```csharp
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
```

---

### ContentViewModel.cs

> 🖼️ A full-featured view model for app content. Includes support for navigation (`RouteNavCommand`), rich media (images, YouTube), and layout control (priority, size, view template). Also uses `[JsonIgnore]` and `[SQLite.Ignore]` wisely.

```csharp
public class ContentViewModel : BaseCUFIMobileViewModel, IBaseCUFIMobileViewModel
{
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
```
