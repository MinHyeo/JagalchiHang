using System;
using Unity.Behavior;

[BlackboardEnum]
public enum NPCState
{
	Idle,
	Patrol,
	Wandering,
	Chase,
	Attack
}
