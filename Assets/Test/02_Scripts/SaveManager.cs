using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameSaveData
{
    public float playerX;
    public float playerY;
    public float playerZ;
    public int hasSaveData;

    // THÊM: Mảng chứa danh sách ID các vật phẩm ĐÃ BỊ NHẶT trong Scene này
    public string[] pickedUpItemIDs;
}

public class SaveManager : MonoBehaviour
{
    public Transform playerTransform;

    // Danh sách lưu tạm thời các ID vật phẩm đã bị người chơi nhặt trong màn chơi này
    public static System.Collections.Generic.List<string> globalPickedUpItems = new System.Collections.Generic.List<string>();

    private string GetSaveFilePath(int slotNumber)
    {
        return Path.Combine(Application.persistentDataPath, "save_slot_" + slotNumber + ".json");
    }
    [Header("Cấu hình Đồ họa")]
    public UnityEngine.UI.Image brightnessOverlay;
    // HÀM LƯU THÀNH FILE JSON
    public void SaveGame()
    {
        if (playerTransform == null) return;

        int slot = MainMenuController.currentSlot;
        string filePath = GetSaveFilePath(slot);

        GameSaveData data = new GameSaveData();
        data.playerX = playerTransform.position.x;
        data.playerY = playerTransform.position.y;
        data.playerZ = playerTransform.position.z;
        data.hasSaveData = 1;

        // Đưa danh sách các vật phẩm đã nhặt vào file lưu JSON
        data.pickedUpItemIDs = globalPickedUpItems.ToArray();

        string jsonText = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, jsonText);

        Debug.Log($"ĐÃ LƯU THÀNH FILE JSON CHO SLOT {slot}: {filePath}");
    }

    // HÀM ĐỌC TỪ FILE JSON
    public void LoadGame()
    {
        int slot = MainMenuController.currentSlot;
        string filePath = GetSaveFilePath(slot);

        if (File.Exists(filePath))
        {
            string jsonText = File.ReadAllText(filePath);
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(jsonText);

            if (data.hasSaveData == 1)
            {
                // 1. Đọc vị trí nhân vật
                if (playerTransform != null)
                {
                    playerTransform.position = new Vector3(data.playerX, data.playerY, data.playerZ);
                }

                // 2. Nạp lại danh sách vật phẩm đã nhặt để các script ItemPickup tự quét và xóa bỏ lúc vào game
                globalPickedUpItems = new System.Collections.Generic.List<string>(data.pickedUpItemIDs);
            }

            Debug.Log($"ĐÃ TẢI FILE JSON THÀNH CÔNG CHO SLOT {slot}!");
        }
    }

    void Start()
    {
        if (brightnessOverlay != null)
        {
            // Đọc giá trị đã lưu (mặc định là 1.0 nếu chưa từng chỉnh)
            float savedBrightness = PlayerPrefs.GetFloat("GameBrightness", 1f);

            Color overlayColor = Color.black;

            // Áp dụng đúng logic tính toán bóng tối như ngoài Menu
            if (savedBrightness < 1.0f)
            {
                float darknessPercentage = (1.0f - savedBrightness) * 1.7f;
                overlayColor.a = Mathf.Clamp(darknessPercentage, 0f, 0.85f);
            }
            else
            {
                overlayColor.a = 0f;
            }

            brightnessOverlay.color = overlayColor;
            Debug.Log("Đã đồng bộ độ sáng thành công sang Main_Scene: " + savedBrightness);
        }

        // Khi bắt đầu chơi mới từ Menu, reset lại danh sách vật phẩm đã nhặt
        if (!MainMenuController.shouldLoadSave)
        {
            globalPickedUpItems.Clear();
        }
        else
        {
            LoadGame();
        }
    }
    public void GoToMainMenu()
    {
        Debug.Log("Đang quay về Main Menu...");
        SceneManager.LoadScene("MainMenu");
    }

    // Hàm 2: Tắt luôn game (Đóng ứng dụng)
    public void QuitGameCompletely()
    {
        Debug.Log("Đang đóng ứng dụng game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Tắt nút Play khi test
#else
        Application.Quit(); // Đóng cửa sổ game khi xuất file .exe
#endif
    }
}
