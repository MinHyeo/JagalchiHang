using UnityEngine;

public class Test_Craft : MonoBehaviour
{
    [SerializeField] private UIButton Button;

    private void OnEnable()
    {
        Button.BindOnClickButtonEvent(OnClickOpenCraft);
    }

    private void OnClickOpenCraft()
    {
        if (UIManager.Instance.IsOpenUI(UIType.CraftUI))
        {
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.CraftUI);
        }
        else
        {
            UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.CraftUI);
        }
    }
}
