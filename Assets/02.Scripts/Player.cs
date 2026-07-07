using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private StateController _stateController;
    [SerializeField] private float _smoothness = 10f;
    [SerializeField] private bool _toggleCameraRotation;    // Alt를 눌렀을 때 둘러보기 가능하도록 하는 변수

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;    // 태그에 MainCamera가 활성화 돼있어야 함
    }

    private void Update()
    {
        Move();
        Attack();

        if(Input.GetKey(KeyCode.LeftAlt))
        {
            _toggleCameraRotation = true;    // 둘러보기 활성화
        }
        else
        {
            _toggleCameraRotation = false;   // 둘러보기 비활성화
        }
    }

    private void LateUpdate()
    {
        if(_toggleCameraRotation == false)
        {
            // 두 벡터 곱하기
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * _smoothness);
        }
    }

    private void ChangeState(EntityAnimState curState)
    {
        _stateController.SetState(curState);
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 move = (Vector3.right * x) + (Vector3.forward * y);

        if(move.magnitude > 0f)
        {
            ChangeState(EntityAnimState.Walk);
            
            bool isRun = Input.GetKey(KeyCode.LeftShift);

            ChangeState(isRun ? EntityAnimState.Run : EntityAnimState.Walk);

            float speed = isRun ? _moveSpeed * 1.5f : _moveSpeed;

            transform.Translate(move.normalized * speed * Time.deltaTime);

        }
        else
        {
            ChangeState(EntityAnimState.Idle);
        }

    }

    private void Attack()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ChangeState(EntityAnimState.Attack);
        }
    }
}
