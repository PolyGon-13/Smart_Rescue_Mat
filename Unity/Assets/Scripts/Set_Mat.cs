using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LegController : MonoBehaviour
{
    [Header("Leg Settings")]
    public List<ArticulationBody> leg1 = new List<ArticulationBody>(4);
    public List<ArticulationBody> leg2 = new List<ArticulationBody>(4);

    [Header("Movement Settings")]
    [Tooltip("이동하는 데 걸리는 시간(초)이야. 이 값을 키우면 더 천천히 움직여!")]
    public float moveDuration = 1.0f; 

    [Header("Mat Settings")]
    public GameObject mat;

    void Start()
    {
        // 시작할 때 바로 이동시키기 (이건 즉시 이동하거나 짧은 Lerp를 줄 수 있어)
        SetAllLegTargetsImmediately(true);
        if (mat != null) mat.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StopAllCoroutines(); // 혹시 실행 중인 동작이 있다면 멈추고 새로 시작
            StartCoroutine(LegActionSequence());
        }
    }

    // 처음 시작할 때처럼 즉시 위치를 잡아야 할 때 쓰는 함수
    void SetAllLegTargetsImmediately(bool isLower)
    {
        foreach (var ab in leg1) MoveImmediately(ab, isLower);
        foreach (var ab in leg2) MoveImmediately(ab, isLower);
    }

    void MoveImmediately(ArticulationBody ab, bool isLower)
    {
        if (ab == null) return;
        var drive = ab.xDrive;
        drive.target = isLower ? drive.lowerLimit : drive.upperLimit;
        ab.xDrive = drive;
    }

    // --- 핵심: Lerp를 이용한 부드러운 이동 코루틴 ---
    IEnumerator SmoothMove(List<ArticulationBody> legs, float targetValue, float duration)
    {
        float elapsed = 0f;
        
        // 각 다리의 현재 시작 지점을 저장해둬야 정확한 Lerp가 가능해
        float[] startValues = new float[legs.Count];
        for (int i = 0; i < legs.Count; i++)
        {
            startValues[i] = legs[i].xDrive.target;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration); // SmoothStep을 쓰면 가감속이 생겨서 더 자연스러워!

            for (int i = 0; i < legs.Count; i++)
            {
                if (legs[i] == null) continue;
                var drive = legs[i].xDrive;
                // 시작점부터 목표점까지 서서히 보간
                drive.target = Mathf.Lerp(startValues[i], targetValue, t);
                legs[i].xDrive = drive;
            }
            yield return null; // 다음 프레임까지 대기
        }
    }

    IEnumerator LegActionSequence()
    {
        // 1. 모든 다리를 동시에 upperLimit으로 부드럽게 이동
        Debug.Log("부드럽게 Upper Limit으로 이동 중...");
        
        // 두 리스트를 동시에 움직이기 위해 각각 코루틴을 돌려도 되지만, 
        // 관리하기 편하게 하나로 합쳐서 실행하거나 아래처럼 작성해
        yield return StartCoroutine(MoveBothLegsTo(true)); // 아래 정의된 헬퍼 함수 사용

        yield return new WaitForSeconds(1f);

        // 2. 모든 다리를 동시에 0으로 부드럽게 이동
        Debug.Log("부드럽게 0으로 이동 중...");
        yield return StartCoroutine(MoveBothLegsTo(false));

        yield return new WaitForSeconds(1f);

        // 3. mat 활성화
        if (mat != null)
        {
            mat.SetActive(true);
            Debug.Log("매트 등장!");
        }
    }

    // 편의를 위해 leg1, leg2를 같이 움직여주는 헬퍼 함수
    IEnumerator MoveBothLegsTo(bool toUpper)
    {
        float elapsed = 0f;
        int totalCount = leg1.Count + leg2.Count;
        float[] startValues = new float[totalCount];
        List<ArticulationBody> allLegs = new List<ArticulationBody>();
        allLegs.AddRange(leg1);
        allLegs.AddRange(leg2);

        for (int i = 0; i < allLegs.Count; i++)
            startValues[i] = allLegs[i].xDrive.target;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / moveDuration);

            for (int i = 0; i < allLegs.Count; i++)
            {
                if (allLegs[i] == null) continue;
                var drive = allLegs[i].xDrive;
                float endValue = toUpper ? drive.upperLimit : 0f;
                drive.target = Mathf.Lerp(startValues[i], endValue, t);
                allLegs[i].xDrive = drive;
            }
            yield return null;
        }
    }
}