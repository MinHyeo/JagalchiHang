using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class GeneratorUI : UIBase
{
    [Header("공용")]
    [SerializeField] private UIButton _exitButton;

    [Header("충전")]
    private MaterialSlot _selectSlot;
    [SerializeField] private List<MaterialSlot> _materialSlotList = new List<MaterialSlot>();
    [SerializeField] private TextMeshProUGUI _rechargeValueText;
    [SerializeField] private UIButton _rechargeButton;

    [Header("수리")]
    [SerializeField] private TextMeshProUGUI _fixValueText;
    [SerializeField] private UIButton _fixButton;
    private string _itemId = "Item_Drop_03";
    private int _fixItemCount = 15;

    private string redCss = "red";
    private string greenCss = "green";

    private GeneratorViewModel _generatorViewModel;
    private InventoryViewModel _inventoryViewModel;

    private void OnEnable()
    {
        _generatorViewModel = NetworkManager.Instance.GeneratorService.GetGeneratorViewModel();
        _generatorViewModel.PropertyChanged += OnPropertyChanged;
        _generatorViewModel.InvokeOnceOnInit();

        _inventoryViewModel = NetworkManager.Instance.InventoryService.GetLocalInventoryViewModel();
        _inventoryViewModel.PropertyChanged += OnPropertyChanged;

        BindOnClickEvents();

        UpdateFullCountText();
        foreach(var slot in _materialSlotList)
        {
            slot.UpdateItemCount(_inventoryViewModel);
        }
        UpdateFixValueText();
    }

    private void OnDisable()
    {
        _generatorViewModel.PropertyChanged -= OnPropertyChanged;
        _inventoryViewModel.PropertyChanged -= OnPropertyChanged;

        _exitButton.UnBindOnClickButtonEvent(ExitUI);
        _exitButton.UnBindOnClickButtonEvent(RechargeGenerator);
    }

    private void BindOnClickEvents()
    {
        _exitButton.BindOnClickButtonEvent(ExitUI);

        foreach (var slot in _materialSlotList)
        {
            slot.OnSlotClickedEvent += SelectMaterialSlot;
            slot.SetActiveOutLine(false);
        }
        _selectSlot = _materialSlotList[0];
        _selectSlot.SetActiveOutLine(true);
        _rechargeButton.BindOnClickButtonEvent(RechargeGenerator);

        _fixButton.BindOnClickButtonEvent(FixGenerator);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(GeneratorViewModel.CurrentPower):
            case nameof(GeneratorViewModel.MaxPower):
                UpdateFullCountText();
                break;
            case nameof(InventorySlotViewModel.ItemStackCount):
                UpdateFixValueText();
                break;
        }
    }

    private void ExitUI()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.GeneratorUI);
    }

    private void SelectMaterialSlot(MaterialSlot slot)
    {
        _selectSlot.SetActiveOutLine(false);
        _selectSlot = slot;
        _selectSlot.SetActiveOutLine(true);

        UpdateFullCountText();
    }

    private void UpdateFullCountText()
    {
        if (_selectSlot == null)
            return;

        int currentPower = _generatorViewModel.CurrentPower;
        int maxPower = _generatorViewModel.MaxPower;
        int amount = _selectSlot.Amount;

        _rechargeValueText.text = $"{currentPower}<color=green>(+{amount})</color> / {maxPower})";
    }

    private void RechargeGenerator()
    {
        int amount = _selectSlot.Amount;

        if(_selectSlot.CheckRechargeable(_inventoryViewModel, _generatorViewModel) == false)
        {
            Debug.Log("충전 안됨");
            return;
        }

        _selectSlot.Recharge(_inventoryViewModel);
        NetworkManager.Instance.GeneratorService.ReChargePower(amount); 
    }

    private void UpdateFixValueText()
    {
        int itemCount = _inventoryViewModel.GetItemCount(_itemId);

        string colorString = (itemCount >= _fixItemCount) ? greenCss : redCss;
        _fixValueText.text = $"<color={colorString}>{itemCount}</color> / {_fixItemCount}";
    }

    private void FixGenerator()
    {
        int itemCount = _inventoryViewModel.GetItemCount(_itemId);
        if(itemCount >= _fixItemCount)
        {
            return;
        }

        _inventoryViewModel.RemoveItem(_itemId, _fixItemCount);
        NetworkManager.Instance.GeneratorService.FixGenerator();
    }
}