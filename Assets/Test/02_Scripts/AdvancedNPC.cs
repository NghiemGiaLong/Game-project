using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

// Cấu trúc một câu thoại có kèm theo các lựa chọn phản hồi
[System.Serializable]
public class DialogueNode
{
    public string npcName = "Người Lạ";
    [TextArea(2, 5)]
    public string npcSpeech;

    [Header("Các lựa chọn phản hồi (Tối đa 2)")]
    public string option1Text = "Tạm biệt";
    public string option2Text = "Hỏi câu khác";

    [Header("ID nút tiếp theo khi bấm lựa chọn (0 là kết thúc)")]
    public int nextNodeForOpt1 = 0;
    public int nextNodeForOpt2 = 0;
}

public class AdvancedNPC : MonoBehaviour
{
    [Header("1. Chức năng tự thoại lơ lửng (Cũ)")]
    public TMP_Text floatingText;
    public string floatingSpeech = "Chào cậu! Hôm nay trời đẹp nhỉ?";
    public float typeSpeed = 0.05f;

    [Header("2. Chức năng gợi ý bấm nút E")]
    public GameObject pressEHintUI; // Kéo file chữ "Ấn E để trò chuyện" lơ lửng vào đây

    [Header("3. Chức năng Đối thoại chủ động màn hình (Mới)")]
    public GameObject subtitlePanel;
    public TMP_Text npcNameUI;
    public TMP_Text dialogueMainUI;
    public GameObject optionsPanel;
    public TMP_Text opt1ButtonText;
    public TMP_Text opt2ButtonText;

    [Header("Kịch bản đối thoại (Nhập số câu thoại ở đây)")]
    public DialogueNode[] dialogueTree; // Cây hội thoại cấu hình trực tiếp từ Unity

    private int currentNodeIndex = 0;
    private bool isPlayerNearby = false;
    private bool isTalking = false; // Trạng thái đang trong màn hình đối thoại to
    private Coroutine floatingCoroutine;
    private Coroutine mainDialogueCoroutine;

