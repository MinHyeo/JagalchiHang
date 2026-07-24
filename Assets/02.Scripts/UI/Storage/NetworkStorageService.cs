using UnityEngine;

public class NetworkStorageService
{
    private StorageViewModel _localStorageViewModel;

    public StorageViewModel GetLocalStorageViewModel()
    {
        if (_localStorageViewModel == null)
        {
            CreateLocalStorageViewModel();
        }

        return _localStorageViewModel;
    }

    private StorageViewModel CreateLocalStorageViewModel()
    {
        GameDataManager.Instance.LoadData<ItemData>();
        var storageVm = new StorageViewModel();
        storageVm.AddStorageSlotViewModel();
        _localStorageViewModel = storageVm;
        return storageVm;
    }
}
