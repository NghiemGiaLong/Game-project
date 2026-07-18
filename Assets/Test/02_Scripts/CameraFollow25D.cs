using UnityEngine;

public class CameraFollow25D : MonoBehaviour
{
    [Header("Mục tiêu bám theo")]
    public Transform target;          // Kéo khối Player vào đây

    [Header("Cấu hình Khoảng Cách")]
    public float deadzoneX = 3f;      // Chiều rộng vùng an toàn (nhân vật đi trong khoảng này cam không di chuyển)
    public float smoothSpeed = 5f;    // Độ mượt mà khi camera đuổi theo nhân vật

    [Header("Giới hạn trục Y và Z")]
    private float fixedY;
    private float fixedZ;

    void Start()
    {
        if (target != null)
        {
            // Ghi nhớ vị trí chiều cao (Y) và khoảng cách sâu (Z) ban đầu của Camera
            fixedY = transform.position.y;
            fixedZ = transform.position.z;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Tính toán khoảng cách hiện tại giữa Camera và Nhân vật theo trục X
        float targetX = transform.position.x;
        float deltaX = target.position.x - transform.position.x;

        // 2. Kiểm tra xem nhân vật có vượt quá vùng Deadzone sang bên PHẢI không
        if (deltaX > deadzoneX)
        {
            targetX = target.position.x - deadzoneX;
        }
        // 3. Kiểm tra xem nhân vật có vượt quá vùng Deadzone sang bên TRÁI không
        else if (deltaX < -deadzoneX)
        {
            targetX = target.position.x + deadzoneX;
        }

        // 4. Tính toán vị trí mục tiêu mới cho Camera (Giữ nguyên Y và Z để chuẩn game 2.5D)
        Vector3 targetPosition = new Vector3(targetX, fixedY, fixedZ);

        // 5. Di chuyển camera mượt mà từ vị trí cũ sang vị trí mới (Lerp)
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

    // Mẹo vẽ khung trực quan trong tab Scene để bạn dễ căn chỉnh độ rộng của vùng Deadzone
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position;
        if (target != null) center.y = target.position.y;

        // Vẽ 2 đường thẳng màu đỏ thể hiện biên giới của vùng Deadzone
        Gizmos.DrawLine(new Vector3(transform.position.x - deadzoneX, center.y - 5f, transform.position.z + 10f), new Vector3(transform.position.x - deadzoneX, center.y + 5f, transform.position.z + 10f));
        Gizmos.DrawLine(new Vector3(transform.position.x + deadzoneX, center.y - 5f, transform.position.z + 10f), new Vector3(transform.position.x + deadzoneX, center.y + 5f, transform.position.z + 10f));
    }
}
