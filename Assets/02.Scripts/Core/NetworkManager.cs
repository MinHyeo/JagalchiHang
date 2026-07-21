using System.IO;
using UnityEngine;

public class NetworkManager : SingletonBase<NetworkManager>
{
    public NetworkPlayerService PlayerService { get; private set; }
    public NetworkInventoryService InventoryService { get; private set; }
    public NetworkFarmingService FarmingService { get; private set; }
    public NetworkStorageService StorageService { get; private set; }
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

        NpcService = new NetworkNpcService();
        NpcService.BindInputEvents();
    }

    public void RequestMoveItem_InvenToFarming(int invenIdx, int farmingIdx)
    {
        var invenVm = InventoryService.GetLocalInventoryViewModel();
        var farmingVm = FarmingService.GetFarmingViewModel();

        if (!invenVm.InventorySlots.ContainsKey(invenIdx) || !farmingVm.FarmingSlots.ContainsKey(farmingIdx)) return;

        var invenSlot = invenVm.InventorySlots[invenIdx];
        var farmingSlot = farmingVm.FarmingSlots[farmingIdx];

        string tempId = invenSlot.ItemDataId;
        int tempCount = invenSlot.ItemStackCount;

        invenSlot.SetItem(farmingSlot.ItemDataId, farmingSlot.ItemStackCount);
        farmingSlot.SetItem(tempId, tempCount);

        // TODO: 추후 세이브 필요
        // RequestSaveData();
    }

    public void RequestMoveItem_InvenToStorage(int invenIdx, int storageIdx)
    {
        var invenVm = InventoryService.GetLocalInventoryViewModel();
        var storageVm = StorageService.GetLocalStorageViewModel();

        if (!invenVm.InventorySlots.ContainsKey(invenIdx) || !storageVm.StorageSlots.ContainsKey(storageIdx)) return;

        var invenSlot = invenVm.InventorySlots[invenIdx];
        var farmingSlot = storageVm.StorageSlots[storageIdx];

        string tempId = invenSlot.ItemDataId;
        int tempCount = invenSlot.ItemStackCount;

        invenSlot.SetItem(farmingSlot.ItemDataId, farmingSlot.ItemStackCount);
        farmingSlot.SetItem(tempId, tempCount);

        // TODO: 추후 세이브 필요
        // RequestSaveData();
    }

    public void AddItemToInventory(string itemDataId, int stackCount)
    {
        InventoryService.AddItem(itemDataId, stackCount);
    }
}