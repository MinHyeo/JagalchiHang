using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.ComponentModel;
using System.Diagnostics.Contracts;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("등록 부분")]
    [SerializeField] private Image _imageIcon;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private CanvasGroup _canvasGroup;

    private InventorySlotViewModel _vm; // 뷰모델 멤버변수
    private InventoryUI _inventoryUI;
    private Canvas _cachedCanvas;

    public int SlotKey => transform.GetSiblingIndex();

    public void Setup(InventoryUI inv)
    {
        _inventoryUI = inv;
    }

    public void BindViewModel(InventorySlotViewModel vm)
    {
        UnbindViewModel();

        _vm = vm;
        _vm.PropertyChanged += OnPropertyChanged_View;

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
            case nameof(InventorySlotViewModel.ItemDataId):
                {
                    UpdateIcon();
                }
                break;
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
        if (string.IsNullOrEmpty(_vm?.ItemDataId))
        {
            _imageIcon.gameObject.SetActive(false);
        }
        else
        {
            _imageIcon.gameObject.SetActive(true);
            // TODO: 이미지 로드 필요
        }
    }

    private void UpdateCountText()
    {
        if (_vm != null && _vm.ItemStackCount > 1)
        {
            _countText.text = $"{_vm.ItemStackCount}";
            _countText.gameObject.SetActive(true);
        }
        else
        {
            _countText.gameObject.SetActive(false);
        }
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
    }
}