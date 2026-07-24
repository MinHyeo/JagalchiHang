using UnityEngine;

public class FloorTrigger : MonoBehaviour
{
    [SerializeField] private int _floorNumber;
    [SerializeField] private BuildingVisibility _visibility;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            _visibility.SetPlayerFloor(_floorNumber);
        }

        return;
    }
}
