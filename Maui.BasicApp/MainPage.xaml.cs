namespace Maui.BasicApp;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();

        ReadDeviceInfo();

        ReadDeviceDisplay();

        WatchBattery();
    }

    /// <summary>
    /// 디바이스 정보 가져오기
    /// </summary>
    private void ReadDeviceInfo()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.AppendLine($"Model: {DeviceInfo.Current.Model}");
        sb.AppendLine($"Manufacturer: {DeviceInfo.Current.Manufacturer}");
        sb.AppendLine($"Name: {DeviceInfo.Current.Name}");
        sb.AppendLine($"OS Version: {DeviceInfo.Current.VersionString}");
        sb.AppendLine($"Idiom: {DeviceInfo.Current.Idiom}");
        sb.AppendLine($"Platform: {DeviceInfo.Current.Platform}");

        bool isVirtual = DeviceInfo.Current.DeviceType switch
        {
            DeviceType.Physical => false,
            DeviceType.Virtual => true,
            _ => false
        };

        sb.AppendLine($"Virtual device? {isVirtual}");

        DisplayDeviceLabel.Text = sb.ToString();
    }

    /// <summary>
    /// 디스플레이 정보 가져오기
    /// </summary>
    private void ReadDeviceDisplay()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.AppendLine($"Pixel width: {DeviceDisplay.Current.MainDisplayInfo.Width} / Pixel Height: {DeviceDisplay.Current.MainDisplayInfo.Height}");
        sb.AppendLine($"Density: {DeviceDisplay.Current.MainDisplayInfo.Density}");
        sb.AppendLine($"Orientation: {DeviceDisplay.Current.MainDisplayInfo.Orientation}");
        sb.AppendLine($"Rotation: {DeviceDisplay.Current.MainDisplayInfo.Rotation}");
        sb.AppendLine($"Refresh Rate: {DeviceDisplay.Current.MainDisplayInfo.RefreshRate}");

        DisplayDetailsLabel.Text = sb.ToString();
    }

    /// <summary>
    /// 배터리 정보 가져오기
    /// </summary>
    private void WatchBattery()
    {
        Battery.Default.BatteryInfoChanged += Battery_BatteryInfoChanged;
    }

    /// <summary>
    /// 배터리 상태 변경시 발생 이벤트
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// 슬립모드 빠질지 여부
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AlwaysOnSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        DeviceDisplay.Current.KeepScreenOn = AlwaysOnSwitch.IsToggled;
    }

    /// <summary>
    /// 짧은 Haptic
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HapticShortButton_Clicked(object sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
    }

    /// <summary>
    /// 긴 Haptic
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HapticLongButton_Clicked(object sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
    }

    /// <summary>
    /// 진동 발생
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void VibrateStartButton_Clicked(object sender, EventArgs e)
    {
        // 5 초마다 진동
        Vibration.Default.Vibrate(TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// 진동 취소
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void VibrateStopButton_Clicked(object sender, EventArgs e)
    {
        Vibration.Default.Cancel();
    }

    /// <summary>
    /// 플래시 켜고 끄기
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void FlashlightSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        try
        {
            if (FlashlightSwitch.IsToggled)
            {
                await Flashlight.Default.TurnOnAsync();
            }
            else
            {
                await Flashlight.Default.TurnOffAsync();
            }
        }
        catch (FeatureNotSupportedException ex)
        {
            // Handle not supported on device exception
        }
        catch (PermissionException ex)
        {
            // Handle permission exception
        }
        catch (Exception ex)
        {
            // Unable to turn on/off flashlight
        }
    }

    private async void textShareButton_Clicked(object sender, EventArgs e)
    {
        await ShareText(this.shareEntry.Text);
    }

    private async void uriShareButton_Clicked(object sender, EventArgs e)
    {
        await ShareUri(this.shareEntry.Text, Share.Default);
    }

    public async Task ShareText(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = text,
            Title = "Share Text"
        });
    }

    public async Task ShareUri(string uri, IShare share)
    {
        if (string.IsNullOrEmpty(uri)) return;

        await share.RequestAsync(new ShareTextRequest
        {
            Uri = uri,
            Title = "Share Web Link"
        });
    }

    public async Task ShareFile()
    {
        string fn = "Attachment.txt";
        string file = Path.Combine(FileSystem.CacheDirectory, fn);

        File.WriteAllText(file, "Hello World");

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share text file",
            File = new ShareFile(file)
        });
    }
}

