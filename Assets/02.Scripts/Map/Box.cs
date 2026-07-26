using UnityEngine;

public class Box : MonoBehaviour, IInteractionable
{
    private int _uniqueId;
    public int UniqueId => _uniqueId;

    private void Start()
    {
        _uniqueId = (int)GameUtil.GenerateUniqueId();
    }

    public void Interaction(Transform transform)
    {
        UIManager.Instance.AddSlotHudInteraction(_uniqueId, "열기", "Click", transform, OpenBoxUI);
    }

    private void OpenBoxUI(string dd)
    {
        UIManager.Instance.OpenFarmingUI(_uniqueId);
        UIManager.Instance.RemoveSlotHudInteraction(_uniqueId);
    }
}