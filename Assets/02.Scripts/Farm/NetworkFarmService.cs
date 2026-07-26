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

    public FarmSaveModel GetSaveData()
    {
        var farmSaveModel = new FarmSaveModel();
        farmSaveModel.FarmPlotList = GetFarmViewModel().FarmPlotList;
        return farmSaveModel;
    }

    public void LoadSaveData(FarmSaveModel farmSaveModel)
    {
        if (farmSaveModel == null)
        {
            return;
        }

        Debug.Log($"Farm 로드됨, 밭 개수: {farmSaveModel.FarmPlotList?.Count}");

        if (_farmViewModel == null)
        {
            GetFarmViewModel();
        }
        
        _farmViewModel.FarmPlotList = farmSaveModel.FarmPlotList;
    }
}