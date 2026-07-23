using UnityEngine;

public class HousePortal : MonoBehaviour, IInteractionable
{
    [SerializeField] private Transform TeleportPos;

    private int _uniqueId;
    public int UniqueId { get; }

    private void Start()
    {
        _uniqueId = (int)GameUtil.GenerateUniqueId();
    }

    public void Interaction(Transform transform)
    {
        UIManager.Instance.AddSlotHudInteraction(_uniqueId, "입장하기", "I", transform, RequestHouseInOut);
    }

    private void RequestHouseInOut(string a)
    {
        PlayerManager playerManager = GameUtil.GetPlayerManager();
        playerManager.TransPlayerPosition(TeleportPos.position);

        UIManager.Instance.RemoveSlotHudInteraction(_uniqueId);
    }
}