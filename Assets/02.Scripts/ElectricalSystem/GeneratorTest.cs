using UnityEngine;
using UnityEngine.UI;

public class GeneratorTest : MonoBehaviour
{
    [Header("전기 시스템")]
    [SerializeField] private ElectricGenerator _electricalSystem;

    [Header("UI")]
    [SerializeField] Button _useGeneratorButton;
    [SerializeField] Button _fullGeneratorButton;
    [SerializeField] Button _fixGeneratorButton;

    [Header("수치")]
    [SerializeField] private int useAmount;
    [SerializeField] private int fullAmount;

    private void OnEnable()
    {
        Invoke("ddd", 10);
    }

    private void ddd()
    {
        _useGeneratorButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Instance.GeneratorService.CanUsePower(useAmount) == false)
                return;

            NetworkManager.Instance.GeneratorService.UsePower(useAmount);
        });
        _fullGeneratorButton.onClick.AddListener(() => NetworkManager.Instance.GeneratorService.ReChargePower(fullAmount));
        _fixGeneratorButton.onClick.AddListener(() => UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.GeneratorUI));
    }
}