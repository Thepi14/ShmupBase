using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraEffectControler : MonoBehaviour
{
    public static CameraEffectControler CameraEffectControlerInst;
    public Animator Animator => gameObject.GetComponent<Animator>();
    public PostProcessVolume PostProcessVolumeComponent => gameObject.GetComponent<PostProcessVolume>();
    void Start()
    {
        CameraEffectControlerInst = this;
        Animator.fireEvents = true;
    }
    void Update()
    {
        if (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Default") && !Animator.IsInTransition(0))
        {
            Animator.SetBool("TurnDefault", true);
        }
        else if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Default") && !Animator.IsInTransition(0) && Animator.GetBool("TurnDefault"))
        {
            PostProcessVolumeComponent.profile = Resources.Load<PostProcessProfile>("CameraVFX/Effects/DefaultProfile");
        }
    }
    public void PlayEffect(string animName)
    {
        Animator.SetBool("TurnDefault", false);
        PostProcessVolumeComponent.profile = Resources.Load<PostProcessProfile>("CameraVFX/Effects/" + animName + "Profile");
        Animator.Play(animName);
    }
}
