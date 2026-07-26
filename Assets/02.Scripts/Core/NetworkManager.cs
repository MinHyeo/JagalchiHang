using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NetworkManager : SingletonBase<NetworkManager>
{
    public NetworkSaveLoadService SaveLoadService { get; private set; }
    public NetworkPlayerService PlayerService { get; private set; }
    public NetworkInventoryService InventoryService { get; private set; }
    public NetworkFarmingService FarmingService { get; private set; }
    public NetworkStorageService StorageService { get; private set; }
    public NetworkCraftService CraftService { get; private set; }
    public NetworkNpcService NpcService { get; private set; }
    public NetworkGeneratorService GeneratorService { get; private set; }
    public NetworkSettingService SettingService { get; private set; }

    public NetworkFarmService FarmService { get; private set; }


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

    public SaveModel RequestSaveGame(int slotIndex)
    {
        SaveModel currentSaveData = new SaveModel();
        currentSaveData.ItemSaveModel = new List<ItemSaveModel>();

        if (PlayerService != null) currentSaveData.PlayerSaveModel = PlayerService.GetSaveData();
        if (InventoryService != null)
        {
            var invenData = InventoryService.GetSaveData();
            currentSaveData.ItemSaveModel.AddRange(invenData);
        }
        if (StorageService != null)
        {
            var storageData = StorageService.GetSaveData();
            currentSaveData.ItemSaveModel.AddRange(storageData);
        }

        if (FarmService != null)
        {
            currentSaveData.FarmSaveModel = FarmService.GetSaveData();
        }

        float time = TimeManager.Instance.Time;
        int day = TimeManager.Instance.Day;
        currentSaveData.Time = time;
        currentSaveData.Day = day;

        var mapManager = GameUtil.GetMapManager();
        currentSaveData.MapType = mapManager.CurrentMapType;

        var unlockedNpcIds = NpcService.GetNpcViewModel().UnlockedNpcIds;
        currentSaveData.NpcSaveModel = new NpcSaveModel();
        currentSaveData.NpcSaveModel.UnlockedNpcIds = unlockedNpcIds.ToList<string>();

        SaveGame(slotIndex, currentSaveData);

        return currentSaveData;
    }

    public void RequestLoadGame(SaveModel saveModel)
    {
        PlayerService.LoadSaveData(saveModel.PlayerSaveModel);
        InventoryService.LoadSaveData(saveModel.ItemSaveModel);
        StorageService.LoadSaveData(saveModel.ItemSaveModel);
        NpcService.LoadSaveData(saveModel.NpcSaveModel);
        TimeManager.Instance.SetTime(saveModel.Day, saveModel.Time);
        TimeManager.Instance.RestartTime();

        if (saveModel.FarmSaveModel != null)
        {
            FarmService.LoadSaveData(saveModel.FarmSaveModel);
        }
    }

    public void InitSaveLoadService()
    {
        SaveLoadService = new NetworkSaveLoadService();
    }

    public void InitNetworkService()
    {
        // 앞으로 네트워크 매니저에서 사용할 다양한 서비스를 생성
        PlayerService = new NetworkPlayerService();
        InventoryService = new NetworkInventoryService();
        FarmingService = new NetworkFarmingService();
        StorageService = new NetworkStorageService();
        NpcService = new NetworkNpcService();
        CraftService = new NetworkCraftService();
        GeneratorService = new NetworkGeneratorService();
        SettingService = new NetworkSettingService();
        FarmService = new NetworkFarmService();

        NpcService.BindInputEvents();
        InventoryService.BindInventoryInputEvent();
        SettingService.BindSettingInputEvent();
        CraftService.BindCraftInputEvent();
    }

    public void DestroyNetworkService()
    {
        NpcService.UnBindInputEvents();
        InventoryService.UnBindInventoryInputEvent();
        SettingService.UnBindSettingInputEvent();
        CraftService.UnBindCraftInputEvent();

        PlayerService = null;
        InventoryService = null;
        FarmingService = null;
        StorageService = null;
        NpcService = null;
        CraftService = null;
        GeneratorService = null;
        SettingService = null;
        FarmService = null;
    }

    public void RequestMoveItem_InvenToFarming(int invenIdx, int farmingIdx, int boxUniqueId)
    {
        var invenSlot = InventoryService.GetLocalInventoryViewModel().InventorySlots[invenIdx];
        var farmingSlot = FarmingService.LoadFarmingBox(boxUniqueId).FarmingSlots[farmingIdx];

        MoveOrSwapSlots(invenSlot, farmingSlot);
    }

    public void RequestMoveItem_FarmingToInven(int farmingIdx, int invenIdx, int boxUniqueId)
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