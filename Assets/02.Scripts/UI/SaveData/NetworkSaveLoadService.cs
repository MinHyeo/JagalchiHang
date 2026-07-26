using System;
using System.Linq.Expressions;
using UnityEngine;

public class NetworkSaveLoadService
{
    private SaveLoadViewModel _saveLoadViewModel;

    public SaveLoadViewModel GetSaveLoadViewModel()
    {
        if (_saveLoadViewModel == null)
        {
            var saveLoadViewModel = new SaveLoadViewModel();
            SetSaveLoadData(saveLoadViewModel);
            _saveLoadViewModel = saveLoadViewModel;
        }

        return _saveLoadViewModel;
    }

    private void SetSaveLoadData(SaveLoadViewModel vm)
    {
        for(int index = 0; index < vm.SaveCount; index++)
        {
            var saveModel = NetworkManager.Instance.LoadGame(index);

            //if(saveModel == null)
            //{
            //    saveModel = new SaveModel();
            //}

            vm.SaveModelList.Add(saveModel);
        }
    }

    public void StartNewGame(int index)
    {
        Debug.Log("새 게임 시작");
        var saveModel = _saveLoadViewModel.SaveModelList[index];
        saveModel = new SaveModel();
        GameManager.Instance.EnterInGame(saveModel, index);
    }

    public void StartGame(int index)
    {
        var saveModel = _saveLoadViewModel.SaveModelList[index];
        if (saveModel == null)
        {
            Debug.LogWarning("데이터가 없습니다.");
            return;
        }

        Debug.Log("기존 게임 시작");
        GameManager.Instance.EnterInGame(saveModel, index);
    }

    public void SaveGame(int index)
    {
        var saveModel = NetworkManager.Instance.RequestSaveGame(index);

        _saveLoadViewModel.SaveModelList[index] = saveModel;
        _saveLoadViewModel.InvokeOnceOnInit();
    }
}