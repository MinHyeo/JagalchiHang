using UnityEngine;

public class ItemController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log($"이 {this}을 줍고싶다면 'E' 키를 누르세요.");

            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController == null) return;

            playerController.SetCurrentItem(this);
        }
    }

    public void DestroyItem()
    {
        Destroy(this.gameObject);
    }
}
