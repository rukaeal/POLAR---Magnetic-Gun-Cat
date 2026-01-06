using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{
    private Animator anim;

    // 문을 통과할 때 물리적으로 막고 있는 콜라이더 (센서 아님)
    // 만약 문이 열렸을 때 지나가게 하려면 필요합니다.
    private BoxCollider2D obstacleCollider;

    void Start()
    {
        anim = GetComponent<Animator>();

        // 문 오브젝트에 붙은 콜라이더들을 다 가져와서
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();

        // 그 중에서 "Is Trigger"가 체크 안 된 녀석(벽 역할)을 찾음
        foreach (BoxCollider2D col in colliders)
        {
            if (col.isTrigger == false)
            {
                obstacleCollider = col;
                break;
            }
        }
    }

    // 센서(Trigger)에 플레이어가 들어왔을 때
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어인지 확인
        {
            anim.SetBool("isOpen", true); // 문 열어!

            // (선택) 문이 열리면 벽 판정을 끈다
            if (obstacleCollider != null) obstacleCollider.enabled = false;
        }
    }

    // 센서에서 플레이어가 나갔을 때
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("isOpen", false); // 문 닫아!

            // (선택) 문이 닫히면 벽 판정을 다시 켠다
            if (obstacleCollider != null) obstacleCollider.enabled = true;
        }
    }
}