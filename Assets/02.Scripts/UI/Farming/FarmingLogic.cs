using System.Collections.Generic;
using UnityEngine;

public static class FarmingLogic
{
    public static List<ItemData> SelectRandomItems(List<ItemData> pool, int countToSelect)
    {
        List<ItemData> result = new List<ItemData>();

        List<ItemData> activePool = new List<ItemData>(pool);

        activePool.RemoveAll(item => item.DropWeight <= 0);

        int actualCount = Mathf.Min(countToSelect, activePool.Count);

        for (int i = 0; i < actualCount; i++)
        {
            int totalWeight = 0;
            foreach (var item in activePool)
            {
                totalWeight += item.DropWeight;
            }

            if (totalWeight <= 0) break;

            int randomValue = Random.Range(0, totalWeight);
            int currentSum = 0;
            int selectedIndex = -1;

            for (int j = 0; j < activePool.Count; j++)
            {
                currentSum += activePool[j].DropWeight;
                if (randomValue < currentSum)
                {
                    selectedIndex = j;
                    break;
                }
            }

            if (selectedIndex != -1)
            {
                result.Add(activePool[selectedIndex]);
                activePool.RemoveAt(selectedIndex);
            }
        }

        return result;
    }
}
