using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BuildingFloorVisibility : MonoBehaviour
{
    [SerializeField] private int _floorNumber;

    private List<Renderer> _renderers = new List<Renderer>();
    private ShadowCastingMode[] _originalShadowModes;

    public int FloorNumber => _floorNumber;

    private void Awake()
    {
        InitRenderers();
    }

    private void InitRenderers()
    {
        // 모든 자식 오브젝트에서 Renderer를 찾아 배열로 반환
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer targetRenderer in childRenderers)
        {
            _renderers.Add(targetRenderer);
        }

        // Renderer 개수만큼 ShadowMode 배열 생성
        _originalShadowModes = new ShadowCastingMode[_renderers.Count];

        for (int i = 0; i < _renderers.Count; i++)
        {
            // 현재 Renderer의 ShadowMode 저장
            _originalShadowModes[i] = _renderers[i].shadowCastingMode;
        }
    }

    public void Hide()
    {
        foreach (var targetRenderer in _renderers)
        {
            targetRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            Debug.Log(
            $"{gameObject.name} / {targetRenderer.name} 숨김 / " +
            $"{targetRenderer.shadowCastingMode}");
        }
    }

    public void Show()
    {
        for (int i = 0; i < _renderers.Count; i++)
        {
            // 처음에 저장해뒀던 값 다시 넣기
            _renderers[i].shadowCastingMode = _originalShadowModes[i];

            Debug.Log(
            $"{gameObject.name} / {_renderers[i].name} 보임 / " +
            $"{_renderers[i].shadowCastingMode}"
        );
        }
    }
}
