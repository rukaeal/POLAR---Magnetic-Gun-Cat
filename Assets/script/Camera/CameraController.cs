using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("카메라가 따라다닐 대상 (플레이어)")]
    public Transform target;

    [Header("Settings")]
    [Tooltip("카메라가 따라가는 속도 (0.1 = 느림, 1 = 즉시)")]
    [SerializeField] private float smoothSpeed = 0.125f;

    [Tooltip("플레이어로부터의 거리 보정")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);

    [Header("Look Ahead (시선 유도)")]
    [Tooltip("플레이어가 보는 방향으로 미리 카메라를 옮겨줄지 여부")]
    [SerializeField] private bool enableLookAhead = true;
    [SerializeField] private float lookAheadAmount = 2f;
    [SerializeField] private float lookAheadSpeed = 2f;

    private Vector3 currentVelocity;
    private float lookOffset;

    private void FixedUpdate() // 물리가 섞인 게임은 FixedUpdate 권장
    {
        if (target == null) return;

        // 1. 기본 목표 위치 계산
        Vector3 finalPos = target.position + offset;

        // 2. 시선 유도 (플레이어가 오른쪽을 보면 카메라도 오른쪽을 더 비춤)
        if (enableLookAhead)
        {
            // 플레이어의 입력이나 속도에 따라 방향 결정 (여기선 속도 기준)
            float moveDir = target.GetComponent<Rigidbody2D>().linearVelocity.x;

            if (Mathf.Abs(moveDir) > 0.1f)
            {
                // 움직이는 방향으로 목표 오프셋 설정
                float targetLookOffset = Mathf.Sign(moveDir) * lookAheadAmount;
                // 부드럽게 변경
                lookOffset = Mathf.Lerp(lookOffset, targetLookOffset, lookAheadSpeed * Time.fixedDeltaTime);
            }

            finalPos.x += lookOffset;
        }

        // 3. 부드러운 이동 (Smooth Damp)
        // Lerp보다 더 자연스러운 물리적 감속 이동을 제공합니다.
        transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref currentVelocity, smoothSpeed);
    }

    // 외부(GameManager 등)에서 카메라를 강제로 특정 지점으로 옮길 때 사용
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}