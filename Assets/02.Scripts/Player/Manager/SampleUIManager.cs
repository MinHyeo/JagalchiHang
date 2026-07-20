using System.Collections.Generic;
using UnityEngine;

public class SampleUIBase : MonoBehaviour
{

}

public class SampleUIManager : SingletonBase<SampleUIManager>
{
    [SerializeField] private Canvas _canvasBgRoot;
    [SerializeField] private Canvas _canvasMainRoot;
    [SerializeField] private Canvas _canvasContentRoot;
    [SerializeField] private Canvas _canvasPopupRoot;
    [SerializeField] private Canvas _canvasVeryFrontRoot;

    private Dictionary<SampleUIType, SampleUIBase> _createdUIList = new Dictionary<SampleUIType, SampleUIBase>();
    private HashSet<SampleUIType> _opendUIList = new HashSet<SampleUIType>();

    private void Start()
    {
        this.ShowStartupUIOnGameStart();
    }

    public SampleUIBase OpenUI(SampleUIRootType uiRootType, SampleUIType uiType, bool isInitialHide = false)
    {
        // 딱히 요청이 있진 않고 오픈만 하면 되는 UI에서 사용

        var openedUI = GetCreatedUI(uiRootType, uiType);

        bool isSetActiveOnOpen = (isInitialHide == false); // 열었을 때 기본적으로 숨겨서 열 것인지 체크
        if (_opendUIList.Contains(uiType) == false)
        {
            openedUI.gameObject.SetActive(isSetActiveOnOpen);
            _opendUIList.Add(uiType);
        }

        return openedUI;
    }

    public void CloseUI(SampleUIRootType uiRootType, SampleUIType uiType)
    {
        if (_opendUIList.Contains(uiType))
        {
            var openedUi = _createdUIList[uiType];
            openedUi.gameObject.SetActive(false);
            _opendUIList.Remove(uiType);
        }
    }

    private Transform GetRootTransform(SampleUIRootType uiRootType)
    {
        Transform root = null;
        switch (uiRootType)
        {
            case SampleUIRootType.BackgroundUI:
                root = _canvasBgRoot.transform;
                break;
            case SampleUIRootType.MainUI:
                root = _canvasMainRoot.transform;
                break;
            case SampleUIRootType.ContentUI:
                root = _canvasContentRoot.transform;
                break;
            case SampleUIRootType.PopupUI:
                root = _canvasPopupRoot.transform;
                break;
            case SampleUIRootType.VeryFrontUI:
                root = _canvasVeryFrontRoot.transform;
                break;
        }
        return root;
    }

    public void CreateUI(SampleUIRootType uiRootType, SampleUIType uiType)
    {
        if (_createdUIList.ContainsKey(uiType) == false)
        {
            string path = this.GetUIPath(uiRootType, uiType);
            GameObject loadedObj = Resources.Load<GameObject>(path);
            Transform root = GetRootTransform(uiRootType);
            GameObject gObj = Instantiate(loadedObj, root);
            if (gObj != null)
            {
                var uiBase = gObj.GetComponent<SampleUIBase>();
                _createdUIList.Add(uiType, uiBase);
            }
        }
    }

    public SampleUIBase GetCreatedUI(SampleUIRootType uiRootType, SampleUIType uiType)
    {
        if (_createdUIList.ContainsKey(uiType) == false)
        {
            CreateUI(uiRootType, uiType);
        }
        return _createdUIList[uiType];
    }

    public SampleUIBase GetOpenUI(SampleUIRootType uiRootType, SampleUIType uiType)
    {
        return GetCreatedUI(uiRootType, uiType);
    }
}
