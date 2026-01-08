using UnityEngine;

public class MagnetCalculator : MonoBehaviour
{
    [Header("Physics Settings")]
    [Tooltip("자력의 기본 세기 (높게 설정하세요, 예: 300~500)")]
    [SerializeField] private float magneticForce = 400f; // [변경 권장] 멀리서도 밀리게 수치를 올림

    [Tooltip("힘의 최대 상한선 (가까울 때 폭발 방지)")]
    [SerializeField] private float maxForceLimit = 30f; // [추가됨] 이 이상 힘이 커지지 않음

    [Tooltip("미는 힘 비율 (0.5 = 절반)")]

    [SerializeField] private float pushMultiplier = 0.5f;

    [Tooltip("최소 거리 보정")]
    [SerializeField] private float minDistance = 0.5f; // 계산용 최소 거리

    [Tooltip("물체가 이 거리 안으로 들어오면 당기기 멈춤")]
    [SerializeField] private float stopDistance = 1.5f;

    private Rigidbody2D playerRb;
    private PlayerController playerController;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
    }

    public void ApplyMagnetForce(MagneticObject target, Vector2 origin, bool isNorth)
    {
        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
        if (targetRb == null) return;

        Vector2 direction = (Vector2)target.transform.position - origin;
        float distance = direction.magnitude;

        // ▼▼▼ [지능형 감지 로직 추가] ▼▼▼
        // 1. 레이어로 확인 (정석 방법)
        bool isStandingOn = (playerController.currentGroundObject == target.gameObject);

        // 2. [비상 대책] 레이어 설정이 틀렸을 경우, 위치로 강제 감지
        // "거리가 가깝고(2.5m 이내), 총이 박스보다 위에 있다면" -> 밟고 있는 것으로 간주!
        if (!isStandingOn && distance < 2.5f)
        {
            if (origin.y > target.transform.position.y + 0.5f)
            {
                isStandingOn = true;
            }
        }
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        // ▼▼▼ [엘리베이터 모드: 속도 강제 고정] ▼▼▼
        // 밟고 있고 + 당기기(N극) 라면? -> 힘(Force)을 쓰지 않고 속도(Velocity)를 직접 입력!
        if (isStandingOn && isNorth)
        {
            // 원하는 엘리베이터 속도 (이 숫자를 조절하세요: 3 ~ 5 추천)
            float liftSpeed = 5.0f;

            // X축 속도는 유지하고, Y축 속도만 강제로 고정 (로켓 발사 원천 봉쇄)
            targetRb.linearVelocity = new Vector2(targetRb.linearVelocity.x, liftSpeed);
            return; // 여기서 함수 종료! 아래의 AddForce는 실행되지 않음
        }
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        // --- 여기서부터는 밟고 있지 않을 때의 일반 로직 ---

        // 1. [당기기 안전장치] 너무 가까우면 멈춤
        if (isNorth && distance < stopDistance)
        {
            targetRb.linearVelocity = Vector2.zero;
            targetRb.angularVelocity = 0f;
            return;
        }

        // 2. [밀기] 바닥에서 밀 때 Y축 힘 제거
        bool isOnRail = target.currentRail != null;
        if (!isNorth && !isOnRail)
        {
            direction.y = 0;
            direction.Normalize();
        }
        else
        {
            direction = direction.normalized;
        }

        // 3. 힘 계산
        float rawForce = magneticForce / Mathf.Max(distance * distance, minDistance);
        float finalMagnitude = Mathf.Min(rawForce, maxForceLimit);

        float polarityModifier = isNorth ? -1f : (1f * pushMultiplier);
        Vector2 finalForce = direction * finalMagnitude * polarityModifier * target.magneticSensitivity;

        targetRb.AddForce(finalForce, ForceMode2D.Force);

        // 4. 속도 제한
        float maxSpeed = 15f;
        if (targetRb.linearVelocity.magnitude > maxSpeed)
        {
            targetRb.linearVelocity = targetRb.linearVelocity.normalized * maxSpeed;
        }
    }
}