using UnityEngine;
using TMPro;
using System.Collections;

public class EndingManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI dialogueText; // 텍스트
    public GameObject gameClearUI;       // 결과 화면 그룹
    public GameObject dialoguePanel;     // ★ 추가: 회색 대화창 판넬 (이걸 끌 거야!)

    [Header("설정")]
    [Range(0.01f, 0.2f)]
    public float typingSpeed = 0.05f;

    [TextArea(3, 5)]
    public string[] sentences;

    private int index = 0;
    private bool isTyping = false;

    void Start()
    {
        gameClearUI.SetActive(false);
        dialoguePanel.SetActive(true); // 시작할 땐 대화창 켜기

        if (sentences.Length > 0)
        {
            StartCoroutine(TypeSentence(sentences[0]));
        }
    }

    public void NextSentence()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = sentences[index];
            isTyping = false;
        }
        else
        {
            index++;
            if (index < sentences.Length)
            {
                StartCoroutine(TypeSentence(sentences[index]));
            }
            else
            {
                ShowGameClear();
            }
        }
    }

    void ShowGameClear()
    {
        // ★ 여기가 핵심 변경점 ★
        dialoguePanel.SetActive(false); // 회색 판넬을 꺼버림!
        gameClearUI.SetActive(true);    // 클리어 화면을 킴!
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
            if (gameClearUI.activeSelf == false)
            {
                NextSentence();
            }
        }
    }
}