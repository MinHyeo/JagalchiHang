using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class NetworkManager_re : MonoBehaviour
{
    public static NetworkManager_re Inst { get; set; }
    public NetworkPlayerService PlayerService { get; private set; }
    public NetworkInventoryService InventoryService { get; private set; }
    public NetworkFarmingService FarmingService { get; private set; }
    public NetworkStorageService StorageService { get; private set; }
    public NetworkNpcService NpcService { get; private set; }

    public event Action<NpcViewModel> OnNpcSpawnDataReceived;

    private void Awake()
    {
        Inst = this;
        InitNetworkService();
    }

    private void InitNetworkService()
    {
        // 앞으로 네트워크 매니저에서 사용할 다양한 서비스를 생성
        PlayerService = new NetworkPlayerService();
        InventoryService = new NetworkInventoryService();
        FarmingService = new NetworkFarmingService();
        StorageService = new NetworkStorageService();
        NpcService = new NetworkNpcService();
    }

    public void RequestCreateLocalPlayer()
    {
        // 게임 시작이나, 맵 진입 시 로컬 플레이어를 서버에 생성하는 요청
        //var localPlayerVm = LocalPlayerService.CreateLocalPlayerViewModel();

        // 응답 받았다고 가정한다 = 추후 실제 서버 통신시에는 람다나 비동기 로직으로 받아온다
        // OnRecvCreateLocalPlayer(localPlayerVm);
    }

    //public void OnRecvCreateLocalPlayer(DNLocalPlayerViewModel localPlayerVm)
    //{
    //    DaniTechGameObjectManager.Inst.CreateLocalPlayer(localPlayerVm);
    //}

    // 파일 저장 경로 설정 (C:/Users/이름/.../projectName/save.json)
    private string GetPath()
    {
        return Path.Combine(Application.persistentDataPath, "DaniTechSaveData.json");
    }

    //// 세이브 기능 구현
    //public void RequstSaveData(DaniTechPlayerModel data)
    //{
    //    // prettyPrint = true는 JSON을 보기 좋게 정렬
    //    string json = JsonUtility.ToJson(data, true);
    //    File.WriteAllText(GetPath(), json); // 파일 쓰기는 상당한 비용이 소모됨!
    //    Debug.Log($"저장 완료: {GetPath()}");
    //}

    //// 로드 기능
    //public DaniTechPlayerModel RequstLoadSaveData()
    //{
    //    string path = GetPath();
    //    if (File.Exists(path))
    //    {
    //        string json = File.ReadAllText(path);
    //        DaniTechPlayerModel data = JsonUtility.FromJson<DaniTechPlayerModel>(json);
    //        Debug.Log("데이터를 불러왔습니다.");
    //        return data;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("세이브 파일이 없습니다. 새 데이터를 생성합니다.");
    //        var playerData = GetDefaultPlayerData();
    //        RequstSaveData(GetDefaultPlayerData());
    //        return playerData;
    //    }
    //}

    //public DaniTechPlayerModel GetDefaultPlayerData()
    //{
    //    var newPlayerData = new DaniTechPlayerModel();
    //    newPlayerData.PlayerName = "NoName";
    //    newPlayerData.PlayerTotalExp = 0;
    //    return newPlayerData;
    //}

    public void RequestMoveItem_InvenToFarming(int invenIdx, int farmingIdx, string boxUniqueId)
    {
        var invenVm = InventoryService.GetLocalInventoryViewModel();
        var farmingVm = FarmingService.LoadFarmingBox(boxUniqueId);

        if (!invenVm.InventorySlots.ContainsKey(invenIdx) || !farmingVm.FarmingSlots.ContainsKey(farmingIdx)) return;

        var invenSlot = invenVm.InventorySlots[invenIdx];
        var farmingSlot = farmingVm.FarmingSlots[farmingIdx];

        if (invenSlot.ItemDataId == farmingSlot.ItemDataId && invenSlot.IsStackable)
        {
            int maxCount = invenSlot.MaxCount;
            int remainCount = maxCount - invenSlot.ItemStackCount;

            if (remainCount > 0) 
            {
                int moveAmount = Mathf.Min(remainCount, farmingSlot.ItemStackCount);

                invenSlot.ItemStackCount += moveAmount;
                farmingSlot.ItemStackCount -= moveAmount;

                if (farmingSlot.ItemStackCount < 0)
                {
                    farmingSlot.Clear();
                }

                return;
            }
        }

        long tempUniqueId = invenSlot.ItemUniqueId;
        string tempId = invenSlot.ItemDataId;
        int tempCount = invenSlot.ItemStackCount;

        invenSlot.ItemUniqueId = farmingSlot.ItemUniqueId;
        invenSlot.SetItem(farmingSlot.ItemDataId, farmingSlot.ItemStackCount);

        farmingSlot.ItemUniqueId = tempUniqueId;
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
        var StoragegSlot = storageVm.StorageSlots[storageIdx];

        if (invenSlot.ItemDataId == StoragegSlot.ItemDataId && invenSlot.IsStackable)
        {
            int maxCount = invenSlot.MaxCount;
            int remainCount = maxCount - invenSlot.ItemStackCount;

            if (remainCount > 0)
            {
                int moveAmount = Mathf.Min(remainCount, StoragegSlot.ItemStackCount);

                invenSlot.ItemStackCount += moveAmount;
                StoragegSlot.ItemStackCount -= moveAmount;

                if (StoragegSlot.ItemStackCount < 0)
                {
                    StoragegSlot.Clear();
                }

                return;
            }
        }

        long tempUniqueId = invenSlot.ItemUniqueId;
        string tempId = invenSlot.ItemDataId;
        int tempCount = invenSlot.ItemStackCount;

        invenSlot.ItemUniqueId = StoragegSlot.ItemUniqueId;
        invenSlot.SetItem(StoragegSlot.ItemDataId, StoragegSlot.ItemStackCount);

        StoragegSlot.ItemUniqueId = tempUniqueId;
        StoragegSlot.SetItem(tempId, tempCount);

        // TODO: 추후 세이브 필요
        // RequestSaveData();
    }

    public void RequestLoadNpcData()
    {
        //SaveData saveData = new SaveData();
        NpcViewModel npcViewModel = NpcService.GetNpcViewModel();

        OnNpcSpawnDataReceived?.Invoke(npcViewModel);
    }
}
