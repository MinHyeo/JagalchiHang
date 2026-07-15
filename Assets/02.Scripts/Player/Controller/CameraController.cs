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
            var lens = _camera.Lens;
            lens.FieldOfView -= scroll * _sensitivity;
            lens.FieldOfView = Mathf.Clamp(lens.FieldOfView, _minZoomFOV, _maxZoomFOV);

            _camera.Lens = lens;
        }
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
}
