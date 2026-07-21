using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using Cysharp.Threading.Tasks;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("등록 부분")]
    [SerializeField] private Image _imageIcon;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private CanvasGroup _canvasGroup;

    private InventorySlotViewModel _vm; // 뷰모델 멤버변수
    private InventoryUI _inventoryUI; // 부모 참조 수정 필요,
    private Canvas _cachedCanvas;

    private int _slotKey;
    public int SlotKey => _slotKey;

    private void OnDisable()
    {
        UnbindViewModel();
    }

    public void Setup(InventoryUI inventory, int slotKey)
    {
        _inventoryUI = inventory;
        _slotKey = slotKey;
    }

    public void BindViewModel(InventorySlotViewModel vm)
    {
        _vm = vm;
        _vm.PropertyChanged += OnPropertyChanged_View;
        _vm.InvokeOnceInit();

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
        switch (e.PropertyName)
        {
            case nameof(InventorySlotViewModel.ItemDataId):
                {
                }
                break;
            case nameof(InventorySlotViewModel.IconPath):
                {
                    UpdateIcon();
                }
                break ;
            case nameof(InventorySlotViewModel.ItemStackCount):
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
        if (_vm == null || string.IsNullOrEmpty(_vm?.ItemDataId))
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

    // 드래그 부분
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

        RectTransform rt = _imageIcon.rectTransform;
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;

        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = true;
        }

        UpdateUI();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_inventoryUI == null) return;

        InventorySlotUI fromInventorySlotUI = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (fromInventorySlotUI != null)
        {
            _inventoryUI.RequestSwap(fromInventorySlotUI.SlotKey, this.SlotKey);
            return;
        }

        FarmingSlotUI fromFarmingSlotUI = eventData.pointerDrag?.GetComponent<FarmingSlotUI>();
        if (fromFarmingSlotUI != null)
        {
            _inventoryUI.RequestMoveFromFarming(fromFarmingSlotUI.SlotKey, this.SlotKey);
        }

        StorageSlotUI fromStorageSlotrUI = eventData.pointerDrag?.GetComponent<StorageSlotUI>();
        if (fromStorageSlotrUI != null)
        {
            _inventoryUI.RequestMoveFromStorage(fromStorageSlotrUI.SlotKey, this.SlotKey);
        }
    }
}