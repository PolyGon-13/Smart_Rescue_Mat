using UnityEngine;

public class CameraSequenceController : MonoBehaviour
{
    [Header("대상 설정")]
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 3f;

    private Vector3 _startOffset;
    private float _initialTargetY;
    private float _initialTargetZ;
    private float _initialYDistance; 
    private Quaternion _initialRotation;

    private bool _zTriggered = false; 
    private bool _yTriggered = false; 

    void Start()
    {
        if (target == null) return;

        // 1. 모든 축(X, Y, Z)의 간격을 그대로 저장 (정렬 방지)
        _startOffset = transform.position - target.position;
        
        _initialTargetY = target.position.y;
        _initialTargetZ = target.position.z;
        _initialRotation = transform.rotation;
        
        // Y축 거리 유지용 값 (나중에 1m 추락 후 사용)
        _initialYDistance = Mathf.Abs(transform.position.y - target.position.y);

        // [중요] 시작하자마자 위치를 타겟 + 오프셋 지점으로 순간이동 시켜서 Lerp 튐 방지
        transform.position = target.position + _startOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        CheckTriggers();
        UpdateCamera();
    }

    private void CheckTriggers()
    {
        if (!_zTriggered && Mathf.Abs(target.position.z - _initialTargetZ) > 0.01f)
        {
            _zTriggered = true;
        }

        if (_zTriggered && !_yTriggered && (_initialTargetY - target.position.y >= 1.0f))
        {
            _yTriggered = true;
        }
    }

    private void UpdateCamera()
    {
        Vector3 targetPos;
        Quaternion targetRot;

        if (_yTriggered)
        {
            // [3단계] 추락 중: XZ는 캐릭터와 일치, Y는 초기 높이차 유지
            targetPos = new Vector3(target.position.x, target.position.y + _initialYDistance, target.position.z);
            targetRot = Quaternion.Euler(90f, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else if (_zTriggered)
        {
            // [2단계] Z이동 발생: Z는 캐릭터와 일치(거리무시), X축 90도 회전
            targetPos = new Vector3(target.position.x + _startOffset.x, target.position.y + _startOffset.y, target.position.z);
            targetRot = Quaternion.Euler(90f, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else
        {
            // [1단계] 초기 상태: 인스펙터에 배치한 그 위치(X, Y, Z 오프셋) 그대로 유지
            targetPos = target.position + _startOffset;
            targetRot = _initialRotation;
        }

        // 부드럽게 이동 및 회전
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }
}