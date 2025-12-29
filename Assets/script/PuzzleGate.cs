using UnityEngine;

public class PuzzleGate : MonoBehaviour
{
    [Header("Gate Settings")]
    [Tooltip("문이 열리는 이동 목표 위치 (상대 좌표)")]
    [SerializeField] private Vector3 openOffset = new Vector3(0, 3, 0);
    [SerializeField] private float moveSpeed = 2f;

    private Vector3 closedPos;
    private Vector3 targetPos;
    private bool isOpen = false;

    private void Start()
    {
        closedPos = transform.position;
        targetPos = closedPos; // 처음엔 닫힌 상태
    }

    private void Update()
    {
        // 부드럽게 문 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    // PressurePlate에서 호출할 함수
    public void OpenGate()
    {
        isOpen = true;
        targetPos = closedPos + openOffset;
    }

    // PressurePlate에서 호출할 함수
    public void CloseGate()
    {
        isOpen = false;
        targetPos = closedPos;
    }
}