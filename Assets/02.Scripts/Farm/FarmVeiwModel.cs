using System.Collections.Generic;
using UnityEngine;

public class FarmViewModel : ViewModelBase
{
    public List<FarmPlotModel> FarmPlotList = new List<FarmPlotModel>();
    public Dictionary<int, FarmPlot> FarmPlotDictionary = new Dictionary<int, FarmPlot>();

    private FarmManager _farmManager;

    public void SetFarmManager(FarmManager farmManager)
    {
        _farmManager = farmManager;
    }

    public FarmManager GetFarmManager()
    {
        return _farmManager;
    }
}