using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveDataSlot : MonoBehaviour
{
    private int _slotIndex;
    private SaveModel _saveModel;
    private LoadGameUIType _uiType;

    [SerializeField] private UIButton _slotButton;

    [SerializeField] private TextMeshProUGUI _saveIndexText;
    [SerializeField] private TextMeshProUGUI _saveDayText;

    private void OnDisable()
    {
        _slotButton.UnBindOnClickButtonEvent(LoadNewGame);
        _slotButton.UnBindOnClickButtonEvent(LoadGame);
        _slotButton.UnBindOnClickButtonEvent(SaveGame);
    }

    public void Init(int slotIndex, LoadGameUIType loadGameUIType)
    {
        _slotIndex = slotIndex;
        _uiType = loadGameUIType;

        GetSaveData(slotIndex);
        UpdateSlotUI();

        BindOnClickButtonEvent(loadGameUIType);
    }

    public void BindOnClickButtonEvent(LoadGameUIType loadGameUIType)
    {
        switch (loadGameUIType)
        {
            case LoadGameUIType.NewGame:
                _slotButton.BindOnClickButtonEvent(LoadNewGame);
                break;
            case LoadGameUIType.LoadGame:
                _slotButton.BindOnClickButtonEvent(LoadGame);
                break;
            case LoadGameUIType.SaveGame:
                _slotButton.BindOnClickButtonEvent(SaveGame);
                break;
        }
    }

    private void GetSaveData(int slotIndex)
    {
        _saveModel = NetworkManager.Instance.LoadGame(slotIndex);
        if (_saveModel == null)
            return;
    }

    private void LoadNewGame()
    {
        Debug.Log("새 게임 시작");

        _saveModel = new SaveModel();

        NetworkManager.Instance.SaveGame(_slotIndex, _saveModel);
        GameManager.Instance.EnterInGame(_saveModel, _slotIndex);
    }

    private void LoadGame()
    {
        if(_saveModel == null)
        {
            Debug.LogWarning("데이터가 없습니다.");
            return;
        }

        Debug.Log("기존 게임 시작");
        GameManager.Instance.EnterInGame(_saveModel, _slotIndex);
    }

    private void SaveGame()
    {
        if (_saveModel == null)
        {
            Debug.LogWarning("데이터가 없습니다.");
            return;
        }

        NetworkManager.Instance.RequestSaveGame(_slotIndex);

        GetSaveData(_slotIndex);
        UpdateSlotUI();
    }

    private void UpdateSlotUI()
    {
        if (_saveIndexText != null)
        {
            _saveIndexText.text = $"슬롯 {_slotIndex + 1}";
        }
        if (_saveDayText != null)
        {
            if (_saveModel == null)
            {
                _saveDayText.text = "비어 있음";
            }
            else
            {
                // TODO : 날짜 값 저장해서 넣기
                // _saveDayText.text = $"DAY : {}";
            }
        }
    }
}