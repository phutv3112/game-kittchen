using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Có thể chứa các thuộc tính và phương thức chung cho nhiều loại quầy khác nhau.
public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{

    // Sự kiện tĩnh (static event) được gọi khi bất kỳ đối tượng nào được đặt lên quầy
    public static event EventHandler OnAnyObjectPlacedHere;

    // Phương thức đặt lại dữ liệu tĩnh (reset sự kiện) khi cần
    public static void ResetStaticData()
    {
        OnAnyObjectPlacedHere = null;
    }

    // Khai báo biến chứa điểm trên mặt quầy (được gán từ Unity Editor)
    [SerializeField] private Transform counterTopPoint;

    // Biến lưu trữ đối tượng nhà bếp hiện đang ở trên quầy
    private KitchenObject kitchenObject;

    // Phương thức tương tác với người chơi, có thể được ghi đè trong lớp con
    public virtual void Interact(Player player)
    {
        Debug.LogError("BaseCounter.Interact();");  // Ghi lại lỗi để debug
    }

    // Phương thức tương tác thay thế, có thể được ghi đè trong lớp con (hiện không có hành động)
    public virtual void InteractAlternate(Player player)
    {
        //Debug.LogError("BaseCounter.InteractAlternate();");
    }

    // Lấy vị trí trên quầy (Transform) để theo dõi đối tượng nhà bếp
    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    // Đặt đối tượng nhà bếp lên quầy, đồng thời kích hoạt sự kiện nếu có đối tượng được đặt
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        // Nếu đối tượng không null, kích hoạt sự kiện thông báo
        if (kitchenObject != null)
        {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }

    // Lấy đối tượng nhà bếp hiện tại trên quầy
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    // Xóa đối tượng nhà bếp khỏi quầy (đặt lại thành null)
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    // Kiểm tra xem có đối tượng nhà bếp trên quầy hay không
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    // Lấy đối tượng mạng (NetworkObject) để đồng bộ hóa trong môi trường nhiều người chơi
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

}
