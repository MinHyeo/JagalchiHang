using UnityEngine;

public class NetworkNpcService
{
    private NpcViewModel _npcViewModel;

    public NetworkNpcService()
    {
        InputManager.Instance.OnClickNpcUI += OnOpenNpcUI;
    }

    ~NetworkNpcService()
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
}
