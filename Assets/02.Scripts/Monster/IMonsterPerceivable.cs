using UnityEngine;
using System;

public interface IMonsterPerceivable
{
    bool CanSeePlayer {  get; }

    Vector3? LastKnownPlayerPosition { get; }

    event Action<Vector3> OnPlayerSpotted;
    
    bool HasDetectedTrail { get; }

    Vector3? TrailPosition { get; }

    void ClearTrail();

}
