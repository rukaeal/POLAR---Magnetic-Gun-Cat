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

    // 버튼과 연결할 함수 (public이어야 버튼에서 보입니다)
    public void QuitGame()
    {
        // 1. 에디터에서 플레이 중일 때 (테스트용)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        // 2. 실제 빌드된 게임일 때 (배포용)
#else
            Application.Quit();
#endif

        Debug.Log("게임 종료 버튼이 눌렸습니다."); // 확인용 로그
    }
}