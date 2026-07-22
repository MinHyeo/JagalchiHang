using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class NpcAttack : MonoBehaviour
{

    [Header("Attack Setting")]
    [SerializeField] private float attackRange = 1.0f; // 진짜 데미지를 입힐 수 있는 거리
    [SerializeField] private float attackCoolTime = 1f; // 공격 쿨타임
    [SerializeField] private int attackDamage = 15; // 공격 데미지
    [SerializeField] private float attackMoveSpeed = 6.5f; // 몬스터를 쫓아갈 때 속도

    private NavMeshAgent _agent;
    private Transform _attackTarget; // 현재 공격할 몬스터의 위치 정보
    

    private float _coolTimer = 0.0f; //쿨타임 재기용 (프레임단위로 충전)

    public bool isAttack { get; private set; } = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void StartAttack(Transform targetMonster) // 전투 시작
    {
        Debug.Log("[NpcAttack] StartAttack");
        Debug.Log($"[NpcAttack] 공격 시작: {targetMonster.name}");

        if (targetMonster == null)
        {
            return;
        }

        _agent.speed = attackMoveSpeed;// 추적 이동 속도로 
        _attackTarget = targetMonster; //실시간 몬스터 위치 넣기
        isAttack =true; //공격 가능상태 활성화

        _coolTimer = attackCoolTime;

        Debug.Log($"[NpcAttack] 공격 시작: {targetMonster.name}");

        _coolTimer = attackCoolTime;  // 첫타용
    }

    public void StopAttack() //몬스터 사망하거나 할 때 쓸 것
    {

        if(isAttack == false)
        {
            return;

        }
        
        _attackTarget = null; 
        isAttack = false;
        _coolTimer = 0.0f;

       _agent.ResetPath();
       
    }

    private void Update()
    {
        if (isAttack == false || _attackTarget == null)
        {
            return;
        }

        CoolTimer();

        float distance = Vector3.Distance(transform.position, _attackTarget.position); // 두지점 거리구하기


        if (distance > attackRange)
        {
            RangeAttack();
            return;
        }

        CoolTimeNpcAttack();
    }

    private void CoolTimer() // 공격 쿨타임 프레임마다 충전
    {
        if(_coolTimer < attackCoolTime)
        {
            _coolTimer += Time.deltaTime;
        }
    }

    private void RangeAttack() // 공격사거리가 닿지 않으면 쫓아가기 
    {
        if (_agent != null)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_attackTarget.position);
        }
    }

    private void CoolTimeNpcAttack() // 실제 공격
    {
        LookAtTarget();

        if (_coolTimer >= attackCoolTime)
        {
            AttackToNpc();
            _coolTimer = 0.0f;
        }
    }


    private void LookAtTarget() // Npc 모델 입히고 다시 확인
    {
        if (_attackTarget == null)
        {
            return;
        }

        Vector3 direction = (_attackTarget.position - transform.position).normalized;

        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    
    private void AttackToNpc() 
    {
        if (_attackTarget == null)
        {
            return;
        }

        MonsterHealth monster = _attackTarget.GetComponent<MonsterHealth>();

        if (monster != null)
        {
            Debug.Log($"[NPC 공격] {_attackTarget.name}에게 공격");
            monster.TakeDamage(attackDamage);
        }

    }
}
