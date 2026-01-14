using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("기본 이동 속도")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("점프 힘 (4.5 ~ 6 정도 추천)")]
    [SerializeField] private float jumpForce = 5f;
    [Tooltip("최대 낙하/점프 속도 제한 (뚫림 방지용)")]
    [SerializeField] private float maxVerticalSpeed = 10f; // [추가] 속도 제한

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.4f; // [수정] 감지 범위 살짝 넓힘
    [SerializeField] private LayerMask groundLayer;

    public GameObject currentGroundObject;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isGrounded;
    private bool isFacingRight = true;
    private float horizontalInput;
    private bool isMagnetized = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput != 0) anim.SetBool("isRun", true);
        else anim.SetBool("isRun", false);

        // 점프 시도
        if (Input.GetButtonDown("Jump") && isGrounded && !isMagnetized)
        {
            PerformJump();
        }

        if (horizontalInput > 0 && !isFacingRight) Flip();
        else if (horizontalInput < 0 && isFacingRight) Flip();
    }

    private void FixedUpdate()
    {
        CheckGround();
        Move();
        ClampVelocity(); // [추가] 매 프레임 속도 단속
    }

    private void Move()
    {
        // Y축 속도는 건드리지 않고 X축만 변경
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    // ▼▼▼ [추가된 핵심 기능] 속도 제한 ▼▼▼
    private void ClampVelocity()
    {
        // 현재 Y축 속도가 제한 속도보다 빠르면 강제로 깎아버림
        if (rb.linearVelocity.y > maxVerticalSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxVerticalSpeed);
        }
        // 떨어지는 속도도 너무 빠르면 제한 (선택 사항)
        if (rb.linearVelocity.y < -maxVerticalSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxVerticalSpeed);
        }
    }
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    private void PerformJump()
    {
        // 점프 직전 속도 초기화 (더블 점프나 튕김 방지)
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

        // 힘을 가함 (Impulse는 순간적인 힘)
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // 점프한 순간에는 땅에서 떨어졌다고 처리
        isGrounded = false;
    }

    private void CheckGround()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = groundCollider != null;

        if (isGrounded)
        {
            currentGroundObject = groundCollider.gameObject;
        }
        else
        {
            currentGroundObject = null;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void ApplyExternalForce(Vector2 force, bool disableControl)
    {
        isMagnetized = disableControl;
        rb.AddForce(force);
    }

    public void ReleaseMagnetControl()
    {
        isMagnetized = false;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}