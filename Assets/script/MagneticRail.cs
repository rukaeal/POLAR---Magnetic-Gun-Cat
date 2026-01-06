using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))] // 엣지 콜라이더 필수
public class MagneticRail : MonoBehaviour
{
    [Header("Rail Points")]
    public Transform startPoint;
    public Transform endPoint;

    // 움직이는 레일 지원을 위해 매번 계산하도록 변경할 수도 있지만, 
    // 성능을 위해 캐싱하되, 필요하면 Update에서 갱신하세요.
    private Vector2 dir;
    private float length;

    private void Awake()
    {
        RecalculateRail();
    }

    // 에디터에서 포인트를 옮겼을 때나 레일이 변형될 때 호출
    public void RecalculateRail()
    {
        if (startPoint == null || endPoint == null) return;

        // 방향 벡터 (월드 좌표 기준)
        Vector2 startPos = startPoint.position;
        Vector2 endPos = endPoint.position;

        dir = (endPos - startPos).normalized;
        length = Vector2.Distance(startPos, endPos);
    }

    // 움직이는 레일이라면 FixedUpdate마다 재계산 필요 (성능 고려 선택)
    // private void FixedUpdate() { RecalculateRail(); }

    public Vector2 ClampToRail(Vector2 pos)
    {
        // 안전장치: 점이 없으면 현재 위치 반환
        if (startPoint == null || endPoint == null) return pos;

        // 움직이는 레일 대응: 매번 방향을 다시 계산 (조금 비싸지만 정확함)
        // 정적인 레일이라면 이 두 줄을 지우고 Awake의 값을 쓰세요.
        Vector2 currentDir = (endPoint.position - startPoint.position).normalized;
        float currentLen = Vector2.Distance(startPoint.position, endPoint.position);

        Vector2 fromStart = pos - (Vector2)startPoint.position;
        float t = Vector2.Dot(fromStart, currentDir);

        // 0 ~ 길이 사이로 위치 강제 고정
        t = Mathf.Clamp(t, 0f, currentLen);

        return (Vector2)startPoint.position + currentDir * t;
    }

    // 자동으로 자석 박스를 감지해서 붙여주는 로직 추가
    private void OnTriggerEnter2D(Collider2D other)
    {
        // MagneticObject가 들어오면 레일에 태움
        MagneticObject magObj = other.GetComponent<MagneticObject>();
        if (magObj != null)
        {
            magObj.AttachToRail(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        MagneticObject magObj = other.GetComponent<MagneticObject>();

        // 현재 이 레일에 붙어있는 박스라면
        if (magObj != null && magObj.currentRail == this)
        {
            // [안전장치]
            // 박스의 현재 위치와 레일(선분) 사이의 거리를 잰다.
            Vector2 boxPos = magObj.transform.position;
            Vector2 closestPoint = ClampToRail(boxPos);
            float distance = Vector2.Distance(boxPos, closestPoint);

            // 거리가 1.0f 미만이라면, "박스가 레일 끝에 도달해서 살짝 삐져나온 것"으로 간주한다.
            // 이때는 Detach(하차)를 하지 않고 무시한다.
            // (플레이어가 억지로 잡아당겨서 거리가 멀어졌을 때만 하차시킴)
            if (distance < 1.0f)
            {
                return;
            }

            magObj.DetachFromRail();
        }
    }

    // 에디터에서 레일 경로를 보기 위한 기즈모
    private void OnDrawGizmos()
    {
        if (startPoint != null && endPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(startPoint.position, endPoint.position);
            Gizmos.DrawWireSphere(startPoint.position, 0.2f);
            Gizmos.DrawWireSphere(endPoint.position, 0.2f);
        }
    }
}
