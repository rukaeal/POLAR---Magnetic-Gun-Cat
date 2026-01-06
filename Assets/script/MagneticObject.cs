using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MagneticObject : MonoBehaviour
{
    [HideInInspector] public MagneticRail currentRail;

    private Rigidbody2D rb;

    // ▼▼▼ [중요] 이 부분이 빠져 있어서 에러가 났던 겁니다! ▼▼▼
    [Header("Magnetic Properties")]
    public float magneticSensitivity = 50f;
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    [Header("Magnet Move")]
    public float railMoveSpeed = 20f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (currentRail != null)
        {
            // 1. [위치 고정] 박스가 레일 선을 벗어나지 않게 딱 붙임
            rb.position = currentRail.ClampToRail(rb.position);

            // 2. 레일 정보 가져오기 (방향, 길이)
            Vector2 p1 = currentRail.startPoint.position;
            Vector2 p2 = currentRail.endPoint.position;
            Vector2 railVec = p2 - p1;
            float length = railVec.magnitude;      // 레일 전체 길이
            Vector2 railDir = railVec.normalized;  // 레일 방향

            // 3. [속도 투영] 현재 속도를 레일 방향으로만 정렬 (위아래 흔들림 제거)
            float currentSpeed = Vector2.Dot(rb.linearVelocity, railDir);

            // 4. [떨림 방지 핵심 ] 현재 내 위치가 레일의 어디쯤인지 계산 (0 ~ length)
            float myProgress = Vector2.Dot(rb.position - p1, railDir);

            // A. 시작점(0)에 왔는데 + 뒤로 가려고 하면(< 0) -> 속도 차단!
            if (myProgress <= 0.05f && currentSpeed < 0)
            {
                currentSpeed = 0f;
            }
            // B. 끝점(length)에 왔는데 + 앞으로 가려고 하면(> 0) -> 속도 차단!
            else if (myProgress >= length - 0.05f && currentSpeed > 0)
            {
                currentSpeed = 0f;
            }

            // 5. 계산된 깔끔한 속도 적용
            rb.linearVelocity = railDir * currentSpeed;
            rb.angularVelocity = 0f; // 회전 금지
        }
    }

    /// <summary>
    /// 자력으로 레일 이동
    /// </summary>
    public void MoveOnRail(float input)
    {
        if (currentRail == null) return;

        Vector2 pos = rb.position;
        Vector2 dir = ((Vector2)currentRail.endPoint.position -
                       (Vector2)currentRail.startPoint.position).normalized;

        Vector2 nextPos = pos + dir * input * railMoveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(currentRail.ClampToRail(nextPos));
    }

    /// <summary>
    /// 레일에 붙기
    /// </summary>
    public void AttachToRail(MagneticRail rail)
    {
        currentRail = rail;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;
    }

    /// <summary>
    /// 레일에서 떨어지기
    /// </summary>
    public void DetachFromRail()
    {
        currentRail = null;
        rb.gravityScale = 1f;
    }

    // ▼▼▼ [추가] 아까 GetRigidbody 오류도 이걸 넣으면 해결됩니다 ▼▼▼
    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }
}