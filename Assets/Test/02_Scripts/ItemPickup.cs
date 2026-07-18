using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ItemPickup : MonoBehaviour
{
    [Header("Cấu hình UI")]
    public GameObject pickupTextUI;

    [Header("Định danh vật phẩm (Bắt buộc duy nhất)")]
    public string itemID; // Ví dụ: item_1, item_2, item_3

    private bool isPlayerNearby = false;

    void Start()
    {
        // CHUẨN JSON MỚI: Kiểm tra trong danh sách tổng của SaveManager xem vật phẩm này đã bị nhặt chưa
        if (SaveManager.globalPickedUpItems != null && SaveManager.globalPickedUpItems.Contains(itemID))
        {
            Destroy(gameObject); // Nếu đã nhặt trong file JSON, xóa nó đi luôn
            return;
        }

        if (pickupTextUI != null)
        {
            pickupTextUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerNearby && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            PickUp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (pickupTextUI != null) pickupTextUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (pickupTextUI != null) pickupTextUI.SetActive(false);
        }
    }

    void PickUp()
    {
        Debug.Log("Đã nhặt vật phẩm thành công: " + itemID);

        // CHUẨN JSON MỚI: Thêm ID vật phẩm này vào danh sách đã nhặt của SaveManager
        if (SaveManager.globalPickedUpItems != null)
        {
            SaveManager.globalPickedUpItems.Add(itemID);
        }

        if (pickupTextUI != null) pickupTextUI.SetActive(false);
        Destroy(gameObject);
    }
}
