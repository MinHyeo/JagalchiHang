using UnityEngine;

public class NetworkPlayerService_re : MonoBehaviour
{
    private InventorySlotViewModel _localPlayerInventorySlotViewModel;


    // 인벤토리 슬롯
    public InventorySlotViewModel GetLocalPlayerInventorySlotViewModel()
    {
        if (_localPlayerInventorySlotViewModel == null)
        {
            // TODO : 필요한지 생각해보기, 게임데이터매니저 필요
        }

        return _localPlayerInventorySlotViewModel;
    }

}
