using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{

    // Hằng số chứa tên trigger dùng cho animation "Cut"
    private const string CUT = "Cut";

    // Tham chiếu tới đối tượng CuttingCounter nơi sự kiện OnCut sẽ xảy ra
    [SerializeField] private CuttingCounter cuttingCounter;

    // Biến lưu trữ Animator để điều khiển các hoạt cảnh (animations)
    private Animator animator;

    // Được gọi khi đối tượng được khởi tạo, khởi tạo thành phần Animator
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Được gọi khi script bắt đầu chạy
    private void Start()
    {
        // Đăng ký sự kiện OnCut để khi sự kiện xảy ra, gọi phương thức CuttingCounter_OnCut
        cuttingCounter.OnCut += CuttingCounter_OnCut;
    }

    // Được gọi khi sự kiện OnCut được kích hoạt từ CuttingCounter
    private void CuttingCounter_OnCut(object sender, System.EventArgs e)
    {
        // Kích hoạt trigger "Cut" để bắt đầu chạy animation cắt
        animator.SetTrigger(CUT);
    }

}
