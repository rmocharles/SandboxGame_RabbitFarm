using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            Destroy(this.gameObject);
            StaticManager.Backend.backendGameData.UserData.SetTutorial(2);
            GameManager.Instance.SaveAllData();
        });
        
        leftArrowButton.onClick.AddListener(() =>
        {
            Debug.LogError(skeletonGraphic.startingAnimation);
            switch (skeletonGraphic.startingAnimation)
            {
                case "step1":
                    skeletonGraphic.startingAnimation = "step4";
                    skeletonGraphic.AnimationState.SetAnimation(0, "step4", false);
                    break;
                
                case "step2":
                    skeletonGraphic.startingAnimation = "step1";
                    skeletonGraphic.AnimationState.SetAnimation(0, "step1", false);
                    break;
                
                case "step3":
                    skeletonGraphic.startingAnimation = "step2";
                    skeletonGraphic.AnimationState.SetAnimation(0, "step2", false);
                    break;
                
                case "step4":
                    skeletonGraphic.startingAnimation = "step3";
                    skeletonGraphic.AnimationState.SetAnimation(0, "step3", false);
                    break;
            }

            EditorForceReloadSkeletonDataAssetAndComponent(skeletonGraphic.GetComponent<SkeletonRenderer>());
        });
        
        rightArrowButton.onClick.AddListener(() =>
        {
            Debug.LogError(skeletonGraphic.startingAnimation);
            switch (skeletonGraphic.startingAnimation)
            {
                case "step1":
                    skeletonGraphic.startingAnimation = "step2";
                    skeletonGraphic.AnimationState.SetAnimation(0, "step2", false);
                    break;
                
                case "step2":
                    skeletonGraphic.startingAnimation = "step3";
                    skeletonGraphic.AnimationState.SetAnimation(0, "step3", false);
                    break;
                
                case "step3":
                    skeletonGraphic.startingAnimation = "step4";
                    skeletonGraphic.AnimationState.SetAnimation(0, "step4", false);
                    break;
                
                case "step4":
                    skeletonGraphic.startingAnimation = "step1";
                    skeletonGraphic.AnimationState.SetAnimation(0, "step1", false);
                    break;
            }

            EditorForceReloadSkeletonDataAssetAndComponent(skeletonGraphic.GetComponent<SkeletonRenderer>());
        });
    }
    
    private void EditorForceReloadSkeletonDataAssetAndComponent (SkeletonRenderer component) {
        if (component == null) return;

        // Clear all and reload.
        if (component.skeletonDataAsset != null) {
            foreach (AtlasAssetBase aa in component.skeletonDataAsset.atlasAssets) {
                if (aa != null) aa.Clear();
            }
            component.skeletonDataAsset.Clear();
        }
        component.skeletonDataAsset.GetSkeletonData(true);

        // Reinitialize.
        EditorForceInitializeComponent(component);
    }

    private void EditorForceInitializeComponent (SkeletonRenderer component) {
        if (component == null) return;
        if (!SkeletonDataAssetIsValid(component.SkeletonDataAsset)) return;
        component.Initialize(true);

#if BUILT_IN_SPRITE_MASK_COMPONENT
         SpineMaskUtilities.EditorAssignSpriteMaskMaterials(component);
#endif

        component.LateUpdate();
    }

    private bool SkeletonDataAssetIsValid (SkeletonDataAsset asset) {
        return asset != null && asset.GetSkeletonData(quiet: true) != null;
    }
}
