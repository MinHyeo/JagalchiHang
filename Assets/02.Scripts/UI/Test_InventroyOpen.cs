using UnityEngine;

public class Test_InventroyOpen : MonoBehaviour
{
    [SerializeField] private UIButton Button;

    private void OnEnable()
    {
        Button.BindOnClickButtonEvent(OnClickOpenInventory);
    }

    private void OnClickOpenInventory()
    {
        if (UIManager.Instance.IsOpenUI(UIType.InventoryUI))
        {
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.InventoryUI);
        }
        else
        {
            UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.InventoryUI);
        }
    }
}
