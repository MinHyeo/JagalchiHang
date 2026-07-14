using System;
using System.Collections.Generic;

[Serializable]
public class PlayerModel
{
    public string Name;

    public int CurrentHp;
    public int CurrentHunger;
    public int CurrentThirst;

    public List<FarmPlotModel> FarmPlotList = new List<FarmPlotModel>();
}
