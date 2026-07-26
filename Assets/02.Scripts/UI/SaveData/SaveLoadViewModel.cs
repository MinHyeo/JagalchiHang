using System.Collections.Generic;
using UnityEngine;

public class SaveLoadViewModel : ViewModelBase
{
    private int _saveCount = 10;
    public int SaveCount => _saveCount;

    private List<SaveModel> _saveModelList = new List<SaveModel>();
    public List<SaveModel> SaveModelList
    {
        get { return _saveModelList; }
        set
        {
            if(_saveModelList != value)
            {
                _saveModelList = value;
                OnPropertyChanged(nameof(SaveModelList));
            }
        }
    }

    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(SaveModelList));
    }
}