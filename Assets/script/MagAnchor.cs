using UnityEngine;

public class MagAnchor : MonoBehaviour
{
    // 앵커가 박힐 때 파티클이나 소리를 재생할 수 있습니다.
    private void Start()
    {
        // 박히는 효과음이나 파티클 재생 로직이 들어갈 곳
        Debug.Log("Anchor Created!");
    }

    // 외부에서 앵커를 제거할 때 호출
    public void DestroyAnchor()
    {
        // 사라지는 효과(애니메이션 등) 처리 후 삭제
        Destroy(gameObject);
    }
}