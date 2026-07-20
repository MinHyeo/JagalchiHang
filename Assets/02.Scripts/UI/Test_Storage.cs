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
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.StorageUI);
    }
}
