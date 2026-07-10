using UnityEngine;
using System;
using Unity.Behavior;
using Unity.Properties;
using Action = Unity.Behavior.Action;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Patrol",
    story: "[Agent]가 주변을 랜덤한 방향으로 배회한다.",
    category: "Action",
    id: "29394a5b6c7d8e9f0a1b2c3d4e5f6071")]

public class PatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> WanderRadius;
    [SerializeReference] public BlackboardVariable<float> WaitDuration;

    private IMonsterMoveable _moveable;
    private bool _isWaiting;
    private float _waitElapsedTime;

    protected override Status OnStart()
    {
        _moveable = Agent.Value.GetComponent<IMonsterMoveable>();

        _isWaiting = false;
        _waitElapsedTime = 0f;
        PickNewWanderDestination();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isWaiting)
        {
            _waitElapsedTime += Time.deltaTime;

            if (_waitElapsedTime < WaitDuration.Value)
            {
                return Status.Running;
            }

            _isWaiting = false;
            _waitElapsedTime = 0f;
            PickNewWanderDestination();
        }

        if (_moveable.HasReachedDestination)
        {
            _isWaiting = true;
        }

        return Status.Running;
    }

    private void PickNewWanderDestination()
    {
        Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * WanderRadius.Value;

        randomOffset.y = 0f;
        Vector3 candidatePosition = Agent.Value.transform.position + randomOffset;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(candidatePosition, out hit, WanderRadius.Value, NavMesh.AllAreas))
        {
            _moveable.MoveTo(hit.position);
        }
    }

    protected override void OnEnd()
    {
        try
        {
            _moveable.Stop();
        }
        catch
        {

        }
        
    }
}
