using UnityEngine;

public class LightningFlow : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float scrollSpeed = -10.0f; // 속도 (음수: 발사 방향)

    // 쉐이더 이름표 두 개를 미리 준비
    private int baseMapID;
    private int mainTexID;

    void Start()
    {
        baseMapID = Shader.PropertyToID("_BaseMap"); // URP Particles용
        mainTexID = Shader.PropertyToID("_MainTex"); // 일반용
    }

    void Update()
    {
        if (lineRenderer != null)
        {
            float offset = Time.time * scrollSpeed;
            Vector2 moveVector = new Vector2(offset, 0);

            // 1. BaseMap 이름표가 있으면 움직여라
            if (lineRenderer.material.HasProperty(baseMapID))
            {
                lineRenderer.material.SetTextureOffset(baseMapID, moveVector);
            }

            // 2. MainTex 이름표가 있으면 움직여라 (보험)
            if (lineRenderer.material.HasProperty(mainTexID))
            {
                lineRenderer.material.SetTextureOffset(mainTexID, moveVector);
            }
        }
    }
}