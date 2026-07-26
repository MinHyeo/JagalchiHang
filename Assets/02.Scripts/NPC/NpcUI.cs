using UnityEngine;
using UnityEngine.UI;

public class NpcUI : UIBase
{
    [SerializeField] private Toggle toggleAutoAttack;
    [SerializeField] private Toggle toggleAssistAttack;
    [SerializeField] private Toggle toggleFollow;

    [Header("NPC 에너지")]
    [SerializeField] private Slider sliderBattleNpc;
    [SerializeField] private Slider sliderBagNpc;

    private NpcManager _npcManager;

    private void OnEnable()
    {
        toggleAutoAttack.onValueChanged.AddListener(OnAutoAttackChanged);
        toggleAssistAttack.onValueChanged.AddListener(OnAssistAttackChanged);
        toggleFollow.onValueChanged.AddListener(OnFollowChanged);

        _npcManager = GameUtil.GetNpcManager();
    }



    private void OnDisable()
    {
        toggleAutoAttack.onValueChanged.RemoveListener(OnAutoAttackChanged);
        toggleAssistAttack.onValueChanged.RemoveListener(OnAssistAttackChanged);
        toggleFollow.onValueChanged.RemoveListener(OnFollowChanged);
    }

    private void Update()
    {
        BattleNpcEnergy();
        BagNpcEnergy();
    }


    private void BattleNpcEnergy()
    {
        if (sliderBattleNpc ==null || _npcManager == null)
        {
            return;
        }

        BattleNpc battleNpc = _npcManager.GetBattleNpc();

        if(battleNpc == null)
        {
            sliderBattleNpc.value = 1f;
            return;
        }

        if (battleNpc.MaxEnergy > 0)
        {
            sliderBattleNpc.value = (float)battleNpc.CurrentEnergy/ (float)battleNpc.MaxEnergy;
        }
    }

    private void BagNpcEnergy()
    {
        if (sliderBagNpc == null || _npcManager == null)
        {
            return;
        }

        BagNpc bagNpc = _npcManager.GetBagNpc();

        if (bagNpc == null)
        {
            sliderBagNpc.value = 1f;
            return;
        }

        if (bagNpc.MaxEnergy > 0)
        {
            sliderBagNpc.value = (float)bagNpc.CurrentEnergy / (float)bagNpc.MaxEnergy;
        }
    }
    private void OnAutoAttackChanged(bool isOn)  //Npc매니저로 전달
    {
        if( isOn == true)
        {
            NpcManager npcManager = GameUtil.GetNpcManager();

            npcManager.ChangeBattleMode(BattleMode.AutoAttack);
            Debug.Log("[NpcUI] 자동 전투 모드로 변경");
        }
    }

    private void OnAssistAttackChanged(bool isOn)
    {
        if (isOn == true)
        {
            NpcManager npcManager = GameUtil.GetNpcManager();

            npcManager.ChangeBattleMode(BattleMode.AssistAttack);
            Debug.Log("[NpcUI] 협동 공격모드로 변경 ");
        }
    }

    private void OnFollowChanged(bool isOn)
    {
        if (isOn == true)
        {

            NpcManager npcManager = GameUtil.GetNpcManager();

            npcManager.ChangeBattleMode(BattleMode.FollowOnly);
            Debug.Log("[NpcUI] 동행 전용모드로 변경 ");
        }
    }
}
