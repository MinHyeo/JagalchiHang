using UnityEngine;

public class MapPortal : MonoBehaviour, IInteractionable
{
    private int _uniqueId;
    public int UniqueId => _uniqueId;

    [SerializeField] private MapType _transMapType;

    private void Start()
    {
        _uniqueId = (int)GameUtil.GenerateUniqueId();
    }

    public void Interaction(Transform transform)
    {
        Debug.Log("맵 이동");
        UIManager.Instance.AddSlotHudInteraction(_uniqueId, "입장하기", "I", transform, RequestMapChange);
    }

    private void RequestMapChange(string a)
    {
        Debug.Log("벙커 입장");
        WorldManager worldManager = GameUtil.GetWorldManager();
        worldManager.TransMap(_transMapType);

        UIManager.Instance.RemoveSlotHudInteraction(_uniqueId);
    }
}