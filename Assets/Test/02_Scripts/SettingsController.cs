using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("Giao diện UI")]
    public GameObject settingsPanel;
    public TMP_Dropdown screenModeDropdown;
    public TMP_Dropdown aspectRatioDropdown;
    public Slider brightnessSlider;
    public Image brightnessOverlay; // Kéo file BrightnessOverlay vào đây

    void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Đăng ký sự kiện khi người chơi thay đổi các giá trị trên UI
        if (screenModeDropdown != null) screenModeDropdown.onValueChanged.AddListener(SetScreenMode);
        if (aspectRatioDropdown != null) aspectRatioDropdown.onValueChanged.AddListener(SetAspectRatio);
        if (brightnessSlider != null) brightnessSlider.onValueChanged.AddListener(SetBrightness);

        // Tải lại cấu hình độ sáng cũ nếu đã từng chỉnh
        if (brightnessSlider != null)
        {
            float savedBrightness = PlayerPrefs.GetFloat("GameBrightness", 1f);
            brightnessSlider.value = savedBrightness;
            SetBrightness(savedBrightness);
        }
    }

    // 1. Hàm mở/đóng bảng Cài Đặt
    public void OpenSettings() { if (settingsPanel != null) settingsPanel.SetActive(true); }
    public void CloseSettings() { if (settingsPanel != null) settingsPanel.SetActive(false); }

    // 2. Hàm chỉnh Chế độ màn hình (Full, Cửa sổ, Không viền)
    public void SetScreenMode(int index)
    {
        switch (index)
        {
            case 0: // Full Screen
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.fullScreen = true;
                break;
            case 1: // Windowed
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.fullScreen = false;
                break;
            case 2: // Borderless Window
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                Screen.fullScreen = true;
                break;
        }
        Debug.Log("Đã đổi chế độ màn hình!");
    }

    // 3. Hàm chỉnh Tỉ lệ khung hình (Aspect Ratio)
    public void SetAspectRatio(int index)
    {
        int width = Screen.width;
        int height = Screen.height;

        switch (index)
        {
            case 0: // 16:9
                height = Mathf.RoundToInt(width * 9f / 16f);
                break;
            case 1: // 16:10
                height = Mathf.RoundToInt(width * 10f / 16f);
                break;
            case 2: // 4:3
                height = Mathf.RoundToInt(width * 3f / 4f);
                break;
        }

        // Áp dụng độ phân giải mới dựa theo tỉ lệ đã chọn
        Screen.SetResolution(width, height, Screen.fullScreenMode);
        Debug.Log($"Đã đổi tỉ lệ màn hình sang độ phân giải: {width}x{height}");
    }

    // 4. Hàm chỉnh Sáng/Tối bằng cách thay đổi độ Alpha của tấm màn đen
    //public void SetBrightness(float value)
    //{
    //    if (brightnessOverlay == null) return;

    //    Color overlayColor = brightnessOverlay.color;

    //    // Nếu thanh kéo nhỏ hơn 1 (Tối đi): Tăng độ đậm của màn đen bao phủ
    //    if (value < 1f)
    //    {
    //        overlayColor.a = (1f - value) * 0.8f; // Tối đa che 80% màn hình để không bị đen xì hoàn toàn
    //    }
    //    // Nếu thanh kéo lớn hơn 1 (Sáng lên): Đổi tấm màn sang màu trắng và tăng độ mờ để giả lập lóa sáng
    //    else
    //    {
    //        overlayColor = Color.white;
    //        overlayColor.a = (value - 1f) * 0.4f;
    //    }

    //    brightnessOverlay.color = overlayColor;
    //    PlayerPrefs.SetFloat("GameBrightness", value); // Lưu lại cấu hình độ sáng
    //}
    public void SetBrightness(float value)
    {
        if (brightnessOverlay == null) return;

        // Giữ nguyên tấm màn màu đen, chỉ thay đổi độ trong suốt (Alpha)
        Color overlayColor = Color.black;

        // Giá trị Slider chạy từ 0.5 (Tối) đến 1.5 (Sáng). Mức chuẩn ở giữa là 1.0.
        // Nếu kéo về bên trái (nhỏ hơn 1), game sẽ tối dần.
        if (value < 1f)
        {
            overlayColor.a = (1f - value) * 0.8f; // Tối tối đa che 80% màn hình
        }
        // Nếu kéo về bên phải (lớn hơn hoặc bằng 1), màn hình trong suốt hoàn toàn (Sáng bình thường)
        else
        {
            overlayColor.a = 0f;
        }

        brightnessOverlay.color = overlayColor;
        PlayerPrefs.SetFloat("GameBrightness", value); // Lưu lại cấu hình độ sáng vào máy
    }

}
