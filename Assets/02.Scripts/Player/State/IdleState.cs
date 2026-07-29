using UnityEngine;

public class IdleState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.Animator.SetBool("IsWalk", false);
        player.Animator.SetBool("IsRun", false);
    }

    public void Update(PlayerController player)
    {
        if (player.IsWalking == true)
        { 
            if(player.IsRunning == true)
            {
                player.ChangeState(StateType.Run);
            }

            else
            {
                player.ChangeState(StateType.Walk);
            }
        }
    }

    public void FixedUpdate(PlayerController player)
    {

    }

    public void Exit(PlayerController player)
    {

    }
}
