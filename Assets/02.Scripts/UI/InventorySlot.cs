
[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int count;

    public InventorySlot()
    {
        ClearSlot();
    }

    public void AddItem(ItemData newItem, int amount)
    {
        item = newItem;
        count += amount;
    }

    public void ClearSlot()
    {
        item = null;
        count = 0;
    }
}
