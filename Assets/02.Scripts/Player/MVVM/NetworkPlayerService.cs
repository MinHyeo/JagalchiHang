using UnityEngine;

public class NetworkPlayerService
{
    private PlayerViewModel _playerViewModel;

    public PlayerViewModel GetPlayerViewModel()
    {
        if(_playerViewModel == null)
        {
            // 뷰모델 일단 생성
            var playerViewModel = new PlayerViewModel();
            SetPlayerStatData(playerViewModel, curHp: 0, curHunger: 0, curThirst : 0);
            _playerViewModel = playerViewModel;

            // 데이터를 받아오고 성공하면 내용 채워줌
            var playerData = GameDataManager.Instance.GetData<PlayerData>("Player_1");
            if(playerData == null)
            {
                Debug.LogError($"플레이어 데이터를 찾을 수 없습니다!");
                return _playerViewModel;
            }

            SetPlayerStatData(playerViewModel, curHp: playerData.MaxHp, curHunger: playerData.MaxHunger, curThirst: playerData.MaxThirst);
        }

        return _playerViewModel;
    }

    private void SetPlayerStatData(PlayerViewModel vm, int curHp, int curHunger, int curThirst)
    {
        vm.CurrentHp = curHp;
        vm.CurrentHunger = curHunger;
        vm.CurrentThirst = curThirst;
    }
}
