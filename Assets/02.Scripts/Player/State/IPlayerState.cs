using JetBrains.Annotations;
using UnityEngine;

public interface IPlayerState
{
    void Enter(PlayerController player);
    void Update(PlayerController player);
    void FixedUpdate(PlayerController player);
    void Exit(PlayerController player);
}
