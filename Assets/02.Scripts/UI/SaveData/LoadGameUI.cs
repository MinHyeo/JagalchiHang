using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public enum LoadGameUIType
{
    None,
    NewGame,
    LoadGame,
    SaveGame,
}

public class LoadGameUI : UIBase
{
    [Header("프리팹")]
    [SerializeField] private GameObject _saveDataSlotPrefab;

    [Header("세이브 데이터 생성")]
    [SerializeField] private Transform _saveDataRoot;

    [Header("버튼 등록")]
    [SerializeField] private UIButton _exitButton;

    private LoadGameUIType _loadGameUIType;
    private List<SaveDataSlot> _createdSaveSlotList = new List<SaveDataSlot>();

    private SaveLoadViewModel _saveLoadViewModel;

    private void OnEnable()
    {
        _saveLoadViewModel = NetworkManager.Instance.SaveLoadService.GetSaveLoadViewModel();

        _exitButton.BindOnClickButtonEvent(OnClickExitButton);
        _saveLoadViewModel.PropertyChanged += OnPropertyChanged;

        Init(_loadGameUIType);
    }

    private void OnDisable()
    {
        _exitButton.UnBindOnClickButtonEvent(OnClickExitButton);
        _saveLoadViewModel.PropertyChanged -= OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SaveLoadViewModel.SaveModelList):
                Init(_loadGameUIType);
                break;
        }
    }

    private void OnClickExitButton()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.LoadGameUI);
    }

    public void Init(LoadGameUIType loadGameUIType)
    {
        Debug.Log("세이브 데이터 최신화");
        int listCount = _createdSaveSlotList.Count;

        if (listCount == 0)
        {
            CreateSaveDataSlot();
            listCount = _createdSaveSlotList.Count;
        }

        for(int index = 0; index < listCount; index++)
        {
            SlotInit(index, loadGameUIType);
        }
    }

    private void SlotInit(int index, LoadGameUIType loadGameUIType)
    {
        //if (_loadGameUIType == loadGameUIType)
        //    return;

        _loadGameUIType = loadGameUIType;
        var saveModel = _saveLoadViewModel.SaveModelList[index];

        string title = $"슬롯 {index + 1}";
        string info;
        if(saveModel == null)
        {
            info = "비어 있음";
        }
        else
        {
            info = $"DAY : {saveModel.Day}";
        }
        _createdSaveSlotList[index].Init(index, title, info, OnSlotClick);
    }

    private void OnSlotClick(int clickedIndex)
    {
        switch (_loadGameUIType)
        {
            case LoadGameUIType.NewGame:
                NetworkManager.Instance.SaveLoadService.StartNewGame(clickedIndex);
                break;
            case LoadGameUIType.LoadGame:
                NetworkManager.Instance.SaveLoadService.StartGame(clickedIndex);
                break;
            case LoadGameUIType.SaveGame:
                NetworkManager.Instance.SaveLoadService.SaveGame(clickedIndex);
                break;
        }
    }

    private void CreateSaveDataSlot()
    {
        int listSize = _saveLoadViewModel.SaveCount;
        for(int index = 0; index < listSize; index++)
        {
            GameObject slotObject = Instantiate(_saveDataSlotPrefab, _saveDataRoot);
            SaveDataSlot slotComponent = slotObject.GetComponent<SaveDataSlot>();

            _createdSaveSlotList.Add(slotComponent);
        }
    }
}