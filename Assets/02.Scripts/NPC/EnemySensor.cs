using System.Collections.Generic;
using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    private List<GameObject> monsters = new List<GameObject>(); // 감지 범위 내에 들어온 몬스터들 담을 리스트 

    public GameObject CurrentTarget { get; private set; }

    private void Update()
    {
     
        // 현재 잡혀있던 목스터가 죽어서 null일 때 목록에 다른 몬스터가 남아있으면 타켓 갱신 
        if (CurrentTarget == null && monsters.Count >0 )
        {
            UpdateCurrentTarget();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Monster")) // 몬스터 레이어가 아니면 무시
        {
            return;
        }

        if (monsters.Contains(other.gameObject)) // 중복 체크
        {
            return;
        }

        monsters.Add(other.gameObject);
        UpdateCurrentTarget();
    }

    private void OnTriggerExit(Collider other) //감지범위 벗어났을 때 
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Monster")) // 몬스터 레이어가 아니면 무시
        {
            return;
        }
        monsters.Remove(other.gameObject);

        UpdateCurrentTarget();

    }

    public void ClearTarget() // 타겟을 완전히 비우고 센서 초기화 
    {
        CurrentTarget = null;
        monsters.Clear();
    }

    private void UpdateCurrentTarget()
    {
        CurrentTarget = null; //매번 다시 계산하기 위해 초기화시킴

        //처음에는 가장 가까운 거리를 무한대로 설정(그래야 처음 찾은 몬스터가 가장 가까운 몬스터가 된다.)
        float closeDistance = Mathf.Infinity; 

        for (int i = 0; i < monsters.Count; i++)
        {
            if (monsters[i] == null)
            {
                monsters.RemoveAt(i);
                i--;
                continue;
            }

            float distance = Vector3.Distance(transform.position, monsters[i].transform.position); //BattleNPC와 몬스터 사이 거리 계산

            if (distance < closeDistance)  //현재 몬스터가 더 가깝다면
            {
                closeDistance = distance;
                CurrentTarget = monsters[i];
            }
        }
    }

    private void OnDrawGizmosSelected() // 범위 그리기 
    {
        SphereCollider sphre = GetComponent<SphereCollider>();

        if (sphre == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphre.radius);
    }

}
