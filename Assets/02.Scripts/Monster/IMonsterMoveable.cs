using UnityEngine;
using System;

public interface IMonsterMoveable
{
    bool HasReachedDestination { get; }

    bool IsMoving { get; }

    Vector3 Velocity { get; }

    event Action<bool> OnMovingStateChanged;

    void MoveTo(Vector3 destination);
    void Move(Vector3 direction);

    void Stop();
    void ApplySpeed(float speed);
}
