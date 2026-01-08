using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동 필수 기능

public class TitleManager : MonoBehaviour
{
    [Header("이동할 씬 이름")]
    public string nextSceneName = "OpeningScene"; // 오프닝 씬 이름을 적으세요

    // 시작 버튼이 눌렸을 때 실행될 함수
    public void OnClickStart()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    // 종료 버튼이 눌렸을 때 (선택 사항)
    public void OnClickExit()
    {
        Debug.Log("게임 종료!"); // 에디터에서는 로그만 뜸
        Application.Quit(); // 실제 빌드된 게임에서는 꺼짐
    }
}