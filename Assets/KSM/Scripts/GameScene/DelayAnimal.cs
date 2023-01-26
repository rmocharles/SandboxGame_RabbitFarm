using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class DelayAnimal : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "idle_r", true);
    }
}
