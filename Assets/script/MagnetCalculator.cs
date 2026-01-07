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

        // 1. [당기기 안전장치] 너무 가까우면(1.0 미만) 아예 멈춤
        if (isNorth && distance < 1.0f)
        {
            targetRb.linearVelocity = Vector2.zero;
            targetRb.angularVelocity = 0f;
            return;
        }

        // 2. [밀기] 땅바닥에서 밀 때 박스가 뜨지 않게 Y축 힘 제거
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

        // ▼▼▼ [핵심 수정: 로켓 방지 절대 코드] ▼▼▼
        // 밟고 있든 아니든, 거리가 4미터보다 가까우면 
        // 강제로 "4미터 떨어져 있는 척" 계산하게 만듭니다.
        // 이러면 가까이서 쏴도 힘이 아주 약하게 들어갑니다.
        if (distance < 4.0f)
        {
            distance = 4.0f; // 이 숫자가 클수록 올라가는 속도가 느려집니다.
        }
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        // 3. 힘 계산
        float rawForce = magneticForce / Mathf.Max(distance * distance, minDistance);
        float finalMagnitude = Mathf.Min(rawForce, maxForceLimit);

        float polarityModifier = isNorth ? -1f : (1f * pushMultiplier);
        Vector2 finalForce = direction * finalMagnitude * polarityModifier * target.magneticSensitivity;

        targetRb.AddForce(finalForce, ForceMode2D.Force);

        // 4. 속도 제한 (그래도 빠를까봐 한 번 더 막음)
        float maxSpeed = 5f; // 원래 15였는데 5로 확 줄였습니다.
        if (targetRb.linearVelocity.magnitude > maxSpeed)
        {
            targetRb.linearVelocity = targetRb.linearVelocity.normalized * maxSpeed;
        }
    }
}