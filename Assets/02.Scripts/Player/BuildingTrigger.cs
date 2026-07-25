using UnityEngine;

public class BuildingTrigger : MonoBehaviour
{
    [SerializeField] private BuildingVisibility _visibility;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            _visibility.SetPlayerInside(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            _visibility.SetPlayerInside(false);
        }
    }
}
