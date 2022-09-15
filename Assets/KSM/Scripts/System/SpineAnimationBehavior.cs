using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SpineAnimationBehavior : StateMachineBehaviour
{
    public AnimationClip motion;
    private string animationClip;

    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState spineAnimationState;
    private Spine.TrackEntry trackEntry;

    [Header("< Spine Motion Layer >")]
    public int layer = 0;
    public float timeScale = 1.0f;
    private bool loop;

    void Awake()
    {
        if(motion != null)
        {
            animationClip = motion.name;
        }
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(skeletonAnimation == null)
        {
            skeletonAnimation = animator.GetComponentInChildren<SkeletonAnimation>();
            spineAnimationState = skeletonAnimation.state;
        }

        if(animationClip != null)
        {
            loop = stateInfo.loop;
            trackEntry = spineAnimationState.SetAnimation(layer, animationClip, loop);
            trackEntry.TimeScale = timeScale;
        }
    }

    //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    Debug.LogError(skeletonAnimation.name);
    //    Debug.LogError(skeletonAnimation.state);
    //}

    //public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    Debug.LogError(animator.layer)
    //    Debug.LogError(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    //    Debug.LogError(skeletonAnimation.AnimationName);
    //}
}
