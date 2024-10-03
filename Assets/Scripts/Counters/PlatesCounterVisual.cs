using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Quản lý việc hiển thị hình ảnh (visual) của các đĩa (plates) trên counter.
// MonoBehaviour có thể sử dụng các chức năng đặc biệt như xử lý sự kiện, cập nhật mỗi frame, và xử lý các sự kiện của game như va chạm hoặc input từ người chơi.
public class PlatesCounterVisual : MonoBehaviour
{

    // Tham chiếu đến đối tượng PlatesCounter để lắng nghe sự kiện sinh và lấy đĩa
    [SerializeField] private PlatesCounter platesCounter;
    // Vị trí trên counter nơi các đĩa sẽ xuất hiện
    [SerializeField] private Transform counterTopPoint;
    // Prefab của đĩa để tạo các đối tượng hình ảnh của đĩa
    [SerializeField] private Transform plateVisualPrefab;

    // Danh sách lưu trữ các GameObject đĩa hiện đang hiển thị
    private List<GameObject> plateVisualGameObjectList;

    // Phương thức Awake được gọi khi đối tượng này được khởi tạo
    private void Awake()
    {
        // Khởi tạo danh sách lưu trữ các GameObject của đĩa
        plateVisualGameObjectList = new List<GameObject>();
    }

    // Phương thức Start được gọi ở frame đầu tiên khi đối tượng này hoạt động
    private void Start()
    {
        // Đăng ký lắng nghe sự kiện sinh và lấy đĩa từ PlatesCounter
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemoved;
    }

    // Phương thức này được gọi khi một đĩa bị lấy đi
    private void PlatesCounter_OnPlateRemoved(object sender, System.EventArgs e)
    {
        // Lấy đĩa cuối cùng trong danh sách
        GameObject plateGameObject = plateVisualGameObjectList[plateVisualGameObjectList.Count - 1];
        // Xóa đĩa đó khỏi danh sách
        plateVisualGameObjectList.Remove(plateGameObject);
        // Hủy đối tượng GameObject của đĩa để xóa nó khỏi scene
        Destroy(plateGameObject);
    }

    // Phương thức này được gọi khi một đĩa mới được sinh ra
    private void PlatesCounter_OnPlateSpawned(object sender, System.EventArgs e)
    {
        // Tạo đối tượng hình ảnh của đĩa từ prefab tại vị trí trên counter
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);

        // Đặt vị trí của đĩa, xếp chồng theo chiều dọc dựa trên số lượng đĩa hiện tại
        float plateOffsetY = .1f; // Độ cao của mỗi đĩa
        plateVisualTransform.localPosition = new Vector3(0, plateOffsetY * plateVisualGameObjectList.Count, 0);

        // Thêm đối tượng đĩa mới vào danh sách quản lý
        plateVisualGameObjectList.Add(plateVisualTransform.gameObject);
    }

}
