using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필요

public class EndingManager : MonoBehaviour
{
    // 1. 데이터 구조 (글자 + 이미지)
    [System.Serializable]
    public struct EndingStep
    {
        [TextArea(3, 5)] public string dialogue; // 엔딩 대사
        public Sprite cutsceneImage; // 엔딩 컷신 이미지
    }

    [Header("UI 연결")]
    public Image displayImage;       // 중앙 이미지 UI
    public TextMeshProUGUI subTitleText; // 자막 텍스트 UI

    [Header("이동할 씬 이름")]
    public string titleSceneName = "TitleScene"; // 끝나면 돌아갈 타이틀 화면 이름

    [Header("엔딩 데이터")]
    public List<EndingStep> endingSteps; // 인스펙터에서 채우세요

    private int currentIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        // 시작하면 엔딩 시퀀스 재생
        if (endingSteps.Count > 0)
        {
            StartCoroutine(PlaySequence());
        }
    }

    void Update()
    {
        // 클릭하면 다음 장면
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // 타이핑 중이면 즉시 완성
                StopAllCoroutines();
                subTitleText.text = endingSteps[currentIndex].dialogue;
                isTyping = false;
            }
            else
            {
                // 다 나왔으면 다음 장으로
                NextStep();
            }
        }
    }

    void NextStep()
    {
        currentIndex++;

        if (currentIndex < endingSteps.Count)
        {
            StartCoroutine(PlaySequence());
        }
        else
        {
            // 엔딩이 모두 끝났을 때!
            FinishEnding();
        }
    }

    IEnumerator PlaySequence()
    {
        isTyping = true;
        EndingStep currentStep = endingSteps[currentIndex];

        // 이미지 교체 (비어있으면 이전 그림 유지)
        if (currentStep.cutsceneImage != null)
        {
            displayImage.sprite = currentStep.cutsceneImage;
        }

        // 텍스트 타이핑 효과
        subTitleText.text = "";
        foreach (char letter in currentStep.dialogue.ToCharArray())
        {
            subTitleText.text += letter;
            yield return new WaitForSeconds(0.05f); // 글자 속도
        }

        isTyping = false;
    }

    // ⭐ 엔딩 종료 처리
    void FinishEnding()
    {
        Debug.Log("엔딩 종료! 타이틀로 이동합니다.");

        // 1. 타이틀 화면으로 가거나
        SceneManager.LoadScene(titleSceneName);

        // 2. 아예 게임을 끄고 싶다면 아래 주석을 푸세요
        // Application.Quit();
    }
}