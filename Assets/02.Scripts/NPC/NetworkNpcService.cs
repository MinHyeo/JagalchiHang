using System.Linq;
using UnityEngine;

public class NetworkNpcService
{
    private NpcViewModel _npcViewModel;

    public void BindInputEvents()
    {
        InputManager.Instance.OnClickNpcUI += OnOpenNpcUI;
    }

    public void UnBindInputEvents()
    {
        InputManager.Instance.OnClickNpcUI -= OnOpenNpcUI;
    }

    // NPC UI 오픈 입력 처리
    private void OnOpenNpcUI()
    {
        if (UIManager.Instance.IsOpenUI(UIType.NpcUI))
        {
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.NpcUI);
            Debug.Log("[NpcUI] N키 클릭 - UI 닫힘");
        }

        else
        {
            UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.NpcUI);
            Debug.Log("[NpcUI] N키 클릭 - UI 열림");
        }
    }

    public NpcViewModel GetNpcViewModel()
    {
        if (_npcViewModel == null)
        {
            var npcViewModel = new NpcViewModel();
            _npcViewModel = npcViewModel;
        }

        return _npcViewModel;
    }

    public void LoadSaveData(NpcSaveModel saveModel)
    {
        if (saveModel == null)
            return;

        _npcViewModel = GetNpcViewModel();
        _npcViewModel.UnlockedNpcIds = saveModel.UnlockedNpcIds.ToHashSet<string>();

        foreach(var id in _npcViewModel.UnlockedNpcIds)
        {
            if (id == "Npc_Bag_01")
               GameUtil.GetNpcManager().SpawnBagNpc(id).Forget();
            if (id == "Npc_Battle_01")
               GameUtil.GetNpcManager().SpawnBattleNpc(id).Forget();
        }
    }
}
