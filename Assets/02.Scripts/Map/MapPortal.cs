using UnityEngine;

public class MapPortal : MonoBehaviour, IInteractionable
{
    [SerializeField] private MapType _transMapType;

    public void Interaction(Transform transform)
    {
        Debug.Log("맵 이동");
        //GameManager.Instance.RequestMapChange(_transMapType);
        UIManager.Instance.AddSlotHudInteraction(0, "입장하기", "I", transform, RequestMapChange);
    }

    private void RequestMapChange(string a)
    {
        Debug.Log("벙커 입장");
        GameManager.Instance.RequestMapChange(_transMapType);
        UIManager.Instance.RemoveSlotHudInteraction(0);
    }
}