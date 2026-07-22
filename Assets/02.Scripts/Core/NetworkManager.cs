using System.IO;
using UnityEngine;

public class NetworkManager : SingletonBase<NetworkManager>
{
    public NetworkPlayerService PlayerService { get; private set; }
    public NetworkInventoryService InventoryService { get; private set; }
    public NetworkFarmingService FarmingService { get; private set; }
    public NetworkStorageService StorageService { get; private set; }
    public NetworkCraftService CraftService { get; private set; }
    public NetworkNpcService NpcService { get; private set; }

    private string GetSaveFilePath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"saveData{slotIndex}.json");
    }

    public void SaveGame(int slotIndex, SaveModel saveModel)
    {
        string saveFilePath = GetSaveFilePath(slotIndex);
        string jsonText = JsonUtility.ToJson(saveModel, true);

        File.WriteAllText(saveFilePath, jsonText);
        Debug.Log($"저장 완료: {saveFilePath}");
    }

    public SaveModel LoadGame(int slotIndex)
    {
        string saveFilePath = GetSaveFilePath(slotIndex);

        if (File.Exists(saveFilePath) == false)
            return null;

        string jsonText = File.ReadAllText(saveFilePath);
        SaveModel saveModel = JsonUtility.FromJson<SaveModel>(jsonText);

        return saveModel;
    }

    public void InitNetworkService()
    {
        // 앞으로 네트워크 매니저에서 사용할 다양한 서비스를 생성
        PlayerService = new NetworkPlayerService();
        InventoryService = new NetworkInventoryService();
        FarmingService = new NetworkFarmingService();
        StorageService = new NetworkStorageService();

    }

    public void RequestMoveItem_InvenToFarming(int invenIdx, int farmingIdx, string boxUniqueId)
    {
        var invenSlot = InventoryService.GetLocalInventoryViewModel().InventorySlots[invenIdx];
        var farmingSlot = FarmingService.LoadFarmingBox(boxUniqueId).FarmingSlots[farmingIdx];

        MoveOrSwapSlots(invenSlot, farmingSlot);
    }

    public void RequestMoveItem_FarmingToInven(int farmingIdx, int invenIdx, string boxUniqueId)
    {
        var farmingSlot = FarmingService.LoadFarmingBox(boxUniqueId).FarmingSlots[farmingIdx];
        var invenSlot = InventoryService.GetLocalInventoryViewModel().InventorySlots[invenIdx];

        MoveOrSwapSlots(farmingSlot, invenSlot);
    }

    public void RequestMoveItem_StorageToInven(int storageIdx, int invenIdx)
    {
        var storageSlot = StorageService.GetLocalStorageViewModel().StorageSlots[storageIdx];
        var invenSlot = InventoryService.GetLocalInventoryViewModel().InventorySlots[invenIdx];

        MoveOrSwapSlots(storageSlot, invenSlot);
    }

    public void RequestMoveItem_InvenToStorage(int invenIdx, int storageIdx)
    {
        var invenSlot = InventoryService.GetLocalInventoryViewModel().InventorySlots[invenIdx];
        var storageSlot = StorageService.GetLocalStorageViewModel().StorageSlots[storageIdx];

        MoveOrSwapSlots(invenSlot, storageSlot);
    }

    public void MoveOrSwapSlots(ISlotViewModel startSlot, ISlotViewModel endSlot)
    {
        if (startSlot == null || endSlot == null) return;
        if (string.IsNullOrEmpty(startSlot.ItemDataId)) return;

        if (!string.IsNullOrEmpty(endSlot.ItemDataId) &&
            startSlot.ItemDataId == endSlot.ItemDataId &&
            startSlot.IsStackable)
        {
            int maxCount = endSlot.MaxCount;
            int slotCountLeft = maxCount - endSlot.ItemStackCount;

            if (slotCountLeft > 0)
            {
                int moveAmount = Mathf.Min(slotCountLeft, startSlot.ItemStackCount);

                endSlot.ItemStackCount += moveAmount;
                startSlot.ItemStackCount -= moveAmount;

                if (startSlot.ItemStackCount <= 0)
                {
                    startSlot.Clear();
                }
                return;
            }
        }

        long tempUniqueId = startSlot.ItemUniqueId;
        string tempId = startSlot.ItemDataId;
        int tempCount = startSlot.ItemStackCount;

        startSlot.ItemUniqueId = endSlot.ItemUniqueId;
        startSlot.SetItem(endSlot.ItemDataId, endSlot.ItemStackCount);

        endSlot.ItemUniqueId = tempUniqueId;
        endSlot.SetItem(tempId, tempCount);
    }

    public void AddItemToInventory(string itemDataId, int stackCount)
    {
        InventoryService.AddItem(itemDataId, stackCount);
    }
}