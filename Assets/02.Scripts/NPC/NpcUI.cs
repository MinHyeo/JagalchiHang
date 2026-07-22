using UnityEngine;
using UnityEngine.UI;

public class NpcUI : UIBase
{
    [SerializeField] private NpcManager npcManager;
    [SerializeField] private Toggle toggleAutoAttack;
    [SerializeField] private Toggle toggleAssistAttack;
    [SerializeField] private Toggle toggleFollow;

    private void OnEnable()
    {
        toggleAutoAttack.onValueChanged.AddListener(OnAutoAttackChanged);
        toggleAssistAttack.onValueChanged.AddListener(OnAssistAttackChanged);
        toggleFollow.onValueChanged.AddListener(OnFollowChanged);
    }


    private void OnDisable()
    {
        toggleAutoAttack.onValueChanged.RemoveListener(OnAutoAttackChanged);
        toggleAssistAttack.onValueChanged.RemoveListener(OnAssistAttackChanged);
        toggleFollow.onValueChanged.RemoveListener(OnFollowChanged);
    }

    private void OnAutoAttackChanged(bool isOn)  //Npc매니저로 전달
    {
        if( isOn && npcManager != null)
        {
            npcManager.ChangeBattleMode(BattleMode.AutoAttack);
            Debug.Log("[NpcUI] 자동 전투 모드로 변경");
        }
    }

    private void OnAssistAttackChanged(bool isOn)
    {
        if (isOn && npcManager != null)
        {
            npcManager.ChangeBattleMode(BattleMode.AssistAttack);
            Debug.Log("[NpcUI] 협동 공격모드로 변경 ");
        }
    }

    private void OnFollowChanged(bool isOn)
    {
        if (isOn && npcManager != null)
        {
            npcManager.ChangeBattleMode(BattleMode.FollowOnly);
            Debug.Log("[NpcUI] 동행 전용모드로 변경 ");
        }
    }
}
