using UnityEngine;

public class Monster : MonoBehaviour, ISpawnable
{
    [SerializeField] private string _defaultMonsterId;

    public IMonsterDamageable Damageable { get; private set; }
    public IMonsterMoveable Moveable { get; private set; }
    public IMonsterCombatable Combatable { get; private set; }
    public IMonsterPerceivable Perceivable { get; private set; }
    public IMonsterStatProvider StatProvider { get; private set; }
    public IMonsterGroupBehavior Group { get; private set; }

    private string _dataId;
    private int _instanceId;

    public string DataId => _dataId;
    public int InstanceId => _instanceId;

    private void Awake()
    {
        StatProvider = new MonsterStats();

        Damageable = GetComponent<IMonsterDamageable>();
        Moveable = GetComponent<IMonsterMoveable>();
        Combatable = GetComponent<IMonsterCombatable>();
        Perceivable = GetComponent<IMonsterPerceivable>();
        Group = GetComponent<IMonsterGroupBehavior>();
    }

    private void Start() 
    {
        _dataId = _defaultMonsterId;

        StatProvider.LoadStats(_dataId);

        ApplyStatsToComponents();
    }

    public void Init(int instanceId, string dataId)
    {
        _dataId = dataId;
        _instanceId = instanceId;
        StatProvider.LoadStats(_dataId);

        ApplyStatsToComponents();
    }

    private void ApplyStatsToComponents()
    {
        Damageable.ResetForSpawn(StatProvider.MaxHealth);
        Moveable.ApplySpeed(StatProvider.MoveSpeed);
    }
}
