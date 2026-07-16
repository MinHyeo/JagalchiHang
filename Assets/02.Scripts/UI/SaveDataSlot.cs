using UnityEngine;
using UnityEngine.UI;

public class SaveDataSlot : MonoBehaviour
{
    private int _slotIndex;
    private SaveModel _saveModel;

    [SerializeField] private UIButton _slotButton;

    [SerializeField] private Text _saveIndexText;
    [SerializeField] private Text _saveDayText;

    private void OnDisable()
    {
        _slotButton.UnBindOnClickButtonEvent(LoadNewGame);
        _slotButton.UnBindOnClickButtonEvent(LoadGame);
    }

    public void Init(int slotIndex)
    {
        _slotIndex = slotIndex;
        GetSaveData(slotIndex);
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
        GameManager.Instance.EnterInGame(_saveModel);
    }

    private void LoadGame()
    {
        if(_saveModel == null)
        {
            Debug.LogWarning("데이커가 없습니다.");
            return;
        }

        Debug.Log("기존 게임 시작");
        GameManager.Instance.EnterInGame(_saveModel);
    }
}