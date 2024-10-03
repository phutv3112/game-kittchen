using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter {


    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // Không có KitchenObject ở đây
            if (player.HasKitchenObject()) {
                // Người chơi đang mang theo thứ gì đó
                player.GetKitchenObject().SetKitchenObjectParent(this);
            } else {
                // Player không mang theo bất kỳ cái gì
            }
        } else {
            // Có KitchenObject ở đây
            if (player.HasKitchenObject()) {
                // Player đang mang thứ gì đó
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    // Player đang cầm đĩa
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                } else {
                    // Player không cầm đĩa nhưng cầm thứ khác
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        // Counter đang có đĩa
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        }
                    }
                }
            } else {
                // Player đang không cầm bất kỳ cái gì
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}