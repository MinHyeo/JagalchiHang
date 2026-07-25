using UnityEngine;

public class FloorTrigger : MonoBehaviour
{
    [SerializeField] private int _floorNumber;
    [SerializeField] private BuildingVisibility _visibility;
    [SerializeField] private BuildingFloorVisibility _floorVisibility;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            _visibility.SetPlayerInside(true, _floorNumber);
        }

        return;
    }
}
