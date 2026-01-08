using UnityEngine;

public class KeepBGM : MonoBehaviour
{
    public static KeepBGM instance; // 이 게임에 딱 하나만 존재해야 함

    private void Awake()
    {
        // 1. 이미 BGM 플레이어가 있는지 확인
        if (instance == null)
        {
            // 아직 없다면? -> 내가 대표다!
            instance = this;

            // 2. 씬이 넘어가도 나를 파괴하지 마라 (핵심 코드 ⭐)
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 이미 다른 BGM 플레이어가 살아있다면?
            // 나는 필요 없으니 사라진다 (중복 방지)
            Destroy(gameObject);
        }
    }
}