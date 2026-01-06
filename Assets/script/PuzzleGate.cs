using UnityEngine;

public class PuzzleGate : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D obstacleCollider;

    private void Start()
    {
        anim = GetComponent<Animator>();

        // 문을 막고 있는 벽(Collider) 찾기 (Is Trigger가 아닌 것)
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D col in colliders)
        {
            if (!col.isTrigger)
            {
                obstacleCollider = col;
                break;
            }
        }
    }

    // 발판이 밟혔을 때 (Pressure Plate에서 호출)
    public void OpenGate()
    {
        if (anim != null)
        {
            anim.SetBool("isOpen", true); // 애니메이션 재생!

            // (선택) 문이 열리면 벽 판정을 꺼서 지나갈 수 있게 함
            if (obstacleCollider != null) obstacleCollider.enabled = false;
        }
    }

    // 발판에서 발을 뗐을 때
    public void CloseGate()
    {
        if (anim != null)
        {
            anim.SetBool("isOpen", false); // 닫는 애니메이션!

            // 문이 닫히면 다시 벽을 켜서 못 지나가게 함
            if (obstacleCollider != null) obstacleCollider.enabled = true;
        }
    }
}