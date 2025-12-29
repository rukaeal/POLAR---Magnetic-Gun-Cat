using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 떨어지면 즉시 리셋
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.RestartStage();
        }
        // 중요한 퍼즐 상자가 떨어져도 리셋 (선택 사항)
        else if (other.GetComponent<MagneticObject>() != null)
        {
            // 상자가 떨어지면 바로 리셋할지, 아니면 상자만 리스폰할지 기획에 따라 다름
            // 여기서는 '즉시 리셋' 철학에 따라 스테이지 재시작으로 구현
            GameManager.Instance.RestartStage(); //
        }
    }
}