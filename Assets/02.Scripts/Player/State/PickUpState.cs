using UnityEngine;

public class PickUpState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.Animator.SetTrigger("PickUp");
    }

    public void Update(PlayerController player)
    {
        if (player.IsAttacking == true)
        {
            return;
        }

        if (player.IsWalking == false)
        {
            player.SetState(StateType.Idle);
            return;
        }

        if (player.IsRunning == true)
        {
            player.SetState(StateType.Run);
            return;
        }

        player.SetState(StateType.Walk);

    }

    public void Exit(PlayerController player)
    {

    }
}
