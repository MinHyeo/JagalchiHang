using UnityEngine;

public class Test_FarmingOpen : MonoBehaviour
{
    [SerializeField] private UIButton Button;

    private void OnEnable()
    {
        Button.BindOnClickButtonEvent(OnClickOpenFarming);
    }

    private void OnClickOpenFarming()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.FarmingUI);
    }
}
