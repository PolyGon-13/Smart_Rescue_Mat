using UnityEngine;

public class CameraFollowWithOffset : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Vector3 offset; // 시작 시점의 간격을 저장할 변수

    void Start()
    {
        if (target != null)
        {
            // 시작할 때의 위치 차이 계산 (카메라 pos - 타겟 pos)
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. 타겟의 현재 위치에 저장해둔 오프셋을 더함 (XZ 유지)
        Vector3 targetPosWithOffset = target.position + offset;

        // 2. Y 좌표만 타겟의 현재 Y와 동일하게 맞춤 (요청 사항)
        // 만약 Y도 시작 간격을 유지하고 싶다면 이 줄을 지우면 돼!
        targetPosWithOffset.y = target.position.y;

        // 3. 최종 위치 적용
        transform.position = targetPosWithOffset;
    }
}