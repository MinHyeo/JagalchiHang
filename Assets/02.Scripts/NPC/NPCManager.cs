using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class NpcManager : MonoBehaviour// 벙커 로직 테스트용 
{
    private GameObject _battleNpc;
    private GameObject _bagNpc;

    [SerializeField] private BattleNpc battleNpc; 
    [SerializeField] private BagNpc bagNpc;

    private ITargetable _chasePlayer;

    public Vector3 BunkerSpawnPos = new Vector3(7, 1, -9); // 테스트용 코드 (게임매니저에서 관리할것)
    public Vector3 ReturnPos = new Vector3(7, 1, -8.5f); // 돌아갈 좌표 

    public void Init(ITargetable target)
    {
        _chasePlayer = target;
        Debug.Log($"{_chasePlayer}");

        SpawnNpc().Forget();
    }

    public async UniTaskVoid SpawnNpc() {
        _battleNpc = await Addressables.InstantiateAsync("Prefab/Npc_Battle");

        if(_battleNpc != null)
        {
            battleNpc = _battleNpc.GetComponent<BattleNpc>();
            Debug.Log("[NPCManager] BattleNpc 생성 및 컴포넌트 연결 완료 ");
        }

        _bagNpc = await Addressables.InstantiateAsync("Prefab/Npc_Bag");

        if (_bagNpc != null)
        {
            bagNpc = _bagNpc.GetComponent<BagNpc>();
            Debug.Log("[NPCManager] BagNpc 생성 및 컴포넌트 연결 완료 ");
        }

    }

    public void NpcUpdate()
    {
        if (_chasePlayer == null)
        {
            Debug.Log("예외 발생");
            return;
        }
            

        if(battleNpc != null)
        {
            battleNpc.UpdatePlayerPosition(_chasePlayer.GetPosition());
        }
        if(bagNpc != null)
        {
            bagNpc.UpdatePlayerPosition(_chasePlayer.GetPosition());
        }
        if (Input.GetKeyDown(KeyCode.F)) // 테스트용 코드
        {
            OnBunkerEnterData(true, BunkerSpawnPos);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            OnBunkerExitData(false, ReturnPos);
        }
    }

    public void ChangeBattleMode(BattleMode battleMode) //BattleNpc로 전달
    {
        if(battleNpc != null)
        {
            battleNpc.SetBattleMode(battleMode);
            Debug.Log($"[NpcManager] 배틀 Npc에게 새로운 전투 모드 {battleMode} 전달 ");
        }
    }
    public void OnBunkerEnterData(bool isInBunker,  Vector3 bunkerPos) // 게임매니저한테 추후 전달 받을 곳 
    {

       if(battleNpc != null)
        {
            battleNpc.EnterBunker(isInBunker, bunkerPos);
            Debug.Log($"[NPC 매니저] Battle Npc벙커 진입");
        }

       if(bagNpc != null)
        {
            bagNpc.EnterBunker(isInBunker, bunkerPos);
            Debug.Log($"[NPC 매니저] Bag Npc벙커 진입");
        }

        Debug.Log($"[NPC 매니저] 벙커 진입");
    }

    public void OnBunkerExitData(bool isInBunker, Vector3 returnPos)
    {
        if (battleNpc != null)
        {
            battleNpc.ExitBunker(isInBunker, returnPos);
        }

        if(bagNpc != null)
        {
            bagNpc.ExitBunker(isInBunker, returnPos);
        }


        Debug.Log($"[NPC 매니저] 벙커 탈출 ");
    }


}