    void Start()
    {
        if (floatingText != null) floatingText.text = "";
        if (pressEHintUI != null) pressEHintUI.SetActive(false);
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Keyboard.current != null)
        {
            // Nếu đang đứng gần, KHÔNG trong chế độ đối thoại to, và nhấn phím E
            if (!isTalking && Keyboard.current.eKey.wasPressedThisFrame)
            {
                StartActiveDialogue();
            }

            // Tính năng đè giữ Ctrl để tua nhanh chữ (Giao diện to)
            if (isTalking && Keyboard.current.leftCtrlKey.isPressed)
            {
                // Tốc độ tua nhanh x3 được xử lý trực tiếp trong Coroutine dưới
            }
        }
    }

    // --- KÍCH HOẠT ĐỐI THOẠI CHỦ ĐỘNG (BẤM E) --
    void StartActiveDialogue()
    {
        isTalking = true;
        if (pressEHintUI != null) pressEHintUI.SetActive(false); // Ẩn nút gợi ý E đi
        if (floatingText != null) floatingText.text = ""; // Xóa chữ lầm bầm lơ lửng
        if (floatingCoroutine != null) StopCoroutine(floatingCoroutine);

        if (subtitlePanel != null) subtitlePanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        currentNodeIndex = 0; // Bắt đầu từ câu thoại số 0 trong danh sách
        DisplayNode(currentNodeIndex);
    }

    void DisplayNode(int nodeIndex)
    {
        if (nodeIndex < 0 || nodeIndex >= dialogueTree.Length)
        {
            EndDialogue();
            return;
        }

        DialogueNode node = dialogueTree[nodeIndex];
        if (npcNameUI != null) npcNameUI.text = node.npcName;

        if (mainDialogueCoroutine != null) StopCoroutine(mainDialogueCoroutine);
        mainDialogueCoroutine = StartCoroutine(TypeMainDialogue(node));
    }

    IEnumerator TypeMainDialogue(DialogueNode node)
    {
        dialogueMainUI.text = "";
        if (optionsPanel != null) optionsPanel.SetActive(false);

        foreach (char letter in node.npcSpeech.ToCharArray())
        {
            dialogueMainUI.text += letter;
            float currentWait = typeSpeed;
            if (Keyboard.current != null && Keyboard.current.leftCtrlKey.isPressed)
            {
                currentWait = typeSpeed / 3f; // Đè Ctrl tua nhanh x3
            }
            yield return new WaitForSeconds(currentWait);
        }

        // SỬA LỖI TẠI ĐÂY: Nếu CẢ HAI lựa chọn đều dẫn về số 0 (Kết thúc)
        // Thì không thèm hiện bảng OptionsPanel lên nữa, cho phép người chơi bấm Space/E hoặc tự đóng thoại
        if (node.nextNodeForOpt1 == 0 && node.nextNodeForOpt2 == 0)
        {
            // Đợi người chơi nhấn phím E hoặc phím Space một lần nữa để đóng hẳn khung thoại lớn
            yield return new WaitUntil(() => Keyboard.current != null &&
                (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame));

            EndDialogue(); // Tắt khung thoại to, kết thúc trò chuyện
        }
        else
        {
            // Nếu có ID nhảy nhánh thực sự, mới hiển thị bảng lựa chọn lên
            ShowOptions(node);
        }
    }

    void ShowOptions(DialogueNode node)
    {
        // Chỉ hiển thị bảng lựa chọn lên màn hình khi thực sự có dữ liệu
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            if (opt1ButtonText != null) opt1ButtonText.text = node.option1Text;
            if (opt2ButtonText != null) opt2ButtonText.text = node.option2Text;
        }
    }

    // Sự kiện khi người chơi Click chuột chọn Lựa chọn 1
    public void OnSelectOption1()
    {
        int nextNode = dialogueTree[currentNodeIndex].nextNodeForOpt1;
        if (nextNode == 0) EndDialogue();
        else { currentNodeIndex = nextNode; DisplayNode(currentNodeIndex); }
    }

    // Sự kiện khi người chơi Click chuột chọn Lựa chọn 2
    public void OnSelectOption2()
    {
        int nextNode = dialogueTree[currentNodeIndex].nextNodeForOpt2;
        if (nextNode == 0) EndDialogue();
        else { currentNodeIndex = nextNode; DisplayNode(currentNodeIndex); }
    }

    void EndDialogue()
    {
        isTalking = false;
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        // Trả lại trạng thái lầm bầm cũ nếu người chơi vẫn đứng cạnh
        if (isPlayerNearby)
        {
            if (pressEHintUI != null) pressEHintUI.SetActive(true);
            floatingCoroutine = StartCoroutine(TypeFloatingEffect());
        }
    }

    // --- VA CHẠM TRIGGER (GIỮ NGUYÊN TÍNH NĂNG TỰ THOẠI CŨ) ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTalking)
        {
            isPlayerNearby = true;
            if (pressEHintUI != null) pressEHintUI.SetActive(true); // Hiện chữ "Ấn E..."

            if (floatingCoroutine != null) StopCoroutine(floatingCoroutine);
            floatingCoroutine = StartCoroutine(TypeFloatingEffect());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (pressEHintUI != null) pressEHintUI.SetActive(false);
            if (floatingCoroutine != null) StopCoroutine(floatingCoroutine);
            if (mainDialogueCoroutine != null) StopCoroutine(mainDialogueCoroutine);
            EndDialogue();
            if (floatingText != null) floatingText.text = "";
        }
    }

    IEnumerator TypeFloatingEffect()
    {
        floatingText.text = "";
        foreach (char letter in floatingSpeech.ToCharArray())
        {
            floatingText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    void LateUpdate()
    {
        Camera mainCamera = Camera.main;
        // Xoay các khung chữ lơ lửng trên đầu NPC hướng về Camera
        if (mainCamera != null)
        {
            if (floatingText != null && floatingText.text != "")
                floatingText.transform.LookAt(floatingText.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
            if (pressEHintUI != null && pressEHintUI.activeSelf)
                pressEHintUI.transform.LookAt(pressEHintUI.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
    }
}