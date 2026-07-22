using UnityEngine;

public class Box : MonoBehaviour, IInteractionable
{
    public void Interaction(Transform transform)
    {
        UIManager.Instance.AddSlotHudInteraction(1, "열기", "I", transform, OpenBoxUI);
    }

    private void OpenBoxUI(string dd)
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.FarmingUI);
    }
}