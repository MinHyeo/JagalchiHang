using UnityEngine;

public class MapPortal : MonoBehaviour, IInteractionable
{
    [SerializeField] private MapType _transMapType;

    public void Interaction()
    {
        Debug.Log("맵 이동");
        GameManager.Instance.RequestMapChange(_transMapType);
    }
}