using UnityEngine;

public class Player : MonoBehaviour, ISpawnable
{
    private string _dataId;
    private int _instanceId;

    private PlayerData _playerData;

    public PlayerData PlayerData => _playerData;

    private PlayerStatusController _statusController;
    private PlayerController _controller;

    private void Awake()
    {
        _statusController = GetComponent<PlayerStatusController>();
        _controller = GetComponent<PlayerController>();
    }

    public void Init(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _dataId = dataId;

        _playerData = GameDataManager.Instance.GetData<PlayerData>(_dataId);

        if (_playerData == null)
        {
            Debug.LogError("플레이어 데이터를 찾을 수 없습니다.");
            return;
        }

        _statusController.InitPlayerStatus(_playerData);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interaction"))
        {
            var component = other.GetComponent<IInteractionable>();
            if (component == null)
                return;

            component.Interaction();
        }
    }
}
