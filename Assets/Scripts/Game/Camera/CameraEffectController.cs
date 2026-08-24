using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorTools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Main.CameraSystem
{
    //Shaking part credits to https://gist.github.com/ftvs/5822103

    [RequireComponent(typeof(Camera), typeof(Animator), typeof(PostProcessVolume))]
    public class CameraEffectController : MonoBehaviour
    {
        public const string CAMERA_EFFECTS_PATH = "Assets/Resources/GameProfiles/";
        public const string DEFAULT_PROFILE = "Default";

        public static List<CameraEffectController> cameraEffectControllers = new List<CameraEffectController>();

#if UNITY_EDITOR
#pragma warning disable 0414
        [ShowOnly]
        [SerializeField]
        private string cameraEffectsPath = CAMERA_EFFECTS_PATH, defaultProfileName = DEFAULT_PROFILE;
#pragma warning restore 0414
#endif

        protected Animator animator;
        protected PostProcessVolume postProcessVolumeComponent;

        [Space(10f)]
        [SerializeField]
        private List<PostProcessProfile> profiles;

        // Transform of the camera to shake. Grabs the gameObject's transform
        // if null.
        public Transform cameraTransform;

        // How long the object should shake for.
        public float shakeDuration = 0f;

        // Amplitude of the shake. A larger value shakes the camera harder.
        public float shakeAmount = .7f;
        public float decreaseFactor = 1f;

        [SerializeField]
        private Vector3 originalPos;
        [SerializeField]
        private bool cameraIs2D = false;

        private void Awake()
        {
            //profile
            animator = gameObject.GetComponent<Animator>();
            postProcessVolumeComponent = gameObject.GetComponent<PostProcessVolume>();

            if (!cameraEffectControllers.Contains(this))
                cameraEffectControllers.Add(this);

            profiles = Resources.LoadAll<PostProcessProfile>(CAMERA_EFFECTS_PATH).ToList();
            animator.fireEvents = true;

            //shake
            if (cameraTransform == null)
            {
                cameraTransform = GetComponent(typeof(Transform)) as Transform;
            }
        }

        private void Update()
        {
            //profile
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName(DEFAULT_PROFILE) && !animator.IsInTransition(0))
                {
                    animator.SetBool("TurnDefault", true);
                }
                else if (animator.GetCurrentAnimatorStateInfo(0).IsName(DEFAULT_PROFILE) && !animator.IsInTransition(0) && animator.GetBool("TurnDefault"))
                {
                    postProcessVolumeComponent.profile = profiles.Find((profile) => profile.name == DEFAULT_PROFILE);
                }
            }

            //shake
            if (shakeDuration > 0)
            {
                cameraTransform.localPosition = originalPos + ((cameraIs2D ? (Vector3)Random.insideUnitCircle : Random.insideUnitSphere) * shakeAmount);

                shakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shakeDuration = 0f;
                cameraTransform.localPosition = originalPos;
            }
        }

        public void PlayEffect(string animName)
        {
            animator.SetBool("TurnDefault", false);
            postProcessVolumeComponent.profile = Resources.Load<PostProcessProfile>(CAMERA_EFFECTS_PATH + animName + "Profile");
            animator.Play(animName);
        }

        public void Shake(float duration, float shakeAmount = 0.7f, float decreaseFactor = 1f)
        {
            shakeDuration = duration;
            this.shakeAmount = shakeAmount;
        }

        public void SetPosition(Vector3 position)
        {
            originalPos = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        private void OnEnable()
        {
            //profile
            if (!cameraEffectControllers.Contains(this))
                cameraEffectControllers.Add(this);

            //shake
            originalPos = cameraTransform.localPosition;
        }

        private void OnDisable()
        {
            cameraEffectControllers.Remove(this);
        }

        private void OnDestroy()
        {
            cameraEffectControllers.Remove(this);
        }
    }
}
