using UnityEngine;

public class TestPlayer : MonoBehaviour
{ 
    // 테스트용 플레이어 코드 

    public float moveSpeed = 5.0f;
    public float jumpPower;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v);

        dir.Normalize();

        transform.position += dir * moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse); //무게 적용하고 순간적인 힘 가하기 
        }
    }
}
