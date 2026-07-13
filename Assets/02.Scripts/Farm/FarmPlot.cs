using UnityEngine;

public class FarmPlot : MonoBehaviour
{
    [SerializeField] private GameObject Object_PlotSet;

    private int _plotUniqueId;

    private void Awake()
    {
        Object_PlotSet.SetActive(false);
    }

    public void InitPlot(int plotUniqueId)
    {
        _plotUniqueId = plotUniqueId;
    }

    public void ActivatePlot()
    {
        Object_PlotSet.SetActive(true);
    }

}
