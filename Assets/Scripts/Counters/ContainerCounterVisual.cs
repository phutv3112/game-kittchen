using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{

    // Chuỗi hằng chứa tên trigger cho Animator đóng mở tủ
    private const string OPEN_CLOSE = "OpenClose";

    // Tham chiếu đến đối tượng ContainerCounter (gán từ Unity Editor)
    [SerializeField] private ContainerCounter containerCounter;

    // Biến lưu trữ Animator để điều khiển các hoạt ảnh
    private Animator animator;

    // Phương thức Awake được gọi khi đối tượng khởi tạo, gán Animator
    private void Awake()
    {
        animator = GetComponent<Animator>();  // Lấy Animator từ component
    }

    // Phương thức Start được gọi khi bắt đầu, đăng ký sự kiện cho containerCounter
    private void Start()
    {
        containerCounter.OnPlayerGrabbedObject += ContainerCounter_OnPlayerGrabbedObject;
        // Đăng ký sự kiện OnPlayerGrabbedObject của containerCounter
    }

    // Phương thức được gọi khi sự kiện OnPlayerGrabbedObject xảy ra
    private void ContainerCounter_OnPlayerGrabbedObject(object sender, System.EventArgs e)
    {
        animator.SetTrigger(OPEN_CLOSE);  // Kích hoạt trigger "OpenClose" để chạy hoạt ảnh
    }

}
