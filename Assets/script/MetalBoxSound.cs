using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody2D))]
public class MetalBoxSound : MonoBehaviour
{
    [Header("Sound Clips")]
    public AudioClip impactClip; // 쿵! (충돌)
    public AudioClip dragClip;   // 끼이익 (끌기)

    [Header("Settings")]
    public float impactThreshold = 2.0f; // 이 속도 이상일 때만 '쿵' 소리 남
    public float dragSpeedThreshold = 0.5f; // 이 속도 이상일 때만 '끼익' 소리 남

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private bool isDragging = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        // 오디오 소스 기본 설정
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // 3D 사운드 (멀어지면 작게)
    }

    private void Update()
    {
        HandleDragSound();
    }

    // 1. 충돌음 (쿵!)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 부딪힌 속도가 일정 이상일 때만 소리 재생 (살짝 닿는 건 무시)
        if (collision.relativeVelocity.magnitude > impactThreshold)
        {
            // 소리 크기를 충돌 속도에 비례해서 조절 (세게 부딪히면 크게)
            float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 10f);
            audioSource.PlayOneShot(impactClip, volume);
        }
    }

    // 2. 드래그음 (끼이익~)
    private void HandleDragSound()
    {
        // 박스가 움직이고 있는가? (속도가 있고 + 땅에 닿아있음)
        // (IsTouchingLayers는 Collider가 Ground 레이어와 닿아있는지 확인합니다)
        bool isMoving = rb.linearVelocity.magnitude > dragSpeedThreshold;
        bool isGrounded = IsGrounded();

        if (isMoving && isGrounded)
        {
            // 움직이는데 소리가 안 나고 있다면 -> 재생 시작
            if (!isDragging)
            {
                audioSource.clip = dragClip;
                audioSource.loop = true; // 반복 재생
                audioSource.volume = 0.5f; // 드래그 소리는 너무 크면 시끄러우니 줄임
                audioSource.Play();
                isDragging = true;
            }
        }
        else
        {
            // 멈췄거나 공중에 떴는데 소리가 나고 있다면 -> 정지
            if (isDragging)
            {
                audioSource.Stop();
                audioSource.loop = false;
                isDragging = false;
            }
        }
    }

    // 바닥에 닿아있는지 확인하는 간단한 레이캐스트
    private bool IsGrounded()
    {
        // 박스 중심에서 아래로 살짝 레이저를 쏴서 확인
        float extraHeight = 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, GetComponent<Collider2D>().bounds.extents.y + extraHeight);

        // 무언가(플레이어 제외)에 닿았다면 땅에 있는 것으로 간주
        return hit.collider != null && hit.collider.gameObject != gameObject;
    }
}