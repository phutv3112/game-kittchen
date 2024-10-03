using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{

    // Sự kiện toàn cục (static event) được gọi khi bất kỳ đối tượng nào được cắt
    public static event EventHandler OnAnyCut;

    // Phương thức đặt lại dữ liệu tĩnh (sự kiện)
    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    // Sự kiện thông báo khi tiến trình cắt thay đổi
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    // Sự kiện riêng khi đối tượng bị cắt
    public event EventHandler OnCut;

    // Mảng chứa các công thức cắt (các đối tượng có thể được cắt)
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    // Biến theo dõi tiến trình cắt
    private int cuttingProgress;

    // Phương thức tương tác chính giữa người chơi và quầy cắt
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // Không có đối tượng nhà bếp trên quầy
            if (player.HasKitchenObject())
            {
                // Người chơi đang mang một vật thể
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // Người chơi mang theo một vật có thể bị cắt
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);  // Đặt vật thể lên quầy cắt

                    // Đồng bộ hóa hành động đặt vật thể lên quầy
                    InteractLogicPlaceObjectOnCounterServerRpc();
                }
            }
            else
            {
                // Người chơi không mang gì cả
            }
        }
        else
        {
            // Có vật thể trên quầy
            if (player.HasKitchenObject())
            {
                // Người chơi đang mang vật thể khác
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Người chơi đang cầm đĩa
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        // Nếu vật thể có thể được thêm vào đĩa, phá hủy vật thể trên quầy
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
            }
            else
            {
                // Người chơi không cầm gì, nên đưa vật thể từ quầy cho người chơi
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    // Phương thức Server RPC để đồng bộ hành động đặt vật thể lên quầy giữa các client
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();  // Đồng bộ lên client
    }

    // Phương thức Client RPC để đồng bộ trạng thái quầy cắt
    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;  // Đặt lại tiến trình cắt về 0

        // Gọi sự kiện thay đổi tiến trình với tiến trình ban đầu là 0
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }

    // Phương thức thay thế cho tương tác (phụ), dùng để thực hiện hành động cắt
    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            // Nếu có vật thể trên quầy và nó có thể bị cắt
            CutObjectServerRpc();  // Gửi yêu cầu cắt lên server
            TestCuttingProgressDoneServerRpc();  // Kiểm tra xem tiến trình cắt đã hoàn thành chưa
        }
    }

    // Phương thức Server RPC để đồng bộ hành động cắt giữa các client
    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            // Nếu có vật thể và có công thức cắt
            CutObjectClientRpc();  // Đồng bộ hành động cắt lên client
        }
    }

    // Phương thức Client RPC để xử lý cắt vật thể
    [ClientRpc]
    private void CutObjectClientRpc()
    {
        cuttingProgress++;  // Tăng tiến trình cắt lên

        // Gọi sự kiện khi cắt hoàn thành
        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);

        // Lấy công thức cắt tương ứng với vật thể
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

        // Gọi sự kiện thay đổi tiến trình với tiến trình hiện tại
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
        });
    }

    // Phương thức Server RPC để kiểm tra xem tiến trình cắt đã hoàn thành chưa
    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            // Nếu có vật thể và có công thức cắt
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                // Nếu tiến trình cắt đã đạt mức tối đa, chuyển đổi vật thể thành sản phẩm sau khi cắt
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                KitchenObject.DestroyKitchenObject(GetKitchenObject());  // Phá hủy vật thể hiện tại

                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);  // Sinh ra vật thể mới sau khi cắt
            }
        }
    }

    // Phương thức kiểm tra xem vật thể có công thức cắt hay không
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    // Lấy đầu ra (sản phẩm sau khi cắt) cho một vật thể đầu vào
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;  // Trả về sản phẩm sau khi cắt
        }
        else
        {
            return null;
        }
    }

    // Tìm công thức cắt tương ứng với vật thể đầu vào
    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;  // Trả về công thức cắt nếu tìm thấy
            }
        }
        return null;  // Trả về null nếu không tìm thấy
    }
}

