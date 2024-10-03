using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Xử lý việc giao món ăn từ người chơi đến hệ thống giao hàng.
public class DeliveryCounter : BaseCounter
{

    // Singleton instance để đảm bảo chỉ có một đối tượng DeliveryCounter
    public static DeliveryCounter Instance { get; private set; }

    // Được gọi khi đối tượng được tạo, gán instance cho singleton
    private void Awake()
    {
        Instance = this;
    }

    // Ghi đè phương thức Interact để xử lý tương tác của người chơi với quầy giao hàng
    public override void Interact(Player player)
    {
        // Kiểm tra nếu người chơi đang cầm món ăn (KitchenObject)
        if (player.HasKitchenObject())
        {
            // Kiểm tra xem món ăn người chơi cầm có phải là đĩa (Plate) hay không
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                // Chỉ chấp nhận đĩa (Plates) để giao hàng

                // Gửi đĩa này tới DeliveryManager để xử lý việc giao hàng
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

                // Sau khi giao hàng, hủy món ăn (KitchenObject) khỏi tay người chơi
                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            }
        }
    }

}
