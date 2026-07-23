using Cysharp.Threading.Tasks;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftIngredientSlotUI : MonoBehaviour
{
    [SerializeField] private Image _imageIcon;
    [SerializeField] private TextMeshProUGUI _textCount;

    private CraftIngredientSlotViewModel _vm;

    private void OnDisable()
    {
        UnbindViewModel();
    }

    public void BindViewModel(CraftIngredientSlotViewModel vm)
    {
        UnbindViewModel();

        _vm = vm;
        _vm.PropertyChanged += OnPropertyChanged_View;

        UpdateUI();
    }

    private void UnbindViewModel()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropertyChanged_View;
            _vm = null;
        }
    }

    private void OnPropertyChanged_View(object sender, PropertyChangedEventArgs e)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_vm == null) return;

        if (_imageIcon != null && !string.IsNullOrEmpty(_vm.IconPath))
        {
            InitImage().Forget();
        }

        if (_textCount != null)
        {
            _textCount.text = $"{_vm.CurrentCount}/{_vm.RequireCount}";
            _textCount.color = _vm.HasEnough ? Color.white : Color.red;
        }
    }

    private async UniTask InitImage()
    {
        var iconPath = _vm.IconPath;
        if (string.IsNullOrEmpty(iconPath)) return;

        var cancellationToken = this.GetCancellationTokenOnDestroy();

        Sprite loadecSprite = await ResourceManager.Instance.LoadAsset<Sprite>(iconPath);

        if (cancellationToken.IsCancellationRequested) return;
        if (_vm == null || _vm.IconPath != iconPath) return;

        _imageIcon.sprite = loadecSprite;
    }
}
