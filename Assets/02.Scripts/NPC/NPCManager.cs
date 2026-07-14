using System;
using UnityEngine;

public class NpcManager : MonoBehaviour // 벙커 로직 테스트용 
{
    public static NpcManager Instance { get; private set; }

    public static Action<bool,  Vector3> OnBunkerEnterEvent;
    public static Action<bool, Vector3> OnBunkerExitEvent;

    public Vector3 testBunkerSpawnPos = new Vector3(-7, 1, -9); // 테스트용 코드 (게임매니저에서 관리할것)
    public Vector3 testBattleNPCHomePos = new Vector3(0, 1, -13); // NPC  매니저에서 관리 
    public Vector3 testReturnPos = new Vector3(7, 1, -8.5f); // 돌아갈 좌표 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // 테스트용 코드
        {
            OnBunkerData(true, testBunkerSpawnPos);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            OnBunkerExitData(false, testReturnPos);
        }
    }


    public void OnBunkerData(bool isInBunker,  Vector3 bunkerPos) // 게임매니저한테 추후 전달 받을 곳 
    {

        OnBunkerEnterEvent?.Invoke(isInBunker, bunkerPos);

        Debug.Log($"[NPC 매니저] 벙커 진입");
    }

    public void OnBunkerExitData(bool isInBunker, Vector3 returnPos)
    {
        OnBunkerExitEvent?.Invoke(isInBunker, returnPos);

        Debug.Log($"[NPC 매니저] 벙커 탈출 ");
    }


}
