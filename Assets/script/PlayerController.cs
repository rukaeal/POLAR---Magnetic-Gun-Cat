using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("기본 이동 속도")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("보정용 약한 점프 힘")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // ▼▼▼ [핵심 추가] 현재 밟고 있는 물체를 저장하는 변수 ▼▼▼
    public GameObject currentGroundObject;
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isFacingRight = true;
    private float horizontalInput;
    private bool isMagnetized = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

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
    }

    private void Move()
    {
        if (isMagnetized) return;
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // ▼▼▼ [수정됨] 밟은 물체를 식별하는 로직 ▼▼▼
    private void CheckGround()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        isGrounded = groundCollider != null;

        if (isGrounded)
        {
            currentGroundObject = groundCollider.gameObject; // 밟은 물체 저장
        }
        else
        {
            currentGroundObject = null;
        }
    }
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

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