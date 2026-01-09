using UnityEngine;
using UnityEngine.SceneManagement; // 씬(맵) 재시작을 위해 필요

public class DeathWallController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2.0f; // 벽이 다가오는 속도

    void Update()
    {
        // 1. 오른쪽으로 계속 이동
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }

    // 2. 무언가 닿았을 때 실행되는 함수 (Is Trigger가 켜져 있어야 함)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 닿은 물체가 "Player" 태그를 달고 있다면
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 사망!");

            // 현재 스테이지 다시 로드 (죽는 처리)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}