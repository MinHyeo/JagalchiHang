using UnityEngine;

public class TestPlayer : MonoBehaviour
{ 
    // 테스트용 플레이어 코드 

    public float moveSpeed = 5.0f;
    public float jumpPower;

    private Rigidbody _rb;

    [SerializeField] private GameObject currentTargetMonster;

    [SerializeField] private LayerMask monsterLayer;

    [SerializeField] private GameObject NpcManageUI;

    [SerializeField] private float attackRange = 10.0f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        NpcManageUI.SetActive(false); // 어차피 테스트용 코드라 SetActive 사용 

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public GameObject GetPlayerTarget() // 플레이어가 어떤 몬스터를 때렸는지 이 함수가 필요함(테스트용)
    {
        return currentTargetMonster;
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

        //==========================================

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (NpcManageUI.activeSelf == true)
            {
                NpcManageUI.SetActive(false);
            }

            else
            {
                NpcManageUI.SetActive(true);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }


        // ============================================
        if (Input.GetKeyDown(KeyCode.T))
        {
            FindNearMonster();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currentTargetMonster != null)
            {
                Debug.Log("[TestPlayer] 타겟 몬스터 제거 ");
                currentTargetMonster = null;
            }
        }
    }

   private void FindNearMonster() // 테스트용 코드 
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, monsterLayer);

        currentTargetMonster = null;

        float closeDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);

            if (distance < closeDistance) 
            {
                closeDistance = distance;
                currentTargetMonster = hit.gameObject;
            }
        }

        if(currentTargetMonster != null)
        {
            Debug.Log($"[TestPlayer] 가장 가까운 몬스터 선택: {currentTargetMonster}");

         }
   
    }


}
