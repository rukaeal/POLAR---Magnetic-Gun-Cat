using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 어디서든 접근할 수 있게 싱글톤(Singleton)으로 만듭니다.
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartStage();
        }
    }

    public void RestartStage()
    {
        // 현재 씬을 다시 불러옵니다 (가장 빠른 리셋 방식)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CompleteStage()
    {
        Debug.Log("Stage Clear!");
        // 다음 씬이 있다면 로드, 없으면 재시작 (MVP용)
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("All Stages Cleared! Restarting...");
            SceneManager.LoadScene(0);
        }
    }
}