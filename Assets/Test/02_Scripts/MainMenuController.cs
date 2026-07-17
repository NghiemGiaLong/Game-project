using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    public static bool shouldLoadSave = false;
    public static int currentSlot = 1;

    [Header("Giao diện UI")]
    public GameObject slotPanel; // Kéo thả đối tượng SlotPanel vào đây

    // Biến ghi nhớ trạng thái: true = người chơi muốn Tiếp tục, false = muốn Chơi mới
    private bool isContinueMode = false;

    void Start()
    {
        PlayerPrefs.DeleteAll();
        // Đảm bảo lúc vừa vào game, bảng chọn slot luôn bị ẩn
        if (slotPanel != null)
        {
            slotPanel.SetActive(false);
        }
    }

    // 1. Hàm xử lý khi nhấn nút "CHƠI MỚI" ở Menu chính
    public void OpenSlotForNewGame()
    {
        isContinueMode = false; // Đánh dấu là đang muốn chơi mới
        if (slotPanel != null)
        {
            slotPanel.SetActive(true); // Hiện bảng chọn slot lên
        }
    }

    // 2. Hàm xử lý khi nhấn nút "TIẾP TỤC" ở Menu chính
    public void OpenSlotForContinue()
    {
        isContinueMode = true; // Đánh dấu là đang muốn chơi tiếp
        if (slotPanel != null)
        {
            slotPanel.SetActive(true); // Hiện bảng chọn slot lên
        }
    }

    // 3. Hàm xử lý khi nhấn nút "QUAY LẠI" (Đóng bảng Slot)
    public void CloseSlotPanel()
    {
        if (slotPanel != null)
        {
            slotPanel.SetActive(false); // Ẩn bảng chọn slot đi
        }
    }

    // 4. Hàm cốt lõi kích hoạt khi người chơi click vào nút Slot 1, 2 hoặc 3
    public void SelectSlot(int slotNumber)
    {
        currentSlot = slotNumber; // Ghi nhớ số slot được chọn
        string filePath = Path.Combine(Application.persistentDataPath, "save_slot_" + slotNumber + ".json");

        if (isContinueMode)
        {
            // NẾU ĐANG Ở CHẾ ĐỘ TIẾP TỤC: Kiểm tra file lưu có tồn tại không
            if (File.Exists(filePath))
            {
                shouldLoadSave = true;
                SceneManager.LoadScene("Main_Scene");
            }
            else
            {
                Debug.LogWarning($"Slot {slotNumber} đang TRỐNG! Không thể tiếp tục chơi.");
            }
        }
        else
        {
            // NẾU ĐANG Ở CHẾ ĐỘ CHƠI MỚI: Xóa file cũ của riêng slot này và vào game
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Đã xóa file save cũ của Slot {slotNumber} để chơi mới.");
            }

            shouldLoadSave = false;
            SceneManager.LoadScene("Main_Scene");
        }
    }

    //public void OpenSettings()
    //{
    //    Debug.Log("Đang mở bảng Cài Đặt...");
    //}

    public void QuitGame()
    {
        Debug.Log("Đang thoát game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

}
