using UnityEngine;

public class ScreenScaler : MonoBehaviour
{
    public GameObject backgroundPanel;
    public GameObject UIPanel;
    public Camera[] scalableCameras = new Camera[0];

    [Space(10f)]
    public float gameWidth = 1600;
    public float gameHeight = 1000;

    [Space(10f)]
    public float cameraWidthMultiplier = 0.5f;
    public float cameraHeightMultiplier = 1f;

    [Space(10f)]
    public float cameraX = 0.25f;
    public float cameraY = 0f;

    private float screenWidth;
    private float screenHeight;
    private float screenAspect;
    private float gameAspect;

    private float scaleWidth;
    private float scaleHeight;

    private float currentCameraPosX;
    private float currentCameraPosY;

    private float currentCameraScaleWidth;
    private float currentCameraScaleHeight;

    private void OnValidate()
    {
#if UNITY_EDITOR
        runInEditMode = true;
#endif
    }

    private void Update()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        screenAspect = (screenWidth / screenHeight);
        gameAspect = (gameWidth / gameHeight);

        if (gameAspect >= screenAspect)
        {
            scaleWidth = 1f;
            scaleHeight = (screenAspect / gameAspect);
        }
        else
        {
            scaleHeight = 1f;
            scaleWidth = (gameAspect / screenAspect);
        }

        if (backgroundPanel != null)
            backgroundPanel.transform.localScale = new Vector3(scaleWidth, scaleHeight, 1f);
        if (UIPanel != null)
            UIPanel.transform.localScale = new Vector3(scaleHeight, scaleHeight, 1f);

        currentCameraPosX = (cameraX * scaleWidth) + ((1f - scaleWidth) * .5f);
        currentCameraPosY = (cameraY * scaleHeight) + ((1f - scaleHeight) * .5f);

        currentCameraScaleWidth = cameraWidthMultiplier * scaleWidth;
        currentCameraScaleHeight = cameraHeightMultiplier * scaleHeight;

        if (scalableCameras != null && scalableCameras.Length > 0)
            foreach (var cam in scalableCameras)
            {
                cam.rect = new Rect(currentCameraPosX, currentCameraPosY, currentCameraScaleWidth, currentCameraScaleHeight);
            }
    }
}
