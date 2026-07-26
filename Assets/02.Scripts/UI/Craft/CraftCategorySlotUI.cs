using Cysharp.Threading.Tasks;
using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftCategorySlot : MonoBehaviour
{
    [SerializeField] private Image _imageIcon;
    [SerializeField] private UIButton _buttonChoose;
    [SerializeField] private TextMeshProUGUI _textName;
    [SerializeField] private CanvasGroup _canvasGroup;

    private CraftCategorySlotViewModel _vm;
    public event Action<string> OnSlotClicked;

    private void OnDisable()
    {
        UnBindViewModel();
    }

    public void BindViewModel(CraftCategorySlotViewModel vm)
    {
        UnBindViewModel();

        _vm = vm;
        _vm.PropertyChanged += OnPropertyChanged_View;

        UpdateIcon();
        UpdateText();
        UpdateLockState();

        _buttonChoose.BindOnClickButtonEvent(OnClickSlot);
    }

    private void UpdateIcon()
    {
        if (_vm == null || string.IsNullOrEmpty(_vm.IconPath))
        {
            _imageIcon.sprite = null;
            _imageIcon.gameObject.SetActive(false);
        }
        else
        {
            InitImage().Forget();
            _imageIcon.gameObject.SetActive(true);
        }
    }

    private void UpdateText()
    {
        if (_vm == null || string.IsNullOrEmpty(_vm.ItemName))
        {
            _textName.text = null;
            _textName.gameObject.SetActive(false);
        }
        else
        {
            _textName.text = _vm.ItemName;
            _textName.gameObject.SetActive(true);
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

    private void UnBindViewModel()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropertyChanged_View;
            _vm = null;
        }
    }

    private void OnPropertyChanged_View(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CraftCategorySlotViewModel.IsLocked))
        {
            if (_buttonChoose != null)
            {
                UpdateLockState();
            }
        }
    }

    private void UpdateLockState()
    {
        if (_vm == null) return;

        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = !_vm.IsLocked;
            _canvasGroup.alpha = _vm.IsLocked ? 0.4f : 1.0f;
        }
    }

    private void OnClickSlot()
    {
        if (_vm != null)
        {
            OnSlotClicked?.Invoke(_vm.RecipeId);
        }
    }
}
