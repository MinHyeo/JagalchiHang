using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public enum MapType 
{
    None,
    ParmingMap,
    ParkingGarage,
}

public class MapManager
{
    private Dictionary<MapType, Map> _mapList = new Dictionary<MapType, Map>();
    private MapType _currnetMapType;

    private string path = "Prefabs/Map/";

    public async UniTask CreateMap()
    {
        await SpawnMap(MapType.ParmingMap, true);
        await SpawnMap(MapType.ParkingGarage, false);

        _currnetMapType = MapType.ParmingMap;
        //_mapList[_currnetMapType].gameObject.SetActive(true);
    }

    private async UniTask SpawnMap(MapType mapType, bool isActive)
    {
        string mapPath = path + mapType.ToString();
        Vector3 spawnSpot = Vector3.zero;

        GameObject mapObject = await GameObjectManager.Instance.CreateObjectAsync("", mapPath, spawnSpot);
        mapObject.SetActive(isActive);
        Map mapComponent = mapObject.GetComponent<Map>();
        _mapList.Add(mapType, mapComponent);
    }

    public void ChangeMap(MapType mapType)
    {
        Map mapComponent = _mapList[_currnetMapType];
        if (mapComponent == null)
            return;
        mapComponent.gameObject.SetActive(false);

        _currnetMapType = mapType;

        Map newMapComponent = _mapList[_currnetMapType];
        if (newMapComponent == null) 
            return;

        newMapComponent.gameObject.SetActive(true);
        newMapComponent.GetSpawnPosition();
    }

    public Vector3 GetMapSpawnPosition()
    {
        return _mapList[_currnetMapType].GetSpawnPosition();
    }
}