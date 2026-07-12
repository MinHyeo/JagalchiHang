using System;
using UnityEngine;

public class TestNPCManager : MonoBehaviour // 벙커 로직 테스트용 
{
    public static TestNPCManager Instance { get; private set; }

    public static Action<bool,  Vector3> OnBunkerEvent;

    public Vector3 testBunkerSpawnPos = new Vector3(-7, 1, -9); // 테스트용 코드 (게임매니저에서 관리할것)
    public Vector3 testBattleNPCHomePos = new Vector3(0, 1, -13); // NPC  매니저에서 관리 

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
    }


    public void OnBunkerData(bool isInBunker,  Vector3 bunkerPos) // 게임매니저한테 추후 전달 받을 곳 
    {

        OnBunkerEvent?.Invoke(isInBunker, bunkerPos);

        Debug.Log($"[NPC 매니저] 게임 매니저로부터 값을 받아왔고 데이터 전달");
    }


}
