using UnityEditor.Search;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _objectToFollow;      // 따라갈 오브젝트 정보
    [SerializeField] private float _followSpeed = 100f;       // 따라갈 속도
    [SerializeField] private float _sensitivity = 600f;      // 마우스 감도
    [SerializeField] private float _clampAngle = 70f;        // 상하로 움직일 때 각도 제한

    [SerializeField] private Transform _realCamera;          // 카메라 정보

    [SerializeField] private Vector3 _dirNormalized;         // 방향 (어느 방향 보고있는지)
    [SerializeField] private Vector3 _finalDir;              // 최종적인 방향

    [SerializeField] private float _minDistance;              // 최소 거리 (카메라와 플레이어 사이에 방해물이 있을 경우 적용)
    [SerializeField] private float _maxDistance;              // 최대 거리
    [SerializeField] private float _finalDistance;            // 최종적인 거리

    [SerializeField] private float _smoothness = 10f;

    private float _rotX;    // 위아래 회전
    private float _rotY;    // 좌우 회전

    private void Start()
    {
        _rotX = transform.localRotation.eulerAngles.x;
        _rotY = transform.localRotation.eulerAngles.y;

        _dirNormalized = _realCamera.localPosition.normalized;
        _finalDistance = _realCamera.localPosition.magnitude;

        // 커서 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        _rotX -= Input.GetAxis("Mouse Y") * _sensitivity * Time.deltaTime;
        _rotY += Input.GetAxis("Mouse X") * _sensitivity * Time.deltaTime;

        // 카메라의 상하(X축) 회전 각도를 -_clampAngle ~ _clampAngle 범위로 제한
        _rotX = Mathf.Clamp(_rotX, -_clampAngle, _clampAngle);
        Quaternion rot = Quaternion.Euler(_rotX, _rotY, 0);
        transform.rotation = rot;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, _objectToFollow.position, _followSpeed * Time.deltaTime);

        _finalDir = transform.TransformPoint(_dirNormalized * _maxDistance);

        // 플레이어와 카메라 사이에 장애물이 있는지 검사
        RaycastHit hit;

        if(Physics.Linecast(transform.position, _finalDir, out hit))
        {
            // 장애물 있으면 카메라 거리를 최소~최대 범위로 조정
            _finalDistance = Mathf.Clamp(hit.distance, _minDistance, _maxDistance);
        }
        else
        {
            // 장애물 없으면 최대 거리 유지
            _finalDistance = _maxDistance;
        }

        _realCamera.localPosition = Vector3.Lerp(_realCamera.localPosition, _dirNormalized * _finalDistance, Time.deltaTime * _smoothness);
    }
}
