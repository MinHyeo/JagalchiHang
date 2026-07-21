using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    private Dictionary<string, GameObject> _mapList = new Dictionary<string, GameObject>();

    private string path = "Prefabs/Map/";

    public async UniTask CreateMap()
    {
        await SpawnMap("Map");
        await SpawnMap("ParkingGarage");
    }

    private async UniTask SpawnMap(string name)
    {
        string mapPath = path + name;
        Vector3 spawnSpot = Vector3.zero;

        GameObject mapObject = await GameObjectManager.Instance.CreateObjectAsync("", mapPath, spawnSpot);
        _mapList.Add(name, mapObject);
    }
}