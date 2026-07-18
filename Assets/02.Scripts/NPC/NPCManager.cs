using UnityEngine;

public class NpcManager : MonoBehaviour // 벙커 로직 테스트용 
{

    [SerializeField] private BattleNpc battleNpc; 

    public Vector3 BunkerSpawnPos = new Vector3(7, 1, -9); // 테스트용 코드 (게임매니저에서 관리할것)
    public Vector3 BattleNPCHomePos = new Vector3(0, 1, -13); // NPC  매니저에서 관리 
    public Vector3 ReturnPos = new Vector3(7, 1, -8.5f); // 돌아갈 좌표 
    private void Update()
    {
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
        }

        Debug.Log($"[NPC 매니저] 벙커 진입");
    }

    public void OnBunkerExitData(bool isInBunker, Vector3 returnPos)
    {
        if (battleNpc != null)
        {
            battleNpc.ExitBunker(isInBunker, returnPos);
        }


        Debug.Log($"[NPC 매니저] 벙커 탈출 ");
    }


}
