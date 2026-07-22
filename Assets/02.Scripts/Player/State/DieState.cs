using UnityEngine;

public class DieState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.Animator.SetTrigger("Die");
    }

    public void Update(PlayerController player)
    {
        
    }

    public void FixedUpdate(PlayerController player)
    {

    }

    public void Exit(PlayerController player)
    {

    }
}
