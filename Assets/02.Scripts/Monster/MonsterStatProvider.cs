using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class MonsterStatProvider : MonoBehaviour, IMonsterStatProvider
{
    [SerializeField] private string _monsterId;

    private int _maxHealth;
    private int _attackPower;
    private float _attackRange;
    private float _moveSpeed;

    public int MaxHealth
    {
        get { return _maxHealth; }
    }

    public int AttackPower
    {
        get { return _attackPower; }
    }

    public float AttackRange
    {
        get { return _attackRange; }
    }

    public float MoveSpeed
    {
        get { return _moveSpeed; }
    }

    private void Awake()
    {
        MonsterData data = GameDataManager.Instance.GetData<MonsterData>("Monster_1");
    
        if (data == null)
        {
            _maxHealth = 100;
            _attackPower = 10;
            _attackRange = 2f;
            _moveSpeed = 3.5f;
            return;
        }

        _maxHealth = data.MaxHp;
        _attackPower = data.BasicAttack;
        _attackRange = data.BasicAttackRange;
        _moveSpeed = data.BasicSpeed;
    } 
}
