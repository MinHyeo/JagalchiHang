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
        _useGeneratorButton.onClick.AddListener(() => _electricalSystem.UseGenerator(useAmount));
        _fullGeneratorButton.onClick.AddListener(() => _electricalSystem.ReCharageGenerator(fullAmount));
        _fixGeneratorButton.onClick.AddListener(_electricalSystem.FixGenerator);
    }
}