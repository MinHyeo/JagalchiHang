using UnityEngine;

public class WalkState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.Animator.SetBool("IsWalk", true);
        player.Animator.SetBool("IsRun", false);
    }

    public void Update(PlayerController player)
    {
        if (player.IsWalking == false)
        {
            player.SetState(StateType.Idle);
            return;
        }

        if(player.IsRunning == true)
        {
            player.SetState(StateType.Run);
            return;
        }

        player.PlayerMove();
    }

    public void Exit(PlayerController player)
    {

    }
}
