using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필수!

public class OpeningCutsceneManager : MonoBehaviour
{
    // 1. 컷신 데이터 (이미지 + 대사 묶음)
    [System.Serializable]
    public struct CutsceneStep
    {
        [TextArea(3, 5)] public string dialogue; // 대사
        public Sprite cutsceneImage; // 보여줄 이미지 (비워두면 앞 이미지 유지)
    }

    [Header("설정 (Settings)")]
    public string nextSceneName = "Stage1"; // 오프닝 끝나고 넘어갈 씬 이름 (직접 입력 가능)
    public float typingSpeed = 0.05f; // 글자 나오는 속도

    [Header("UI 연결 (Connections)")]
    public Image displayImage;       // 화면 중앙 이미지
    public TextMeshProUGUI subTitleText; // 자막 텍스트

    [Header("컷신 내용 (Steps)")]
    public List<CutsceneStep> steps; // 여기에 + 버튼 눌러서 내용 추가

    private int currentIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        // 시작하자마자 첫 장면 재생
        if (steps.Count > 0)
        {
            StartCoroutine(PlaySequence());
        }
    }

    void Update()
    {
        // 마우스 클릭 시 다음으로 진행
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // 1. 타이핑 중이면 -> 즉시 완성 (스킵)
                StopAllCoroutines();
                subTitleText.text = steps[currentIndex].dialogue;
                isTyping = false;
            }
            else
            {
                // 2. 타이핑 끝났으면 -> 다음 장면으로
                NextStep();
            }
        }
    }

    void NextStep()
    {
        currentIndex++;

        if (currentIndex < steps.Count)
        {
            StartCoroutine(PlaySequence());
        }
        else
        {
            // 더 이상 보여줄 게 없으면 게임 시작!
            Debug.Log("오프닝 종료! 게임 시작!");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    IEnumerator PlaySequence()
    {
        isTyping = true;
        CutsceneStep currentStep = steps[currentIndex];

        // 이미지 교체 (이미지가 비어있지 않을 때만)
        if (currentStep.cutsceneImage != null)
        {
            displayImage.sprite = currentStep.cutsceneImage;
        }

        // 텍스트 타이핑 효과
        subTitleText.text = ""; // 초기화
        foreach (char letter in currentStep.dialogue.ToCharArray())
        {
            subTitleText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}