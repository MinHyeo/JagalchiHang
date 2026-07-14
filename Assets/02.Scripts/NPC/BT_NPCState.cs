using System;
using Unity.Behavior;

[BlackboardEnum]
public enum NpcState
{
	Idle,
	Patrol,
	Wandering,
	Chase,
	Attack
}
