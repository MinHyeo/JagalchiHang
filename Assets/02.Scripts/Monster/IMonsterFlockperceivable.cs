using UnityEngine;
using System.Collections.Generic;

public interface IMonsterFlockperceivable
{
    IReadOnlyList<Transform> Neighbors { get; }
}
