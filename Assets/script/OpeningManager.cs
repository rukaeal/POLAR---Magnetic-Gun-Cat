using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; // 코루틴 사용을 위해 필수

public class OpeningManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public string nextSceneName = "Stage 1";

    [Range(0.01f, 0.2f)]
    public float typingSpeed = 0.05f; // 글자 나오는 속도 (작을수록 빠름)

    [TextArea(3, 5)]
    public string[] sentences;

    private int index = 0;
    private bool isTyping = false; // 현재 타이핑 중인지 확인하는 변수

    void Start()
    {
        if (sentences.Length > 0)
        {
            StartCoroutine(TypeSentence(sentences[0])); // 첫 대사 타이핑 시작
        }
    }

    public void NextSentence()
    {
        // 1. 만약 아직 타이핑 중이라면? -> 즉시 완성시키기 (스킵)
        if (isTyping)
        {
            StopAllCoroutines(); // 타이핑 멈추고
            dialogueText.text = sentences[index]; // 완성된 문장 바로 보여줌
            isTyping = false;
        }
        // 2. 타이핑이 다 끝난 상태라면? -> 다음 대사로 넘어가기
        else
        {
            index++;
            if (index < sentences.Length)
            {
                StartCoroutine(TypeSentence(sentences[index]));
            }
            else
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    // 한 글자씩 출력하는 코루틴 함수
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = ""; // 텍스트 비우고 시작

        // 한 글자씩 더하면서 대기
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed); // typingSpeed 만큼 대기
        }

        isTyping = false; // 타이핑 끝남
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            NextSentence();
        }
    }
}