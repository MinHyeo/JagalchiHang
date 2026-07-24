using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BuildingVisibility : MonoBehaviour
{
    [SerializeField] private LayerMask _excludeLayer;     // 제외할 오브젝트 레이어

    private List<Renderer> _renderers = new List<Renderer>();          // Renderer들을 저장하는 List
    private ShadowCastingMode[] _originalShadowModes;                  // 어떤 Shadow Mode였는지 저장하는 배열

    private bool _playerInside;
    private int _currentPlayerFloor;

    private BuildingFloorVisibility[] _floors;

    private void Awake()
    {
        InitRenderers();
        _floors = GetComponentsInChildren<BuildingFloorVisibility>(true);
    }

    private void InitRenderers()
    {
        // 모든 자식 오브젝트에서 Renderer를 찾아 배열로 반환
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer targetRenderer in childRenderers)
        {
            // 특정 오브젝트의 Layer가 _excludeLayer가 아니라면
            if (IsInLayerMask(targetRenderer.gameObject.layer, _excludeLayer) == true)
            {
                continue;
            }

            // 리스트에 저장
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

    public void SetPlayerInside(bool isInSide)
    {
        _playerInside = isInSide;

        if (_playerInside == true)
        {
            Show();

            SetPlayerFloor(_currentPlayerFloor);
        }
    }

    public void SetPlayerFloor(int playerFloor)
    {
        if(playerFloor == 0)
        {
            HideRenderers();
            return;
        }

        foreach (var floor in _floors)
        {
            if (floor.FloorNumber > playerFloor)
            {
                floor.Hide();
            }
            else
            {
                floor.Show();
            }
        }
    }

    // 건물 숨기기 - 플레이어가 건물 내부에 있을 경우에는 건물을 숨기지 않음
    public void Hide()
    {
        if (_playerInside == true)
        {
            return;
        }

        HideRenderers();
    }

    // _playerInside 상태를 확인하지 않고 건물을 강제로 숨김
    private void HideRenderers()
    {
        foreach (var targetRenderer in _renderers)
        {
            // ShadowsOnly: 오브젝트 메시는 화면에 안 보이고 그림자는 보임
            targetRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
    }

    // 건물 보여주기
    public void Show()
    {
        for (int i = 0; i < _renderers.Count; i++)
        {
            // 처음에 저장해뒀던 값 다시 넣기
            _renderers[i].shadowCastingMode = _originalShadowModes[i];
        }

    }

    //특정 레이어가 LayerMask에 포함돼있는지 확인
    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }
}
