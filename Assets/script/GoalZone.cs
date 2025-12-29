using UnityEngine;

public class GoalZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 닿으면 게임 클리어 처리
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CompleteStage(); //
        }
    }
}