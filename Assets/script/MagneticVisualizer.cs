using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MagneticVisualizer : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("곡선의 부드러움 정도 (점의 개수)")]
    [SerializeField] private int resolution = 20;

    [Tooltip("자력선이 휘어지는 정도")]
    [SerializeField] private float curveHeight = 0.5f;

    [Tooltip("전류가 흐르는 속도 (텍스처 스크롤)")]
    [SerializeField] private float flowSpeed = 5.0f;

    [Header("Colors")]
    [SerializeField] private Color colorNorth = Color.blue;
    [SerializeField] private Color colorSouth = Color.red;
    [SerializeField] private Color colorAnchor = Color.yellow;

    private LineRenderer lineRenderer;
    private float textureOffset = 0f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
        lineRenderer.useWorldSpace = true; // 월드 좌표 사용 필수
    }

    // MagnetGun에서 매 프레임 이 함수를 호출할 겁니다.
    public void DrawMagneticLine(Vector3 startPos, Vector3 endPos, bool isNorth, bool isAnchorMode)
    {
        // 1. 활성화 확인
        if (!lineRenderer.enabled) lineRenderer.enabled = true;

        // 2. 색상 설정
        Color targetColor = isAnchorMode ? colorAnchor : (isNorth ? colorNorth : colorSouth);
        lineRenderer.startColor = targetColor;
        lineRenderer.endColor = targetColor;

        // 3. 베지어 곡선 계산 (휘어짐 효과)
        // 직선의 중간 지점을 찾고, 위로 살짝 들어올린 제어점(Control Point)을 만듭니다.
        Vector3 midPoint = (startPos + endPos) / 2f;

        // 총의 각도에 따라 휘어지는 방향을 다르게 하면 더 자연스럽습니다 (여기선 단순화하여 위쪽으로 휘게 함)
        // 약간의 랜덤성을 주면 '지지직'거리는 전기 느낌이 납니다.
        float noise = Mathf.Sin(Time.time * 10f) * 0.2f;
        Vector3 controlPoint = midPoint + (Vector3.up * (curveHeight + noise));

        // 곡선 점들 배치
        for (int i = 0; i < resolution; i++)
        {
            float t = (float)i / (resolution - 1);
            Vector3 p = CalculateBezierPoint(t, startPos, controlPoint, endPos);
            lineRenderer.SetPosition(i, p);
        }

        // 4. 텍스처 스크롤 (흐르는 효과)
        // LineRenderer의 Material에 텍스처가 있다면 이동시킵니다.
        textureOffset -= Time.deltaTime * flowSpeed;
        if (lineRenderer.material != null)
        {
            lineRenderer.material.mainTextureOffset = new Vector2(textureOffset, 0);
        }
    }

    public void HideLine()
    {
        if (lineRenderer.enabled) lineRenderer.enabled = false;
    }

    // 2차 베지어 곡선 공식
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // B(t) = (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return (uu * p0) + (2 * u * t * p1) + (tt * p2);
    }
}