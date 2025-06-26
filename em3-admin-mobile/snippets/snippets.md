---

## 👋 Project Philosophy & Leadership Notes

The examples in this portfolio are intentionally simple.

We work on real software for real businesses, and that means we prioritize clarity, maintainability, and practical value. There is a time and place for generics, reflection, and high levels of abstraction—but the reality is that most small to mid-sized projects don’t benefit from overusing those tools. Worse, many projects suffer when complex designs are implemented without strong documentation, disciplined conventions, or clear reasoning.

What you’ll find here are the kinds of straightforward patterns and utilities that help small teams move fast without accumulating technical debt. They reflect the leadership mindset we bring to every engagement: help your team grow, build what’s needed, and leave things better than you found them.

This isn’t just about writing code—it’s about setting the tone, raising the bar, and making good software sustainable.

---

### 🔧 `MauiProgram.cs` – Dependency Injection & App Configuration

```csharp
// MauiProgram.cs – Service registration and DI setup
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Register services
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
```

---

### 🧭 `AppShell.xaml` – Shell Navigation Structure

```xml
<!-- AppShell.xaml – Shell navigation structure -->
<Shell xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       Shell.FlyoutBehavior="Disabled">

    <!-- Register navigation routes -->
    <ShellContent
        Title="Home"
        ContentTemplate="{DataTemplate local:MainPage}" />

</Shell>
```

---

### ✅ `ReferenceNumberValidationHelpers.cs` – Domain-Specific Validation

```csharp
// ReferenceNumberValidationHelpers.cs – Sample of domain-specific validation logic
public static class ReferenceNumberValidationHelpers
{
    public static bool IsValidReference(string reference)
    {
        // Simple example: must be 8 digits
        return !string.IsNullOrEmpty(reference) &&
               reference.Length == 8 &&
               reference.All(char.IsDigit);
    }
}
```

---

### 🧰 `TypeHelpers.cs` – Type Casting Utility

```csharp
// TypeHelpers.cs – Utility method for safe type casting with null fallback
public static class TypeHelpers
{
    public static T? As<T>(this object obj) where T : class
    {
        // Safe cast that returns null instead of throwing
        return obj as T;
    }
}
```

---

### 🧱 `TypeSafeEnum.cs` – Base Class for Type-Safe Enums

```csharp
// TypeSafeEnum.cs – Generic pattern for typesafe enums with value comparison
public abstract class TypeSafeEnum<T> where T : TypeSafeEnum<T>
{
    public string Value { get; }

    protected TypeSafeEnum(string value)
    {
        Value = value;
    }

    public override string ToString() => Value;

    public override bool Equals(object obj)
    {
        return obj is TypeSafeEnum<T> other && Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();
}
```

---

### 🏷️ `NameableTypeSafeEnum.cs` – Named Extension of TypeSafeEnum

```csharp
// NameableTypeSafeEnum.cs – Extension of TypeSafeEnum with a friendly Name
public abstract class NameableTypeSafeEnum<T> : TypeSafeEnum<T> where T : NameableTypeSafeEnum<T>
{
    public string Name { get; }

    protected NameableTypeSafeEnum(string value, string name) : base(value)
    {
        Name = name;
    }

    public override string ToString() => Name;
}
```

---

### 🧩 `BaseViewModel.cs` – Core MVVM ViewModel Logic

```csharp
// BaseViewModel.cs – Provides common MVVM properties like IsBusy and error handling
public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    bool isBusy = false;
    public bool IsBusy
    {
        get => isBusy;
        set
        {
            if (isBusy != value)
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }
    }

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
```

---

### 📋 `BaseListViewModel.cs` – Reusable Generic List Support

```csharp
// BaseListViewModel.cs – Generic list management with observable collection
public abstract class BaseListViewModel<TItem> : BaseViewModel
{
    public ObservableCollection<TItem> Items { get; } = new();

    private TItem selectedItem;
    public TItem SelectedItem
    {
        get => selectedItem;
        set
        {
            selectedItem = value;
            OnPropertyChanged(nameof(SelectedItem));
        }
    }

    public virtual async Task LoadItemsAsync()
    {
        // To be overridden in derived classes for custom data loading
        Items.Clear();
    }
}
```

---

### 📷 `BaseScannerViewModel.cs` – Extendable Barcode Scanner Handling

```csharp
// BaseScannerViewModel.cs – Base for barcode scanner-based view models
public abstract class BaseScannerViewModel : BaseViewModel
{
    public string ScannedCode { get; set; }

    public virtual void OnScan(string scannedValue)
    {
        ScannedCode = scannedValue;
        // Handle scanned result (e.g., lookup, validate, navigate)
    }
}
```

---

### 🏁 `KioskCheckinPage.xaml` – Touch-Friendly UI

```xml
<!-- KioskCheckinPage.xaml – Touch-friendly check-in UI for kiosk mode -->
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             x:Class="KioskCheckinPage"
             BackgroundColor="White">

    <StackLayout Padding="20" Spacing="24" VerticalOptions="Center">
        <Label Text="Welcome to the Event"
               FontSize="28"
               HorizontalOptions="Center" />

        <Entry Placeholder="Enter Appointment Code"
               x:Name="AppointmentCodeEntry" />

        <Button Text="Check In"
                Command="{Binding CheckInCommand}" />
    </StackLayout>
</ContentPage>
```

---

### 🎯 `KioskCheckinPage.xaml.cs` – ViewModel-Driven Check-In Logic

```csharp
// KioskCheckinPage.xaml.cs – Handles appointment check-in from kiosk
public partial class KioskCheckinPage : ContentPage
{
    public KioskCheckinPage()
    {
        InitializeComponent();
        BindingContext = new KioskCheckinViewModel();
    }
}

// ViewModel – Contains the command logic for check-in
public class KioskCheckinViewModel : BaseViewModel
{
    public ICommand CheckInCommand { get; }

    public KioskCheckinViewModel()
    {
        CheckInCommand = new Command(OnCheckIn);
    }

    private void OnCheckIn()
    {
        // Simulate validation and transition
        if (IsBusy) return;
        IsBusy = true;

        // Placeholder: Validate appointment code and navigate
        IsBusy = false;
    }
}
```
