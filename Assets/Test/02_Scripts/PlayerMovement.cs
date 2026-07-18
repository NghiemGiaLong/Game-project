using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;        // Tốc độ di chuyển ngang
    public float turnSpeed = 10f;       // Tốc độ xoay mặt mượt mà

    private float idleTimer = 0f;       // Bộ đếm thời gian đứng yên
    private bool isFacingRight = true;  // Lưu hướng nhìn cuối cùng (để biết đang đứng quay về bên nào)
    private Quaternion targetRotation;  // Góc xoay mục tiêu mà Cube cần hướng tới

    void Start()
    {
        // Lúc đầu game, mặc định nhìn sang phải
        targetRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            float moveX = 0f;

            // Nhận phím di chuyển Trái / Phải
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;

            // Kiểm tra xem người chơi có đang bấm nút di chuyển hay không
            if (moveX != 0f)
            {
                // ---- TRẠNG THÁI DI CHUYỂN ----
                idleTimer = 0f; // Reset bộ đếm thời gian đứng yên về 0 ngay lập tức

                // Thực hiện di chuyển dọc trục X
                Vector3 movement = new Vector3(moveX, 0f, 0f).normalized;
                transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

                // Xác định hướng xoay mục tiêu khi đang chạy
                if (moveX > 0f)
                {
                    isFacingRight = true;
                    targetRotation = Quaternion.Euler(0f, 0f, 0f); // Quay sang phải
                }
                else
                {
                    isFacingRight = false;
                    targetRotation = Quaternion.Euler(0f, 180f, 0f); // Quay sang trái
                }
            }
            else
            {
                // ---- TRẠNG THÁI ĐỨNG YÊN (IDLE) ----
                idleTimer += Time.deltaTime; // Tăng dần thời gian đứng yên theo giây thật

                // Nếu đứng yên đủ từ 1 giây trở lên
                if (idleTimer >= 1f)
                {
                    // Ép góc xoay mục tiêu hướng thẳng về phía Camera (Góc Y = 90 độ)
                    targetRotation = Quaternion.Euler(0f, 90f, 0f);
                }
            }

            // Thực hiện xoay mượt mà (Lerp) từ góc hiện tại tới góc mục tiêu
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }
}
