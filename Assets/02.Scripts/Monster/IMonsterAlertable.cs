using UnityEngine;

public interface IMonsterAlertable
{
    bool IsAlerted { get; }

    Vector3? AlertPosition { get; }

    void ReceiveAlert(Vector3 alertPosition);

    void ClearAlert();
}
