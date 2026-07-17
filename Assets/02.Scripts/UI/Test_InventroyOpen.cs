using UnityEngine;

public class Test_InventroyOpen : MonoBehaviour
{
    [SerializeField] UIButton Button;

    private void OnEnable()
    {
        Button.BindOnClickButtonEvent(OnClickOpenInventory);
    }

    private void OnClickOpenInventory()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.InventoryUI);
    }
}
