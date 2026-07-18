using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorTools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(Camera), typeof(Animator), typeof(PostProcessVolume))]
public class CameraEffectController : MonoBehaviour
{
    public static List<CameraEffectController> cameraEffectControllers = new List<CameraEffectController>();

#if UNITY_EDITOR
    [Space(10f)]
    [ShowOnly]
    [SerializeField]
    private string cameraEffectsPath = CAMERA_EFFECTS_PATH;
    [ShowOnly]
    [SerializeField]
    private string defaultProfileName = DEFAULT_PROFILE;
#endif

    public const string CAMERA_EFFECTS_PATH = "Assets/Resources/GameProfiles/";
    public const string DEFAULT_PROFILE = "Default";

    protected Animator animator;
    protected PostProcessVolume postProcessVolumeComponent;

    private List<PostProcessProfile> profiles;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        postProcessVolumeComponent = gameObject.GetComponent<PostProcessVolume>();

        if (!cameraEffectControllers.Contains(this))
            cameraEffectControllers.Add(this);

        profiles = Resources.LoadAll<PostProcessProfile>(CAMERA_EFFECTS_PATH).ToList();
        animator.fireEvents = true;
    }

    private void Update()
    {
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
    }

    public void PlayEffect(string animName)
    {
        animator.SetBool("TurnDefault", false);
        postProcessVolumeComponent.profile = Resources.Load<PostProcessProfile>(CAMERA_EFFECTS_PATH + animName + "Profile");
        animator.Play(animName);
    }

    private void OnEnable()
    {
        if (!cameraEffectControllers.Contains(this))
            cameraEffectControllers.Add(this);
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
