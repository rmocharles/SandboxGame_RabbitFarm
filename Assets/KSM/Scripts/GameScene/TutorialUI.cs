using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic[] skeletonGraphic;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;

    private int index = 0;

    public void Initialize(string type, bool isSave = false)
    {
        if (type == "Farm")
            index = 0;
        else
        {
            index = 1;
        }

        if (PlayerPrefs.GetInt("LangIndex") == 0)
        {
            skeletonGraphic[0].Skeleton.SetSkin("kor");
            skeletonGraphic[1].Skeleton.SetSkin("kor");
        }
        else
        {
            skeletonGraphic[0].Skeleton.SetSkin("eng");
            skeletonGraphic[1].Skeleton.SetSkin("eng");
        }

        skeletonGraphic[0].gameObject.SetActive(type == "Farm");
        skeletonGraphic[1].gameObject.SetActive(type == "Mart");
        
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            Destroy(this.gameObject);
            
            if (isSave)
            {
                if(type == "Farm")
                    StaticManager.Backend.backendGameData.UserData.SetTutorial(2);
            }
            
            GameManager.Instance.SaveAllData();
        });
        
        leftArrowButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            switch (skeletonGraphic[index].startingAnimation)
            {
                case "step1":
                    skeletonGraphic[index].startingAnimation = "step4";
                    skeletonGraphic[index].AnimationState.SetAnimation(0, "step4", false);
                    break;
                
                case "step2":
                    skeletonGraphic[index].startingAnimation = "step1";
                    skeletonGraphic[index].AnimationState.SetAnimation(0, "step1", false);
                    break;
                
                case "step3":
                    skeletonGraphic[index].startingAnimation = "step2";
                    skeletonGraphic[index].AnimationState.SetAnimation(0, "step2", false);
                    break;
                
                case "step4":
                    skeletonGraphic[index].startingAnimation = "step3";
                    skeletonGraphic[index].AnimationState.SetAnimation(0, "step3", false);
                    break;
            }

            EditorForceReloadSkeletonDataAssetAndComponent(skeletonGraphic[index].GetComponent<SkeletonRenderer>());
        });
        
        rightArrowButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            switch (skeletonGraphic[index].startingAnimation)
            {
                case "step1":
                    skeletonGraphic[index].startingAnimation = "step2";
                    skeletonGraphic[index].AnimationState.SetAnimation(0, "step2", false);
                    break;
                
                case "step2":
                    skeletonGraphic[index].startingAnimation = "step3";
                    skeletonGraphic[index].AnimationState.SetAnimation(0, "step3", false);
                    break;
                
                case "step3":
                    skeletonGraphic[index].startingAnimation = "step4";
                    skeletonGraphic[index].AnimationState.SetAnimation(0, "step4", false);
                    break;
                
                case "step4":
                    skeletonGraphic[index].startingAnimation = "step1";
                    skeletonGraphic[index].AnimationState.SetAnimation(0, "step1", false);
                    break;
            }

            EditorForceReloadSkeletonDataAssetAndComponent(skeletonGraphic[index].GetComponent<SkeletonRenderer>());
        });
    }

    void Start()
    {
        
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
