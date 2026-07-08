using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.ComponentModel;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("등록 부분")]
    [SerializeField] private Image _imageIcon;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private CanvasGroup _canvasGroup;

    private InventorySlotViewModel _vm; // 뷰모델 멤버변수
    private bool _isStackable;
    private int _maxCount;

    private InventoryUI inventory;

    private void OnEnable()
    {
        var inventorySlotvm = NetworkManager_re.Inst.LocalPlayerService.GetLocalPlayerInventorySlotViewModel();
        if (inventorySlotvm != null)
        {
            BindViewModel(inventorySlotvm);
        }
    }

    public void BindViewModel(InventorySlotViewModel vm) // 네트워크 매니저에서 호출
    {
        _vm = vm;
        _vm.PropertyChanged += OnPropertyChanged_View;
        _vm.InvokeOnceInit();
    }

    public void OnDisable()
    {
        if (_vm != null)
        { 
            _vm.PropertyChanged -= OnPropertyChanged_View;
        }
    }

    // TODO : 데이터 더 받아와 추가하기
    private void OnPropertyChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(InventorySlotViewModel.Name):
                {
                    
                }
                break;
            case nameof(InventorySlotViewModel.Count):
                {
                    _countText.text = $"{_vm.Count}";
                }
                break;
            case nameof(InventorySlotViewModel.IsStackable):
                {
                    _isStackable = _vm.IsStackable;
                }
                break;
            case nameof(InventorySlotViewModel.MaxCount):
                {
                    _maxCount = _vm.MaxCount;
                }
                break;
        }
    }

    [HideInInspector] public int slotIndex;
    private Canvas cachedCanvas;

    public void Setup(int index, InventoryUI inv)
    {
        slotIndex = index;
        inventory = inv;
    }

    public void UpdateSlot(InventorySlotViewModel slot)
    {
        if (slot != null && slot.item != null)
        {
            _imageIcon.gameObject.SetActive(true);

            if (slot.item.itemIcon != null)
            {
                _imageIcon.sprite = slot.item.itemIcon;
            }

            if (slot.count > 1 && _countText != null)
            {
                _countText.text = slot.count.ToString();
                _countText.gameObject.SetActive(true);
            }
            else if (_countText != null)
            {
                _countText.gameObject.SetActive(false);
            }
        }
        else
        {
            ClearUI();
        }
    }

    public void ClearUI()
    {
        _imageIcon.gameObject.SetActive(false);
        if (_countText != null)
        {
            _countText.gameObject.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventory.slots[slotIndex].item == null) return;

        if (!inventory.slots.ContainsKey(slotIndex)) return;

        if (cachedCanvas == null)
        {
            cachedCanvas = GetComponentInParent<Canvas>();
        }

        if (cachedCanvas == null) return;

        _imageIcon.transform.SetParent(cachedCanvas.transform);
        _imageIcon.transform.SetAsLastSibling();

        if (_canvasGroup != null) 
        {
            _canvasGroup.blocksRaycasts = false; 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inventory == null || cachedCanvas == null) return;

        if (!inventory.slots.ContainsKey(slotIndex) || inventory.slots[slotIndex].item == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cachedCanvas.transform as RectTransform,
            eventData.position,
            cachedCanvas.worldCamera,
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

        inventory.UpdateAllSlotsUI();
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI startSlotUI = eventData.pointerDrag.GetComponent<InventorySlotUI>();

        if (startSlotUI != null)
        {
            inventory.SwapSlots(startSlotUI.slotIndex, this.slotIndex);
        }
    }
}