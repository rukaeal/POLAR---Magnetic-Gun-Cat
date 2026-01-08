using UnityEngine;

public class MagnetGun : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private LayerMask metalLayer;
    [SerializeField] private MagnetCalculator calculator;

    [Header("Visuals")]
    [Tooltip("새로 만든 비주얼라이저 연결")]
    [SerializeField] private MagneticVisualizer visualizer; // [변경됨]

    [Header("Anchor Settings")]
    [SerializeField] private GameObject anchorPrefab;
    [SerializeField] private float climbSpeed = 5f;

    [Header("Audio")]
    public AudioSource gunAudio; // 오디오 소스를 담을 빈칸

    // 내부 상태 변수
    private bool isNorth = true;
    private MagAnchor currentAnchor;
    private DistanceJoint2D playerJoint;
    private Rigidbody2D playerRb;

    private void Start()
    {
        playerRb = GetComponentInParent<Rigidbody2D>();

        // [중요] 비주얼라이저가 연결 안 되어 있으면 자동으로 찾음
        if (visualizer == null) visualizer = GetComponent<MagneticVisualizer>();
    }

    private void Update()
    {
        RotateGunTowardsMouse();
        HandleInput();
        HandleVisuals(); // 모든 그리기 처리는 여기서

        // ▼▼▼ [소리 추가 부분] ▼▼▼

        // 1. 왼쪽(0) 또는 오른쪽(1) 버튼을 '누르는 순간' 재생
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            gunAudio.Play();
        }

        // 2. 버튼에서 손을 '떼는 순간' 정지
        // (주의: 두 버튼 중 하나라도 떼면 꺼지는 게 아니라, 둘 다 안 누를 때 꺼져야 함)
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            // 아직 다른 버튼을 누르고 있다면 끄지 마라 (예: 왼쪽 떼고 오른쪽 누름)
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                gunAudio.Stop();
            }
        }

        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
    }

    private void HandleInput()
    {
        // 우클릭: 극성 전환
        if (Input.GetMouseButtonDown(1)) isNorth = !isNorth;

        // 좌클릭: 자력 발사 (앵커 없을 때)
        if (Input.GetMouseButton(0) && currentAnchor == null)
        {
            FireMagneticPulse();
        }

        // 휠 클릭: 앵커 발사/해제
        if (Input.GetMouseButtonDown(2))
        {
            if (currentAnchor != null) DetachAnchor();
            else FireAnchor();
        }

        // 앵커 줄타기
        if (currentAnchor != null) HandleClimbing();
    }

    // [핵심 변경] 모든 시각 효과를 한 곳에서 처리
    private void HandleVisuals()
    {
        if (visualizer == null) return;

        Vector3 startPos = transform.position;
        Vector3 endPos;
        bool isAnchorMode = (currentAnchor != null);

        if (isAnchorMode)
        {
            // 앵커 모드: 총구 ~ 앵커 위치
            endPos = currentAnchor.transform.position;
        }
        else
        {
            // 조준 모드: 레이캐스트로 닿는 곳까지
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, maxDistance, metalLayer);
            if (hit.collider != null) endPos = hit.point;
            else endPos = transform.position + transform.right * maxDistance;
        }

        // 비주얼라이저에게 그리라고 명령
        visualizer.DrawMagneticLine(startPos, endPos, isNorth, isAnchorMode);
    }

    private void FireMagneticPulse()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, maxDistance, metalLayer);
        if (hit.collider != null)
        {
            MagneticObject target = hit.collider.GetComponent<MagneticObject>();
            if (target != null && calculator != null)
            {
                calculator.ApplyMagnetForce(target, transform.position, isNorth);
            }
        }
    }

    private void FireAnchor()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, maxDistance, metalLayer);

        if (hit.collider != null)
        {
            // 레일에는 앵커 박기 불가
            if (hit.collider.GetComponent<MagneticRail>() != null) return;

            GameObject newAnchorObj = Instantiate(anchorPrefab, hit.point, Quaternion.identity);
            currentAnchor = newAnchorObj.GetComponent<MagAnchor>();

            if (playerJoint == null) playerJoint = playerRb.gameObject.AddComponent<DistanceJoint2D>();

            playerJoint.enabled = true;
            playerJoint.connectedAnchor = hit.point;
            playerJoint.distance = Vector2.Distance(transform.position, hit.point);
            playerJoint.autoConfigureDistance = false;
            playerJoint.maxDistanceOnly = true;
            playerJoint.enableCollision = true;
        }
    }

    private void DetachAnchor()
    {
        if (currentAnchor != null)
        {
            currentAnchor.DestroyAnchor();
            currentAnchor = null;
        }
        if (playerJoint != null) playerJoint.enabled = false;
    }

    private void HandleClimbing()
    {
        if (playerJoint == null || !playerJoint.enabled) return;
        float vertical = Input.GetAxis("Vertical");
        if (vertical > 0) playerJoint.distance -= climbSpeed * Time.deltaTime;
        else if (vertical < 0) playerJoint.distance += climbSpeed * Time.deltaTime;
    }

    private void RotateGunTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}