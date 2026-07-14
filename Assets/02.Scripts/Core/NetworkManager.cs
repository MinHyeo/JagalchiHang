using System.IO;
using UnityEngine;

public class NetworkManager : SingletonBase<NetworkManager>
{
    private string _saveFilePath;

    private void Start()
    {
        _saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    public void SaveGame(SaveModel saveModel)
    {
        string jsonText = JsonUtility.ToJson(saveModel, true);

        File.WriteAllText(_saveFilePath, jsonText);

        Debug.Log($"저장 완료: {_saveFilePath}");
    }

    public SaveModel LoadGame()
    {
        if (File.Exists(_saveFilePath) == false)
            return new SaveModel();

        string jsonText = File.ReadAllText(_saveFilePath);
        SaveModel saveModel = JsonUtility.FromJson<SaveModel>(jsonText);
        return saveModel;
    }
}