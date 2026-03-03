using UnityEngine;

public class FollowXZ : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    public float speed = 5f;
    public float yThreshold = 1f; // Y좌표가 이만큼 변하면 추적 시작

    // 내부 상태
    private float initialTargetY; // 타깃의 초기 Y 좌표 저장
    private bool followStarted = false;
    private bool isCaught = false;

    void Start()
    {
        if (target != null)
        {
            // 1. 씬 시작 시 타깃의 초기 Y 좌표를 기록해둬.
            initialTargetY = target.position.y;
        }
    }

    void Update()
    {
        if (target == null || isCaught) return;

        // 2. 아직 추적이 시작되지 않았다면 Y값 변화를 감시해.
        if (!followStarted)
        {
            // Mathf.Abs를 사용해서 위로 1만큼 가든, 아래로 1만큼 가든 변화를 감지해.
            // 만약 '아래로 1만큼 떨어졌을 때'만 하고 싶다면 (initialTargetY - target.position.y >= yThreshold)로 바꿔줘.
            if (Mathf.Abs(target.position.y - initialTargetY) >= yThreshold)
            {
                followStarted = true;
                Debug.Log("타깃의 Y축 변화 감지! 추적을 시작합니다.");
            }
        }

        if (!followStarted) return;

        // XZ만 따라감 (기존 로직 유지)
        Vector3 current = transform.position;
        Vector3 targetPos = new Vector3(target.position.x, current.y, target.position.z+2.7f);
        transform.position = Vector3.Lerp(current, targetPos, Time.deltaTime * speed);
    }

    // --- OnCollisionEnter 및 코루틴 로직은 기존과 동일 ---
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == target && !isCaught)
        {
            isCaught = true;

            Rigidbody myRb = GetComponent<Rigidbody>();
            if (myRb != null)
            {
                myRb.linearVelocity = Vector3.zero;
                myRb.angularVelocity = Vector3.zero;
            }

            Rigidbody targetRb = target.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                StartCoroutine(ChangeConstraintsSequence(targetRb));
            }
        }
    }

    private System.Collections.IEnumerator ChangeConstraintsSequence(Rigidbody rb)
    {
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        yield return new WaitForSeconds(2.0f);
        rb.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(0.5f);
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }
}