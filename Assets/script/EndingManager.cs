using UnityEngine;
using TMPro;
using System.Collections; // 코루틴 사용

public class EndingManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI dialogueText; // 상황 설명할 작은 텍스트
    public GameObject gameClearUI;       // 마지막에 뜰 "GAME CLEAR" 글자 덩어리

    [Header("설정")]
    [Range(0.01f, 0.2f)]
    public float typingSpeed = 0.05f;

    [TextArea(3, 5)]
    public string[] sentences; // 엔딩 스토리 대사

    private int index = 0;
    private bool isTyping = false;

    void Start()
    {
        // 시작할 때 "GAME CLEAR" 화면은 꺼두고 시작
        gameClearUI.SetActive(false);

        if (sentences.Length > 0)
        {
            StartCoroutine(TypeSentence(sentences[0]));
        }
    }

    public void NextSentence()
    {
        // 1. 타이핑 중이면 -> 즉시 완성 (스킵)
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = sentences[index];
            isTyping = false;
        }
        // 2. 타이핑 끝났으면 -> 다음 대사 or 결과 화면
        else
        {
            index++;
            if (index < sentences.Length)
            {
                StartCoroutine(TypeSentence(sentences[index]));
            }
            else
            {
                // ★ 대사가 다 끝나면 여기가 실행됩니다 ★
                ShowGameClear();
            }
        }
    }

    void ShowGameClear()
    {
        dialogueText.text = ""; // 설명 텍스트는 지우고
        gameClearUI.SetActive(true); // "GAME CLEAR" 화면을 켠다!
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            // 게임 클리어 화면이 안 떴을 때만 넘기기 가능
            if (gameClearUI.activeSelf == false)
            {
                NextSentence();
            }
        }
    }
}