using UnityEngine;

public class RunState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.Animator.SetBool("IsWalk", false);
        player.Animator.SetBool("IsRun", true);
    }

    public void Update(PlayerController player)
    {
        if (player.IsWalking == false)
        {
            player.SetState(StateType.Idle);
            return;
        }

        if (player.IsRunning == false)
        {
            player.SetState(StateType.Walk);
            return;
        }
    }

    public void FixedUpdate(PlayerController player)
    {
        player.PlayerMove();
    }

    public void Exit(PlayerController player)
    {

    }
}
