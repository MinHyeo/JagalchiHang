using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _myCamera;
    [SerializeField] private float _sensitivity = 5.0f;
    [SerializeField] private float _minZoomFOV = 30f;
    [SerializeField] private float _maxZoomFOV = 80f;

    private static CinemachineCamera _camera;
    private LensSettings _lens;

    [SerializeField]
    private LayerMask _buildingLayer;

    private HashSet<BuildingVisibility> _hiddenBuildings = new HashSet<BuildingVisibility>();      // 숨겨진 건물 목록
    private HashSet<BuildingVisibility> _detectedBuildings = new HashSet<BuildingVisibility>();    // 새롭게 감지된 건물 목록

    private void Awake()
    {
        _camera = _myCamera;
    }

    private void Update()
    {
        // 현재 마우스 휠 입력값 가져옴
        float scroll = Mouse.current.scroll.ReadValue().y;

        // 휠 입력이 있다면
        if (Mathf.Abs(scroll) > 0.01f)
        {
            _lens = _camera.Lens;
            _lens.FieldOfView -= scroll * _sensitivity;
            _lens.FieldOfView = Mathf.Clamp(_lens.FieldOfView, _minZoomFOV, _maxZoomFOV);

            _camera.Lens = _lens;
        }
    }

    private void LateUpdate()
    {
        HideBuildings();
    }

    // 플레이어를 TrackingTarget으로 설정
    public static void SetTrackingTarget(Transform targetTransform)
    {
        if(targetTransform != null)
        {
            _camera.Target.TrackingTarget = targetTransform;
        }

        return;
    }

    // 건물 숨기기
    private void HideBuildings()
    {
        var playerPos = _camera.Target.TrackingTarget.position;
        var cameraPos = this.transform.position;

        var direction = (playerPos - cameraPos).normalized;
        var distance = Vector3.Distance(cameraPos, playerPos);

        // 거리가 너무 짧으면 return
        if (distance <= 0.01f)
        {
            return;
        }

        float radius = 0.5f;

        // 카메라 위치에서 플레이어 방향으로 구 형태의 검사 보내기
        RaycastHit[] hits = Physics.SphereCastAll(cameraPos, radius, direction, distance, _buildingLayer);

        // 감지된 건물 리스트 지우기
        _detectedBuildings.Clear();

        foreach (RaycastHit hit in hits)
        {
            // 직접 맞힌 Collider의 부모의 BuildingVisibility 컴포넌트 찾기
            BuildingVisibility building = hit.collider.GetComponentInParent<BuildingVisibility>();

            if (building == null)
            {
                continue;
            }

            _detectedBuildings.Add(building);
            building.Hide();

            Debug.Log($"감지된 건물: {building.name}");
        }

        // 이전에 숨겼던 건물 확인
        foreach (BuildingVisibility hiddenBuilding in _hiddenBuildings)
        {
            // 감지된 건물에 숨겼던 건물이 없다면
            if (_detectedBuildings.Contains(hiddenBuilding) == false)
            {
                // 다시 보여줘
                hiddenBuilding.Show();
            }
        }

        // 숨겼던 건물 리스트 지우기
        _hiddenBuildings.Clear();

        // 현재 감지된 건물 확인
        foreach (BuildingVisibility detectedBuilding in _detectedBuildings)
        {
            // 감지돼서 숨긴 건물들 _hiddenBuildings 리스트에 저장
            _hiddenBuildings.Add(detectedBuilding);
        }
    }
}

