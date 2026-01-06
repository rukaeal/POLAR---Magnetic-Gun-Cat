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

        // 1. [당기기] 충돌 방지: 플레이어 근처(거리 4)에 오면 즉시 정지
        if (isNorth && distance < stopDistance)
        {
            targetRb.linearVelocity = Vector2.zero;
            targetRb.angularVelocity = 0f;
            return;
        }

        // 2. [밀기] 레일 이탈 방지: 위쪽(Y축) 힘을 아예 삭제! (중요 ⭐)
        if (!isNorth)
        {
            direction.y = 0; // 위로 뜨는 힘 제거
            direction.Normalize(); // 수평 방향으로만 힘 재조정

        }
        else
        {
            direction = direction.normalized;
        }

        // 3. 힘 계산 (거리 제곱 반비례)
        float rawForce = magneticForce / Mathf.Max(distance * distance, minDistance);
        float finalMagnitude = Mathf.Min(rawForce, maxForceLimit);

        float polarityModifier = isNorth ? -1f : (1f * pushMultiplier);
        Vector2 finalForce = direction * finalMagnitude * polarityModifier * target.magneticSensitivity;

        // 4. 물리 적용
        targetRb.AddForce(finalForce, ForceMode2D.Force);

        // 5. [속도 제한] 너무 빨라서 날아가지 않게 "최고 속도 15"로 꽉 잡음
        float maxSpeed = 15f;
        if (targetRb.linearVelocity.magnitude > maxSpeed)
        {
            targetRb.linearVelocity = targetRb.linearVelocity.normalized * maxSpeed;
        }
    }
}