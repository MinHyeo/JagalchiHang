using System.IO;
using UnityEngine;

public class NetworkManager : SingletonBase<NetworkManager>
{
    private string GetSaveFilePath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"saveData{slotIndex}.json");
    }

    public void SaveGame(int slotIndex, SaveModel saveModel)
    {
        string saveFilePath = GetSaveFilePath(slotIndex);
        string jsonText = JsonUtility.ToJson(saveModel, true);

        File.WriteAllText(saveFilePath, jsonText);
        Debug.Log($"저장 완료: {saveFilePath}");
    }

    public SaveModel LoadGame(int slotIndex)
    {
        string saveFilePath = GetSaveFilePath(slotIndex);

        if (File.Exists(saveFilePath) == false)
            return null;

        string jsonText = File.ReadAllText(saveFilePath);
        SaveModel saveModel = JsonUtility.FromJson<SaveModel>(jsonText);

        return saveModel;
    }
}