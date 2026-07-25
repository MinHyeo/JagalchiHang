using System.ComponentModel;
using UnityEngine;

public class ElectricGenerator : MonoBehaviour
{
    private GeneratorViewModel _vm;

    private ParticleSystem _electricParticle;
    private RotateSubmarineBlades _rotateBlades;

    private void Awake()
    {
        _electricParticle = GetComponent<ParticleSystem>();
        _rotateBlades = GetComponent<RotateSubmarineBlades>();
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        _vm = NetworkManager.Instance.GeneratorService.GeneratorViewModel();
        _vm.PropertyChanged += OnPropertyChanged;
    }

    private void OnDisable()
    {
        _vm.PropertyChanged -= OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName) 
        {
            case nameof(GeneratorViewModel.IsStoped):
                UpdateVisual();
                break;
        }
    }

    private void UpdateVisual()
    {
        _rotateBlades.rotate = _vm.IsStoped == false;

        if(_vm.IsStoped == true)
        {
            _electricParticle.Play();
        }
        else
        {
            _electricParticle.Stop();
        }
    }
}