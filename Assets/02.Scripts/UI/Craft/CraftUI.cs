using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class CraftUI : UIBase
{
    [Header("제작 선택")]
    [SerializeField] private GameObject _prefabSlotCategory;
    [SerializeField] private Transform _categoryRoot;

    [Header("제작 공간")]
    [SerializeField] private Image _imageResultIcon;
    [SerializeField] private GameObject _prefabSlotIngredient;
    [SerializeField] private Transform _ingredientRoot;
    [SerializeField] private UIButton _buttonCraft;

    private CraftViewModel _vm;
    private List<CraftCategorySlot> _categorySlotList = new List<CraftCategorySlot>();
    private List<CraftIngredientSlotUI> _ingredientSlotList = new List<CraftIngredientSlotUI>();

    private void OnEnable()
    {
        _vm = NetworkManager.Instance.CraftService.GetCraftViewModel();
        _vm.PropertyChanged += OnPropertyChanged_View;

        _buttonCraft.BindOnClickButtonEvent(OnClickCraft);

        _vm.InitCraftRecipes();
        InitCategoryList();

        UpdateCraftingDetail();
    }

    private void OnDisable()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropertyChanged_View;
        }

        ClearCategoryList();
        ClearIngredientList();
    }

    private void InitCategoryList()
    {
        ClearCategoryList();

        foreach (var slotVm in _vm.CategorySlots)
        {
            GameObject gObj = Instantiate(_prefabSlotCategory, _categoryRoot);
            if (gObj == null) continue;

            CraftCategorySlot slotUI = gObj.GetComponent<CraftCategorySlot>();
            if (slotUI == null) continue;

            slotUI.BindViewModel(slotVm);
            slotUI.OnSlotClicked += OnCategorySlotSelected;

            _categorySlotList.Add(slotUI);
        }
    }

    private void ClearCategoryList()
    {
        foreach (var slotUI in _categorySlotList)
        {
            if (slotUI != null)
            {
                slotUI.OnSlotClicked -= OnCategorySlotSelected;
                Destroy(slotUI.gameObject);
            }
        }

        _categorySlotList.Clear();
    }

    private void OnCategorySlotSelected(string recipeId)
    {
        _vm.SelectRecipe(recipeId);
    }

    private void UpdateCraftingDetail()
    {
        if (_imageResultIcon != null)
        {
            if (!string.IsNullOrEmpty(_vm.ResultIconPath))
            {
                InitImage().Forget();
                _imageResultIcon.gameObject.SetActive(true);
            }
            else
            {
                _imageResultIcon.gameObject.SetActive(false);
            }
        }

        ClearIngredientList();

        foreach (var ingVm in _vm.IngredientSlots)
        {
            GameObject gObj = Instantiate(_prefabSlotIngredient, _ingredientRoot);
            if (gObj == null) continue;

            CraftIngredientSlotUI ingUI = gObj.GetComponent<CraftIngredientSlotUI>();
            if (ingUI == null) continue;

            ingUI.BindViewModel(ingVm);
            _ingredientSlotList.Add(ingUI);
        }
    }

    private void ClearIngredientList()
    {
        foreach (var slotUI in _ingredientSlotList)
        {
            if (slotUI != null)
            {
                Destroy(slotUI.gameObject);
            }
        }
        _ingredientSlotList.Clear();
    }

    private void OnClickCraft()
    {
        _vm.RequestCraft();
    }

    private void OnPropertyChanged_View(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CraftViewModel.CategorySlots))
        {
            InitCategoryList();
        }
        else if (e.PropertyName == nameof(CraftViewModel.SelectedRecipe))
        {
            UpdateCraftingDetail();
        }
    }

    private async UniTask InitImage()
    {
        var iconPath = _vm.ResultIconPath;
        if (string.IsNullOrEmpty(iconPath)) return;

        var cancellationToken = this.GetCancellationTokenOnDestroy();

        Sprite loadecSprite = await ResourceManager.Instance.LoadAsset<Sprite>(iconPath);

        if (cancellationToken.IsCancellationRequested) return;
        if (_vm == null || _vm.ResultIconPath != iconPath) return;

        _imageResultIcon.sprite = loadecSprite;
    }

}
