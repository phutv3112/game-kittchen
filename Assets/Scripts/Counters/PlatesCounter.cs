using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{

    // Sự kiện kích hoạt khi sinh ra một đĩa
    public event EventHandler OnPlateSpawned;
    // Sự kiện kích hoạt khi một đĩa bị lấy đi
    public event EventHandler OnPlateRemoved;

    // Biến lưu trữ thông tin về đối tượng đĩa
    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    // Bộ đếm thời gian để sinh đĩa
    private float spawnPlateTimer;
    // Thời gian tối đa giữa các lần sinh đĩa
    private float spawnPlateTimerMax = 4f;
    // Số lượng đĩa hiện tại đã được sinh ra
    private int platesSpawnedAmount;
    // Số lượng tối đa đĩa có thể sinh ra
    private int platesSpawnedAmountMax = 4;

    // Phương thức được gọi mỗi khung hình (frame)
    private void Update()
    {
        // Chỉ chạy trên máy chủ (Server)
        if (!IsServer)
        {
            return;
        }

        // Cập nhật bộ đếm thời gian
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            // Đã đủ thời gian để sinh đĩa mới
            spawnPlateTimer = 0f;

            // Kiểm tra nếu trò chơi đang hoạt động và số lượng đĩa chưa đạt mức tối đa
            if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax)
            {
                // Sinh đĩa mới
                SpawnPlateServerRpc();
            }
        }
    }

    // Phương thức gọi từ máy chủ để sinh đĩa
    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        // Gọi trên tất cả các client để sinh đĩa
        SpawnPlateClientRpc();
    }

    // Phương thức sinh đĩa cho tất cả các client
    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        // Tăng số lượng đĩa đã sinh ra
        platesSpawnedAmount++;

        // Kích hoạt sự kiện sinh đĩa
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    // Phương thức tương tác với người chơi
    public override void Interact(Player player)
    {
        // Kiểm tra nếu người chơi không cầm vật phẩm nào
        if (!player.HasKitchenObject())
        {
            // Kiểm tra nếu có ít nhất một đĩa trên counter
            if (platesSpawnedAmount > 0)
            {
                // Sinh đĩa mới cho người chơi
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                // Xử lý logic tương tác trên máy chủ
                InteractLogicServerRpc();
            }
        }
    }

    // Phương thức logic tương tác gọi từ máy chủ
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        // Gọi trên tất cả các client để cập nhật việc lấy đĩa
        InteractLogicClientRpc();
    }

    // Phương thức logic tương tác gọi từ client
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        // Giảm số lượng đĩa đã sinh ra
        platesSpawnedAmount--;

        // Kích hoạt sự kiện lấy đĩa
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
