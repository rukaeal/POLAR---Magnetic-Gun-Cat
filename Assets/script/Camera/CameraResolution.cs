using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private void Awake()
    {
        // 고정하고 싶은 비율 (16:9)
        float targetAspect = 16.0f / 9.0f;

        // 현재 기기의 화면 비율
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // 비율에 따라 카메라가 보여줄 영역(Viewport) 조절
        float scaleHeight = windowAspect / targetAspect;

        Camera camera = GetComponent<Camera>();

        // 화면이 너무 홀쭉하다면? (위아래 레터박스)
        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            camera.rect = rect;
        }
        // 화면이 너무 납작하다면? (좌우 레터박스)
        else
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}