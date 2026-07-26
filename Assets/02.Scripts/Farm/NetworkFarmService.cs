using UnityEngine;

public class NetworkFarmService
{
    private FarmViewModel _farmViewModel;

    public FarmViewModel GetFarmViewModel()
    {
        if(_farmViewModel == null)
        {
            var farmViewModel = new FarmViewModel();
            _farmViewModel = farmViewModel;

            //FarmPlotModel 
        }

        return _farmViewModel;
    }
}