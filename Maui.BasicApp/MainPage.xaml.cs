namespace Maui.BasicApp;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();

        WatchBattery();
    }

    private void WatchBattery()
    {
        Battery.Default.BatteryInfoChanged += Battery_BatteryInfoChanged;
    }

    private void Battery_BatteryInfoChanged(object sender, BatteryInfoChangedEventArgs e)
    {
        BatteryStateLabel.Text = e.State switch
        {
            BatteryState.Charging => "Battery is currently charging",
            BatteryState.Discharging => "Charger is not connected and the battery is discharging",
            BatteryState.Full => "Battery is full",
            BatteryState.NotCharging => "The battery isn't charging.",
            BatteryState.NotPresent => "Battery is not available.",
            BatteryState.Unknown => "Battery is unknown",
            _ => "Battery is unknown"
        };

        BatteryLevelLabel.Text = $"Battery is {e.ChargeLevel * 100}% charged.";
    }

    private void HapticShortButton_Clicked(object sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
    }

    private void HapticLongButton_Clicked(object sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
    }

    private void VibrateStartButton_Clicked(object sender, EventArgs e)
    {
        int secondsToVibrate = Random.Shared.Next(1, 7);
        TimeSpan vibrationLength = TimeSpan.FromSeconds(secondsToVibrate);

        Vibration.Default.Vibrate(vibrationLength);
    }

    private void VibrateStopButton_Clicked(object sender, EventArgs e)
    {
        Vibration.Default.Cancel();
    }
}

