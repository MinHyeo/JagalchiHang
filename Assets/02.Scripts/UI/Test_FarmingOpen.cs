using UnityEngine;

public class Test_FarmingOpen : MonoBehaviour
{
    [SerializeField] private UIButton Button_First;
    [SerializeField] private UIButton Button_Second;


    private void OnEnable()
    {
        Button_First.BindOnClickButtonEvent(OnClickOpenFarmingFirst);
        Button_Second.BindOnClickButtonEvent(OnClickOpenFarmingSecond);
    }

    private void OnClickOpenFarmingFirst()
    {
        if (UIManager.Instance.IsOpenUI(UIType.FarmingUI))
        {
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.FarmingUI);
        }
        else
        {
            UIManager.Instance.OpenFarmingUI(1);
        }
    }

    private void OnClickOpenFarmingSecond()
    {
        if (UIManager.Instance.IsOpenUI(UIType.FarmingUI))
        {
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.FarmingUI);
        }
        else
        {
            UIManager.Instance.OpenFarmingUI(2);
        }
    }
}
