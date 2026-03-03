using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class FullBodyPhysicsController : MonoBehaviour
{
    [Header("[ Current Body Parts ]")]
    public List<GameObject> leftArm = new List<GameObject>();
    public List<GameObject> rightArm = new List<GameObject>();
    public List<GameObject> leftLeg = new List<GameObject>();
    public List<GameObject> rightLeg = new List<GameObject>();
    public List<GameObject> chest = new List<GameObject>();

    [Header("[ Target Rotations ]")]
    public List<Transform> targetLeftArm = new List<Transform>();
    public List<Transform> targetRightArm = new List<Transform>();
    public List<Transform> targetLeftLeg = new List<Transform>();
    public List<Transform> targetRightLeg = new List<Transform>();
    public List<Transform> targetChest = new List<Transform>();

    [Header("[ Settings ]")]
    public float rotationSpeed = 2.0f;
    public float moveSpeed = 5.0f;
    public float yThreshold = 1.0f;
    public string buildingLayerName = "Building";

    // 내부 상태 변수
    private Rigidbody rb;
    private float initialY;
    private float targetZLimit;
    private bool isMovingZ = false;
    private bool rotationStarted = false;

    void Start()
    {
        // 1. 리지드바디 참조 및 초기 설정
        rb = GetComponent<Rigidbody>();
        initialY = transform.position.y;

        if (rb != null)
        {
            // 물리 기반 이동 시 화면 떨림을 방지해주는 아주 중요한 설정이야!
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    void Update()
    {
        // 2. 1번 키를 누르면 Z축으로 +2만큼 이동 예약
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            targetZLimit = transform.position.z + 13.0f;
            isMovingZ = true;
        }

        // 3. Y축 변화가 1만큼 생기면 회전 및 충돌 무시 로직 실행
        if (!rotationStarted && Mathf.Abs(transform.position.y - initialY) >= yThreshold)
        {
            rotationStarted = true;
            DisableBuildingCollision();
            Debug.Log("낙하 감지: 신체 회전 시작 및 Building 레이어 충돌 무시");
        }
    }

    void FixedUpdate()
    {
        // 4. 물리 이동은 FixedUpdate에서 처리하는 게 정석이야.
        if (isMovingZ)
        {
            HandleZMovement();
        }
    }

    void LateUpdate()
    {
        // 5. 신체 회전은 모든 물리/애니메이션 연산이 끝난 LateUpdate에서 덮어씌워야 부드러워.
        if (rotationStarted)
        {
            ApplyAllRotations();
        }
    }

    // --- 주요 기능 함수들 ---

    void HandleZMovement()
    {
        // 목표 Z 지점에 도달하면 멈춰
        if (transform.position.z >= targetZLimit)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, 0);
            isMovingZ = false;
            return;
        }

        // 현재 수직 속도(중력)는 유지하면서 Z축으로만 속도를 줘
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, moveSpeed);
    }

    void DisableBuildingCollision()
    {
        int myLayer = gameObject.layer;
        int buildingLayer = LayerMask.NameToLayer(buildingLayerName);

        if (buildingLayer != -1)
        {
            // 물리 엔진 설정에서 두 레이어 간의 충돌을 꺼버려
            Physics.IgnoreLayerCollision(myLayer, buildingLayer, true);
        }
    }

    void ApplyAllRotations()
    {
        RotatePart(leftArm, targetLeftArm);
        RotatePart(rightArm, targetRightArm);
        RotatePart(leftLeg, targetLeftLeg);
        RotatePart(rightLeg, targetRightLeg);
        RotatePart(chest, targetChest);
    }

    void RotatePart(List<GameObject> parts, List<Transform> targets)
    {
        int count = Mathf.Min(parts.Count, targets.Count);
        for (int i = 0; i < count; i++)
        {
            if (parts[i] == null || targets[i] == null) continue;

            // Slerp를 사용하여 부드러운 회전 보간 처리
            parts[i].transform.rotation = Quaternion.Slerp(
                parts[i].transform.rotation,
                targets[i].rotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }
}