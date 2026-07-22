using System;
using System.Threading;
using UnityEngine;

public static class GameUtil
{
    private static long _lastId = 0;

    public static void LoadFullData()
    {

    }

    // 그냥 유니크 키가 발급되어야 할 때 사용하려고 만든 것 (의미가 있는 건 아니므로 사용만 하세요)
    public static long GenerateUniqueId()
    {
        long newId = DateTime.UtcNow.Ticks;

        // 원자적 연산으로 안전하게 ID 갱신
        while (true)
        {
            long lastId = Volatile.Read(ref _lastId);

            // 만약 현재 시간이 이전 ID보다 작거나 같다면 (루프가 너무 빠른 경우 포함)
            // 이전 ID + 1로 강제 설정하여 중복 방지
            long idToAssign = (newId <= lastId) ? lastId + 1 : newId;

            // _lastId가 내가 읽은 시점과 같다면 idToAssign으로 교체 (성공 시 루프 탈출)
            if (Interlocked.CompareExchange(ref _lastId, idToAssign, lastId) == lastId)
            {
                return idToAssign;
            }
            // 그 사이 다른 스레드가 값을 바꿨다면 다시 시도
        }
    }

    public static WorldManager GetWorldManager()
    {
        WorldManager worldManager = GameManager.Instance.GetWorldManager();
        if(worldManager == null)
        {
            Debug.LogError("WorldManager가 존재하지 않습니다.");
            return null;
        }

        return worldManager;
    }

    public static LobbyManager GetLobbyManager() 
    {
        LobbyManager lobbyManager = GameManager.Instance.GetLobbyManager();
        if (lobbyManager == null)
        {
            Debug.LogError("LobbyManager가 존재하지 않습니다.");
            return null;
        }

        return lobbyManager;
    }

    public static PlayerManager GetPlayerManager()
    {
        WorldManager worldManager = GetWorldManager();
        if(worldManager == null)
        {
            return null;
        }

        PlayerManager playerManager = worldManager.GetPlayerManager();
        if(playerManager == null)
        {
            Debug.LogError("PlayerManager가 존재하지 않습니다.");
            return null;
        }

        return playerManager;
    }

    public static NpcManager GetNpcManager()
    {
        WorldManager worldManager = GetWorldManager();
        if (worldManager == null)
        {
            return null;
        }

        NpcManager npcManager = worldManager.GetNpcManager();
        if (npcManager == null)
        {
            Debug.LogError("NpcManager가 존재하지 않습니다.");
            return null;
        }

        return npcManager;
    }

    public static MonsterManager GetMonsterManager()
    {
        WorldManager worldManager = GetWorldManager();
        if (worldManager == null)
        {
            return null;
        }

        MonsterManager monsterManager = worldManager.GetMonsterManager();
        if (monsterManager == null)
        {
            Debug.LogError("MonsterManager가 존재하지 않습니다.");
            return null;
        }

        return monsterManager;
    }

    public static FarmManager GetFarmManager()
    {
        WorldManager worldManager = GetWorldManager();
        if (worldManager == null)
        {
            return null;
        }

        FarmManager farmManager = worldManager.GetFarmManager();
        if (farmManager == null)
        {
            Debug.LogError("FarmManager가 존재하지 않습니다.");
            return null;
        }

        return farmManager;
    }

    public static MapManager GetMapManager()
    {
        WorldManager worldManager = GetWorldManager();
        if (worldManager == null)
        {
            return null;
        }

        MapManager mapManager = worldManager.GetMapManager();
        if (mapManager == null)
        {
            Debug.LogError("MapManager가 존재하지 않습니다.");
            return null;
        }

        return mapManager;
    }
}