using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum LoadGameUIType
{
    None,
    NewGame,
    LoadGame,
}

public class LoadGameUI : UIBase
{
    [Header("프리팹")]
    [SerializeField] private GameObject _saveDataSlotPrefab;

    [Header("세이브 데이터 생성")]
    [SerializeField] private Transform _saveDataRoot;
    [SerializeField] private int _saveDataLength = 10;

    [Header("버튼 등록")]
    [SerializeField] private UIButton _exitButton;

    private LoadGameUIType _loadGameUIType;
    private List<SaveDataSlot> _createdSaveSlotList = new List<SaveDataSlot>();

    private void OnEnable()
    {
        _exitButton.BindOnClickButtonEvent(OnClickExitButton);
    }

    private void OnDisable()
    {
        _exitButton.UnBindOnClickButtonEvent(OnClickExitButton);
    }

    private void OnClickExitButton()
    {
        UIManager.Instance.CloseUI(UIRootType.ContentUI, UIType.LoadGameUI);
    }

    public void Init(LoadGameUIType loadGameUIType)
    {
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
        _createdSaveSlotList[index].BindOnClickButtonEvent(loadGameUIType);

        if (_loadGameUIType == loadGameUIType)
            return;

        _loadGameUIType = loadGameUIType;
        _createdSaveSlotList[index].Init(index);
    }

    private void CreateSaveDataSlot()
    {
        for(int index = 0; index < _saveDataLength; index++)
        {
            GameObject slotObject = Instantiate(_saveDataSlotPrefab, _saveDataRoot);
            SaveDataSlot slotComponent = slotObject.GetComponent<SaveDataSlot>();

            slotComponent.Init(index);
            _createdSaveSlotList.Add(slotComponent);
        }
    }
}