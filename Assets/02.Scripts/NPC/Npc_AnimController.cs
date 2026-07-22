using UnityEngine;

public class Npc_AnimController : MonoBehaviour
{
    public enum Npc_AnimState
    {
        None = 0,
        Idle = 1,
        Walk = 2,
        Attack =4
    }

    [SerializeField] private Animator Npc_Animator;

    private Npc_AnimState currentState;

    public void SetNpcAnimState(Npc_AnimState newstate)
    {
        if(currentState == newstate)
        {
            return;
        }
        currentState = newstate;

        ResetAllAnimParameters();

        switch (currentState)
        {

            case Npc_AnimState.Idle:
                ResetAllAnimParameters();
                break;
            case Npc_AnimState.Walk:
                Npc_Animator.SetBool("IsWalk", true);
                break;
            case Npc_AnimState.Attack:
                Npc_Animator.SetBool("IsAttack", true);
                break;

            default:
                ResetAllAnimParameters();
                Debug.LogWarning("올바르지 않은 상태입니다.");
                break;
        }
    }

    private void ResetAllAnimParameters()
    {
        Npc_Animator.SetBool("IsWalk", false);
        Npc_Animator.SetBool("IsAttack", false);
    }
}
