using UnityEngine;

public class Test_Setting : MonoBehaviour
{
    [SerializeField] private UIButton Button;

    private void OnEnable()
    {
        Button.BindOnClickButtonEvent(OnClickOpenInventory);
    }

    private void OnClickOpenInventory()
    {
        if (UIManager.Instance.IsOpenUI(UIType.SettingUI))
        {
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.SettingUI);
        }
        else
        {
            UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.SettingUI);
        }
    }


}
