using UnityEngine;

public class MagnetCalculator : MonoBehaviour
{
    [Header("Physics Settings")]
    [Tooltip("자력의 기본 세기 (높게 설정하세요, 예: 300~500)")]
    [SerializeField] private float magneticForce = 400f; // [변경 권장] 멀리서도 밀리게 수치를 올림

    [Tooltip("힘의 최대 상한선 (가까울 때 폭발 방지)")]
    [SerializeField] private float maxForceLimit = 30f; // [추가됨] 이 이상 힘이 커지지 않음

    [Tooltip("미는 힘 비율 (0.5 = 절반)")]
    [Range(0.1f, 1.0f)]
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
        Rigidbody2D targetRb = target.GetRigidbody();
        if (targetRb == null) return;

        Vector2 direction = (Vector2)target.transform.position - origin;
        float distance = direction.magnitude;

        // 1. 당기기(N) 근접 정지 로직
        if (isNorth && distance < stopDistance && target.weightClass == MagneticObject.WeightClass.Light)
        {
            targetRb.linearVelocity *= 0.90f;
            return;
        }

        Vector2 forceDir = direction.normalized;

        // 2. 힘 계산 (거리 제곱 반비례)
        float rawForce = magneticForce / Mathf.Max(distance * distance, minDistance);

        // [핵심 변경점] 힘이 아무리 세도 maxForceLimit를 넘지 못하게 자름!
        // 이렇게 하면 가까이 있어도 '적당한' 힘으로 밀립니다.
        float finalMagnitude = Mathf.Min(rawForce, maxForceLimit);

        // 3. 극성 보정
        float polarityModifier = isNorth ? -1f : (1f * pushMultiplier);

        Vector2 finalForce = forceDir * finalMagnitude * polarityModifier * target.magneticSensitivity;

        // 4. 물리 적용
        if (target.weightClass == MagneticObject.WeightClass.Light)
        {
            targetRb.AddForce(finalForce, ForceMode2D.Force);
        }
        else if (target.weightClass == MagneticObject.WeightClass.Heavy)
        {
            if (playerRb != null)
            {
                playerController.ApplyExternalForce(-finalForce, true);
            }
        }
    }
}