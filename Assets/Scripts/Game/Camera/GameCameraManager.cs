using UnityEngine;

namespace Main.CameraSystem
{
    public sealed class GameCameraManager : MonoBehaviour
    {
        [Header("Cameras")]
        public Camera UICamera;
        public Camera gameCamera;
        public Camera backgroundCamera;

        [HideInInspector]
        public CameraEffectController UICameraController;
        [HideInInspector]
        public CameraEffectController gameCameraController;
        [HideInInspector]
        public CameraEffectController backgroundCameraController;

        private void OnValidate()
        {
            UICameraController = UICamera.GetComponent<CameraEffectController>();
            gameCameraController = gameCamera.GetComponent<CameraEffectController>();
            backgroundCameraController = backgroundCamera.GetComponent<CameraEffectController>();
        }
    }
}
