using UnityEngine;

public class MagneticObject : MonoBehaviour
{
    public enum WeightClass { Light, Heavy }

    [Header("Magnetic Properties")]
    [Tooltip("Light: 물체가 움직임 / Heavy: 플레이어가 끌려감")]
    public WeightClass weightClass = WeightClass.Light;

    [Tooltip("자력에 반응하는 감도 (1.0 = 표준)")]
    public float magneticSensitivity = 1.0f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }
}