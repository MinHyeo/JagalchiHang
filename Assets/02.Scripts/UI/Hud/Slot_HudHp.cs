using UnityEngine;
using UnityEngine.UI;

public class Slot_HudHp : MonoBehaviour
{
    [SerializeField] private int _slotOffsetX;
    [SerializeField] private int _slotOffsetY;
    [SerializeField] private Slider _sliderHp;

    private int _instanceId;
    private Transform _targetTransform;

    public void InitSlot(int instanceId, Transform targetTransform)
    {
        _instanceId = instanceId;
        _targetTransform = targetTransform;
        _slotOffsetX = -960; // TODO : 수정 필요
        _slotOffsetY = -540; // TODO : 수정 필요

        TryBingStatChangedEvent(targetTransform.gameObject);
    }

    private void TryBingStatChangedEvent(GameObject gObj)
    {
        // TODO : 몬스터와 연동 필요
        //var enemy = gObj.GetComponent<>();
        //if (enemy != null)
        //{
        //    enemy.BindeOnStatChangedEvent(OnTargetEntitiyHpChanged);
        //    return;
        //}
    }

    private void OnTargetEntitiyHpChanged(float curHp, float maxHp)
    {
        _sliderHp.value = (curHp / maxHp);
    }

    private void Update()
    {
        if (_targetTransform != null)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(_targetTransform.position);

            var rectTransform = this.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector2 finalScreenPos = new Vector2(screenPos.x + _slotOffsetX, screenPos.y + _slotOffsetY);
                rectTransform.anchoredPosition = finalScreenPos;
            }
        }
    }
}
