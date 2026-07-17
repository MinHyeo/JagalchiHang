using System.Collections.Generic;
using UnityEngine;

public class HudMainUI : UIBase
{
    [SerializeField] private GameObject _prefabSlotHudHp;
    [SerializeField] private GameObject _prefabSlotHudInteraction;
    [SerializeField] private Transform _transformSlotRoot;

    private Dictionary<int, Slot_HudHp> _slotHudHpList = new Dictionary<int, Slot_HudHp>();


    public void AddSlotHudHp(int instanceId, Transform targetTransform)
    {
        CreateSlotHudHp(instanceId, targetTransform);
    }

    private void CreateSlotHudHp(int instanceId, Transform targetTransform)
    {
        var gObj = Instantiate(_prefabSlotHudHp, _transformSlotRoot);
        if (gObj == null)
        {
            return;
        }

        var slotComponent = gObj.GetComponent<Slot_HudHp>();
        if (slotComponent == null)
        {
            return;
        }

        slotComponent.InitSlot(instanceId, targetTransform);

        _slotHudHpList.Add(instanceId, slotComponent);
    }

    public void RemoveSlotHudHp(int instanceId)
    {
        if (_slotHudHpList.ContainsKey(instanceId) == true)
        {
            var slot = _slotHudHpList[instanceId];

            Destroy(slot.gameObject);
            _slotHudHpList.Remove(instanceId);
        }
    }

    public void RemoveAllSlotHudHp()
    {
        foreach (var slot in _slotHudHpList)
        {
            var slotKv = slot.Value;

            if (slotKv != null)
            {
                Destroy(slotKv.gameObject);
            }
        }

        _slotHudHpList.Clear();
    }
}
