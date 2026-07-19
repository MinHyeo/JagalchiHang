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
        HideBuilding();
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

    private void HideBuilding()
    {
        var playerPos = SampleGameManager.Instance.GetPlayerPosition();
        var cameraPos = this.transform.position;

        var direction = (playerPos - cameraPos).normalized;
        var distance = Vector3.Distance(cameraPos, playerPos);

        float radius = 0.5f;

        RaycastHit[] hits = Physics.SphereCastAll(cameraPos, radius, direction, distance, _buildingLayer);

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                Debug.Log($"감지된 건물: {hit.collider.name}");
                var renderer = hit.collider.GetComponent<Renderer>();
                Color color = renderer.material.color;
                color.a = 0.5f;
                renderer.material.color = color;
            }
        }
    }
}
