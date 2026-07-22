using UnityEngine;

public class Test_Storage : MonoBehaviour
{
    [SerializeField] private UIButton _button;

    private void OnEnable()
    {
        _button.BindOnClickButtonEvent(OnClickOpenStorage);
    }

    private void OnClickOpenStorage()
    {
        if (UIManager.Instance.IsOpenUI(UIType.StorageUI))
        {
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.StorageUI);
        }
        else
        {
            UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.StorageUI);
        }
    }
}
