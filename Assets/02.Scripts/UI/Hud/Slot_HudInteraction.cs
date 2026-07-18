using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot_HudInteraction : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textInteractionTitle;
    [SerializeField] private TextMeshProUGUI _textKeyName;
    [SerializeField] private UIButton _buttonOnClickInteraction;

    [SerializeField] private int slotOffsetX;
    [SerializeField] private int slotOffsetY;

    private int _instanceId;
    private Transform _targetTransform;

    private string _interactionCallbackMsg;
    private event Action<string> _onClickCallback;

    private void OnEnable()
    {
        _buttonOnClickInteraction.BindOnClickButtonEvent(Onclick_Interaction);
    }

    private void OnDisable()
    {
        _onClickCallback = null;
    }

    public void Onclick_Interaction()
    {
        _onClickCallback?.Invoke(_interactionCallbackMsg);
    }

    public void InitSlot(int instanceId, string interactionTitle, string interactionKey, Transform targetTransform, Action<string> onClickCallback = null)
    {
        _instanceId = instanceId;
        _targetTransform = targetTransform;

        _textKeyName.text = interactionKey;
        _textInteractionTitle.text = interactionTitle;

        slotOffsetX = -240;
        slotOffsetY = 115;
    }

    private void Update()
    {
        if (_targetTransform != null)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(_targetTransform.position);

            var rectTransform = this.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector2 finalScreenPos = new Vector2(screenPos.x + slotOffsetX, screenPos.y + slotOffsetY);
                rectTransform.anchoredPosition = finalScreenPos;
            }
        }
    }
}
