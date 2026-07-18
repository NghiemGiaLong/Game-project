using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class NPCDialogue : MonoBehaviour
{
    [Header("Cấu hình Lời Thoại")]
    public TMP_Text dialogueText; // Kéo file DialogueText vào đây

    [TextArea(3, 10)]
    public string fullText = "Chào cậu! Hôm nay trời đẹp nhỉ? Hãy cẩn thận với những cạm bẫy phía trước nhé!";

    [Header("Tốc độ hiển thị gốc")]
    public float typeSpeed = 0.06f; // Tốc độ chạy chữ bình thường (giây/ký tự)

    private Coroutine typingCoroutine;
    private bool isPlayerNearby = false;

    void Start()
    {
        // Làm rỗng chữ lúc vừa vào game
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            // Bắt đầu hiệu ứng chạy chữ
            typingCoroutine = StartCoroutine(TypeTextEffect());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            // Xóa trắng chữ đi khi người chơi đi ra xa
            if (dialogueText != null)
            {
                dialogueText.text = "";
            }
        }
    }

    // Coroutine cốt lõi xử lý việc tua nhanh tốc độ
    IEnumerator TypeTextEffect()
    {
        dialogueText.text = "";

        foreach (char letter in fullText.ToCharArray())
        {
            dialogueText.text += letter; // Thêm từng chữ cái vào ô hiển thị

            // Thiết lập tốc độ chờ mặc định ban đầu
            float currentWaitTime = typeSpeed;

            // KIỂM TRA ĐÈ PHÍM: Nếu người chơi đang ĐÈ GIỮ phím Left Ctrl
            if (Keyboard.current != null && Keyboard.current.leftCtrlKey.isPressed)
            {
                currentWaitTime = typeSpeed / 3f; // Tua nhanh tốc độ chạy chữ lên gấp 3 lần
            }

            // Nghỉ một khoảng thời gian tương ứng rồi mới in chữ tiếp theo
            yield return new WaitForSeconds(currentWaitTime);
        }
    }

    void LateUpdate()
    {
        Camera mainCamera = Camera.main;
        // Ép trực tiếp chính khung chữ xoay về camera
        if (mainCamera != null && dialogueText != null && dialogueText.text != "")
        {
            dialogueText.transform.LookAt(dialogueText.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
    }
}
