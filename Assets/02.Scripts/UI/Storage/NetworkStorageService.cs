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
        var storageVm = new StorageViewModel();
        _localStorageViewModel = storageVm;
        storageVm.AddStorageSlotViewModel();
        return storageVm;
    }
}
