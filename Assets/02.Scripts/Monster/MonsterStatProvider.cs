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
        MonsterData data = null;

        if (GameDataManager.Instance != null )
        {
            try
            {
                data = GameDataManager.Instance.GetData<MonsterData>(_monsterId);
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning($"{name} : GameDataManager 조회 실패, 기본값으로 대체합니다 ({exception.Message})");
            }
        }
        else
        {
            Debug.LogWarning($"{name} : GameDataManager.Instance가 null입니다. 씬에 GameDataManager가 있는지 확인요망. 기본값으로 대체합니다.");
        }

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
