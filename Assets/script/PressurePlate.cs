using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("눌렸을 때 문이 열리는 등의 이벤트 연결")]
    public UnityEvent onPressed;
    [Tooltip("떼졌을 때 문이 닫히는 등의 이벤트 연결")]
    public UnityEvent onReleased;

    [Tooltip("버튼이 눌리는 시각적 깊이")]
    [SerializeField] private float pressDepth = 0.1f;
    [SerializeField] private Transform buttonVisual; // 밟히는 부분 스프라이트

    private int objectsOnPlate = 0; // 현재 위에 있는 물체 개수
    private Vector3 initialPos;

    private void Start()
    {
        if (buttonVisual == null) buttonVisual = transform;
        initialPos = buttonVisual.localPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어거나, 자력 물체(MagneticObject)일 때만 반응
        if (other.CompareTag("Player") || other.GetComponent<MagneticObject>() != null)
        {
            objectsOnPlate++;
            if (objectsOnPlate == 1) // 첫 번째 물체가 올라왔을 때
            {
                onPressed?.Invoke(); // 연결된 이벤트 실행 (문 열기)
                UpdateVisual(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponent<MagneticObject>() != null)
        {
            objectsOnPlate--;
            if (objectsOnPlate <= 0) // 모든 물체가 나갔을 때
            {
                objectsOnPlate = 0; // 안전장치
                onReleased?.Invoke(); // 연결된 이벤트 실행 (문 닫기)
                UpdateVisual(false);
            }
        }
    }

    private void UpdateVisual(bool isPressed)
    {
        // 버튼이 쑥 들어가는 연출
        Vector3 targetPos = initialPos + (isPressed ? Vector3.down * pressDepth : Vector3.zero);
        buttonVisual.localPosition = targetPos;
    }
}