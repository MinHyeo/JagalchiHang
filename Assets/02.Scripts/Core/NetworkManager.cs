using System.IO;
using UnityEngine;

public class NetworkManager : SingletonBase<NetworkManager>
{
    private string _saveFilePath;

    private void Start()
    {
        _saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
    }
}