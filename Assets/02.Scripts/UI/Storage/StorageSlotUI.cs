using Cysharp.Threading.Tasks;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("등록 부분")]
    [SerializeField] private Image _imageIcon;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private CanvasGroup _canvasGroup;

    private StorageSlotViewModel _vm; // 뷰모델 멤버변수
    private StorageUI _storageUI;
    private Canvas _cachedCanvas;

    private int _slotKey;
    public int SlotKey => _slotKey;

    private void OnDisable()
    {
        UnbindViewModel();
    }

    public void Setup(StorageUI storage, int slotKey)
    {
        _storageUI = storage;
        _slotKey = slotKey;
    }

    public void BindViewModel(StorageSlotViewModel vm)
    {
        UnbindViewModel();

        _vm = vm;
        _vm.PropertyChanged += OnPropertyChanged_View;
        _vm.InvokeOnceInit();

        UpdateUI();
    }

    public void UnbindViewModel()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropertyChanged_View;
            _vm = null;
        }
    }

    private void OnPropertyChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(FarmingSlotViewModel.ItemDataId):
                {
                }
                break;
            case nameof(FarmingSlotViewModel.IconPath):
                {
                    UpdateIcon();
                }
                break;
            case nameof(FarmingSlotViewModel.ItemStackCount):
                {
                    UpdateCountText();
                }
                break;
        }
    }

    private void UpdateUI()
    {
        UpdateIcon();
        UpdateCountText();
    }

    private void UpdateIcon()
    {
        if (string.IsNullOrEmpty(_vm?.ItemDataId))
        {
            _imageIcon.gameObject.SetActive(false);
        }
        else
        {
            InitImage().Forget();
            _imageIcon.gameObject.SetActive(true);
        }
    }

    private void UpdateCountText()
    {
        if (_vm != null && _vm.ItemStackCount >= 1)
        {
            _countText.text = $"{_vm.ItemStackCount}";
            _countText.gameObject.SetActive(true);
        }
        else
        {
            _countText.gameObject.SetActive(false);
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_vm == null || string.IsNullOrEmpty(_vm.ItemDataId)) return;

        if (_cachedCanvas == null)
        {
            _cachedCanvas = GetComponentInParent<Canvas>();
        }
        if (_cachedCanvas == null) return;

        _imageIcon.transform.SetParent(_cachedCanvas.transform);
        _imageIcon.transform.SetAsLastSibling();

        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_vm == null || string.IsNullOrEmpty(_vm.ItemDataId) || _cachedCanvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _cachedCanvas.transform as RectTransform,
            eventData.position,
            _cachedCanvas.worldCamera,
            out Vector2 localPoint
        );
        _imageIcon.rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _imageIcon.transform.SetParent(this.transform);
        _imageIcon.rectTransform.localPosition = Vector2.zero;

        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_storageUI == null) return;

        StorageSlotUI fromStorageSlotUI = eventData.pointerDrag?.GetComponent<StorageSlotUI>();
        if (fromStorageSlotUI != null)
        {
            _storageUI.RequestSwap(fromStorageSlotUI.SlotKey, this.SlotKey);
            return;
        }

        InventorySlotUI fromInventorySlotUI = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (fromInventorySlotUI != null)
        {
            _storageUI.RequestMoveFromInventory(fromInventorySlotUI.SlotKey, this.SlotKey);
            return;
        }
    }
}
