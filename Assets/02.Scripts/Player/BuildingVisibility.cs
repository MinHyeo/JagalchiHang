using UnityEngine;
using UnityEngine.Rendering;

public class BuildingVisibility : MonoBehaviour
{
    private Renderer[] _renderers;                      // 건물 안에 있는 모든 Renderer 저장 배열
    private ShadowCastingMode[] _originalShadowModes;   // 어떤 Shadow Mode였는지 저장하는 배열

    private void Awake()
    {
        // 모든 자식의 Renderer 가져오기
        _renderers = GetComponentsInChildren<Renderer>(true);
        // Renderer 개수만큼 ShadowMode 배열 생성
        _originalShadowModes = new ShadowCastingMode[_renderers.Length];

        for(int i = 0; i < _renderers.Length; i++)
        {
            // 현재 Renderer의 ShadowMode 저장
            _originalShadowModes[i] = _renderers[i].shadowCastingMode;
        }
    }

    // 건물 숨기기
    public void Hide()
    {
        foreach(var targetRenderer in _renderers)
        {
            // ShadowsOnly: 오브젝트 메시는 화면에 안 보이고 그림자는 보임
            targetRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
    }

    public void Show()
    {
        for(int i = 0; i < _renderers.Length; i++)
        {
            // 처음에 저장해뒀던 값 다시 넣기
            _renderers[i].shadowCastingMode = _originalShadowModes[i];
        }
    }
}
