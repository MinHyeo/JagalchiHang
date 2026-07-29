using UnityEngine;

public class HitState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.Animator.SetTrigger("Hit");
    }

    public void Update(PlayerController player)
    {
        if (player.IsHit == true)
        {
            return;
        }

        if (player.IsWalking == false)
        {
            player.ChangeState(StateType.Idle);
            return;
        }

        if (player.IsRunning == true)
        {
            player.ChangeState(StateType.Run);
            return;
        }

        player.ChangeState(StateType.Walk);
    }

    public void FixedUpdate(PlayerController player)
    {

    }

    public void Exit(PlayerController player)
    {

    }
}
