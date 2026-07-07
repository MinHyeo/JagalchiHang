using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image _imageIcon;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private CanvasGroup _canvasGroup;

    [HideInInspector] public int slotIndex;
    private InventoryUI inventory;
    private Canvas cachedCanvas;

    // ★ Find 대신 초기화할 때 정보를 받아오는 함수
    public void Setup(int index, InventoryUI inv)
    {
        slotIndex = index;
        inventory = inv;
    }

    public void UpdateSlot(InventorySlot slot)
    {
        if (slot != null && slot.item != null)
        {
            _imageIcon.gameObject.SetActive(true);
            _imageIcon.color = Color.white;

            if (slot.item.itemIcon != null) _imageIcon.sprite = slot.item.itemIcon;

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
        if (_countText != null) _countText.gameObject.SetActive(false);
    }

    // --- 드래그앤드롭 구현부 ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventory.slots[slotIndex].item == null) return;

        if (!inventory.slots.ContainsKey(slotIndex)) return;

        if (inventory.slots[slotIndex].item == null) return;
        
        if (cachedCanvas == null)
        {
            cachedCanvas = GetComponentInParent<Canvas>();
        }

        if (cachedCanvas == null) return;

        _imageIcon.transform.SetParent(cachedCanvas.transform);
        _imageIcon.transform.SetAsLastSibling();

        if (_canvasGroup != null) _canvasGroup.blocksRaycasts = false;
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

        if (_canvasGroup != null) _canvasGroup.blocksRaycasts = true;

        inventory.UpdateAllSlotsUI();
    }

    public void OnDrop(PointerEventData eventData)
    {
        SlotUI startSlotUI = eventData.pointerDrag.GetComponent<SlotUI>();

        if (startSlotUI != null)
        {
            inventory.SwapSlots(startSlotUI.slotIndex, this.slotIndex);
        }
    }
}