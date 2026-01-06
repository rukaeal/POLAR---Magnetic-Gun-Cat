using UnityEngine;

public class BeamFlow : MonoBehaviour
{
    public LineRenderer lineRenderer; // 라인 렌더러를 가져옴
    public float scrollSpeed = -4.0f; // 흐르는 속도 (음수면 앞으로, 양수면 뒤로)

    void Update()
    {
        // 라인 렌더러가 없으면 아무것도 안 함
        if (lineRenderer == null) return;

        // 시간의 흐름에 따라 오프셋(위치) 값을 계속 변경
        // Time.time은 게임이 시작된 후 흐른 시간입니다.
        float offset = Time.time * scrollSpeed;

        // 마테리얼의 텍스처 위치(Offset)를 X축 방향으로 이동시킴
        lineRenderer.material.mainTextureOffset = new Vector2(offset, 0);
    }
}