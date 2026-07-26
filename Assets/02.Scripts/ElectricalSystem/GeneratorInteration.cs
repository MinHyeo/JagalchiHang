using UnityEngine;

public class GeneratorInteration : MonoBehaviour, IInteractionable
{
    private int _uniqueId;
    public int UniqueId => _uniqueId;

    private void Start()
    {
        _uniqueId = (int)GameUtil.GenerateUniqueId();
    }

    public void Interaction(Transform transform)
    {
        UIManager.Instance.AddSlotHudInteraction(_uniqueId, "발전기", "Click", transform, OpenBoxUI);
    }

    private void OpenBoxUI(string text)
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.GeneratorUI);
        UIManager.Instance.RemoveSlotHudInteraction(_uniqueId);
    }
}
