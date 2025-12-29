using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class MagneticRail : MonoBehaviour
{
    [Header("Rail Settings")]
    public List<Transform> nodes = new List<Transform>();

    [Tooltip("레일 위에서의 마찰력 (0.1 ~ 0.5 추천)")]
    [SerializeField] private float railFriction = 0.1f;

    [Tooltip("레일에 붙어있는 접착력 (자력보다 세야 함! 예: 1000)")]
    [SerializeField] private float snapStrength = 1000f; // [변경됨] 기본값을 대폭 올림

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        UpdateRailGeometry();
    }

    private void OnValidate()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        UpdateRailVisuals();
    }

    public void UpdateRailGeometry()
    {
        UpdateRailVisuals();
        if (nodes.Count > 1)
        {
            Vector2[] points = new Vector2[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                points[i] = transform.InverseTransformPoint(nodes[i].position);
            }
            edgeCollider.points = points;
            edgeCollider.isTrigger = true;
        }
    }

    private void UpdateRailVisuals()
    {
        if (nodes.Count > 0)
        {
            lineRenderer.positionCount = nodes.Count;
            for (int i = 0; i < nodes.Count; i++)
            {
                lineRenderer.SetPosition(i, nodes[i].position);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        MagneticObject magObj = other.GetComponent<MagneticObject>();
        if (magObj != null)
        {
            Rigidbody2D rb = magObj.GetRigidbody();
            rb.gravityScale = 0; // 중력 끄기

            // 1. 가장 가까운 레일 지점과 '레일의 방향(Tangent)'을 찾음
            var result = GetClosestPointAndDirection(rb.position);
            Vector2 closestPoint = result.point;
            Vector2 railDirection = result.dir; // 레일이 뻗어있는 방향

            // 2. 위치 보정: 레일 중심선으로 강력하게 당김 (Snap)
            Vector2 directionToRail = closestPoint - rb.position;
            rb.AddForce(directionToRail * snapStrength);

            // 3. 속도 보정 (핵심 수정 사항 ⭐)
            // 물체의 현재 속도를 '레일 방향'으로만 제한합니다. 
            // 즉, 레일을 벗어나려는(수직) 힘을 강제로 삭제합니다.
            float currentSpeed = Vector2.Dot(rb.linearVelocity, railDirection);
            rb.linearVelocity = railDirection * currentSpeed;

            // 4. 마찰력 적용
            if (Mathf.Abs(currentSpeed) > 0.01f)
            {
                rb.linearVelocity *= (1f - railFriction * Time.fixedDeltaTime);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        MagneticObject magObj = other.GetComponent<MagneticObject>();
        if (magObj != null)
        {
            magObj.GetRigidbody().gravityScale = 1; // 중력 복구
        }
    }

    // 수학 계산: 점과 방향(Tangent)을 동시에 반환하는 구조체
    private (Vector2 point, Vector2 dir) GetClosestPointAndDirection(Vector2 position)
    {
        Vector2 bestPoint = position;
        Vector2 bestDir = Vector2.right;
        float minDistanceSqr = float.MaxValue;

        for (int i = 0; i < nodes.Count - 1; i++)
        {
            Vector2 p1 = nodes[i].position;
            Vector2 p2 = nodes[i + 1].position;

            Vector2 segmentDir = (p2 - p1).normalized; // 선분 방향
            Vector2 point = GetClosestPointOnSegment(p1, p2, position);

            float distSqr = (point - position).sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                bestPoint = point;
                bestDir = segmentDir;
            }
        }
        return (bestPoint, bestDir);
    }

    private Vector2 GetClosestPointOnSegment(Vector2 a, Vector2 b, Vector2 p)
    {
        Vector2 ap = p - a;
        Vector2 ab = b - a;
        float t = Vector2.Dot(ap, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        return a + ab * t;
    }
}