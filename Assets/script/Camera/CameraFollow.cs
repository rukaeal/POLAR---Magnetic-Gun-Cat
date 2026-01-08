using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;       // 따라갈 캐릭터 (Player)
    public float smoothSpeed = 5f; // 따라가는 속도 (높을수록 빠름)
    public Vector3 offset;         // 캐릭터와의 거리 (주로 Z축 -10)

    [Header("Map Boundaries")]
    // 카메라가 이동할 수 있는 최소/최대 좌표 (직접 숫자를 입력해서 제한)
    public Vector2 minLimit;
    public Vector2 maxLimit;

    private void LateUpdate() // 카메라는 보통 LateUpdate에서 처리합니다.
    {
        if (target == null) return;

        // 1. 목표 위치 계산 (캐릭터 위치 + 오프셋)
        Vector3 desiredPosition = target.position + offset;

        // 2. 부드러운 이동 (Lerp 사용)
        // 현재 위치에서 목표 위치까지 smoothSpeed 속도로 서서히 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 3. 맵 밖으로 못 나가게 좌표 가두기 (Clamp)
        // 계산된 위치(smoothedPosition)가 설정한 최소/최대 값을 넘지 못하게 함
        float clampedX = Mathf.Clamp(smoothedPosition.x, minLimit.x, maxLimit.x);
        float clampedY = Mathf.Clamp(smoothedPosition.y, minLimit.y, maxLimit.y);

        // 4. 최종 위치 적용
        transform.position = new Vector3(clampedX, clampedY, -10f); // Z축은 보통 -10 고정
    }
}