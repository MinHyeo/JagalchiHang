using UnityEngine;

public interface IInteractionable
{
    public int UniqueId { get; }

    public void Interaction(Transform transform);
}
