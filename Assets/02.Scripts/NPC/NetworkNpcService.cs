using UnityEngine;

public class NetworkNpcService : MonoBehaviour
{
    private NpcViewModel _npcViewModel;

    public NpcViewModel GetNpcViewModel()
    {
        if (_npcViewModel == null)
        {
            var npcViewModel = new NpcViewModel();
            _npcViewModel = npcViewModel;
        }

        return _npcViewModel;
    }
}
