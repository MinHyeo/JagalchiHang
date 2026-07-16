using UnityEngine;
using System;
using System.Collections.Generic;

public interface IMonsterGroupBehavior
{
    bool IsAlerted { get; }
    Vector3 AlertPosition { get; }

    void ReceiveAlert(Vector3 alertPosition);
    void ClearAlert();

    IReadOnlyList<Transform> Neighbors { get; }

    void Tick();
}
