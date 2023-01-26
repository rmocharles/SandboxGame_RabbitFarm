using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class RewardEffect : MonoBehaviour
{
    //List <아이템 아이디, 아이템 개수>
    [SpineAnimation(dataField: "skeletonDataAsset")]
    public string onceAnimation;
    [SpineAnimation(dataField: "skeletonDataAsset")]
    public string twiceAnimation;

    public enum Point
    {
        Up, Center, Down
    }

    public class Item
    {
        public int itemCode;
        public int itemCount;

        public Item(int itemCode, int itemCount)
        {
            this.itemCode = itemCode;
            this.itemCount = itemCount;
        }
    }

    public void Initialize(int amount)
    {
        GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, onceAnimation, true);
        StartCoroutine(CreateRewardObject(Point.Center, -1, amount, 1.6f));
        Invoke(nameof(DestroyEffect), 1.6f);
    }

    public void Initialize(int itemCode, int amount = 1)
    {
        GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, onceAnimation, true);
        StartCoroutine(CreateRewardObject(Point.Center, itemCode, amount, 1.6f));
        Invoke(nameof(DestroyEffect), 1.6f);
    }

    public void Initialize(Dictionary<int, Item> list)
    {
        switch (list.Count)
        {
            case 1:
                GetComponent<SkeletonGraphic>().startingAnimation = onceAnimation;
                GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, onceAnimation, true);
                
                for (int i = 0; i < 3; i++)
                {
                    if(list.ContainsKey(i))
                        StartCoroutine(CreateRewardObject(Point.Center, list[i].itemCode, list[i].itemCount, 1.6f));
                }
                Invoke(nameof(DestroyEffect), 1.6f);
                break;
            
            case 2:
                GetComponent<SkeletonGraphic>().startingAnimation = onceAnimation;
                GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, onceAnimation, true);
                
                int num = 0;

                for (int i = 0; i < 3; i++)
                {
                    if (list.ContainsKey(i))
                    {
                        if(num == 0)
                            StartCoroutine(CreateRewardObject(Point.Up, list[i].itemCode, list[i].itemCount, 1.6f));
                        else
                            StartCoroutine(CreateRewardObject(Point.Down, list[i].itemCode, list[i].itemCount, 1.6f));
                        
                        num++;
                    }
                }
                
                Invoke(nameof(DestroyEffect), 1.6f);
                break;
            
            case 3:
                GetComponent<SkeletonGraphic>().startingAnimation = twiceAnimation;
                GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, twiceAnimation, true);

                int num2 = 0;
                
                for (int i = 0; i < 3; i++)
                {
                    if (list.ContainsKey(i))
                    {
                        
                        if(num2 == 0)
                            StartCoroutine(CreateRewardObject(Point.Up, list[i].itemCode, list[i].itemCount, 1.2f));
                        else if(num2 == 1)
                            StartCoroutine(CreateRewardObject(Point.Down, list[i].itemCode, list[i].itemCount, 1.2f));
                        else
                            StartCoroutine(CreateRewardObject(Point.Center, list[i].itemCode, list[i].itemCount, 1.1f, 1f));
                        
                        num2++;
                    }
                }
                
                Invoke(nameof(DestroyEffect), 2.3f);
                break;
            
            case 4:
                GetComponent<SkeletonGraphic>().startingAnimation = twiceAnimation;
                StartCoroutine(CreateRewardObject(Point.Up, list[0].itemCode, list[0].itemCount, 0.3f));
                StartCoroutine(CreateRewardObject(Point.Up, list[1].itemCode, list[1].itemCount, 0.3f));
                StartCoroutine(CreateRewardObject(Point.Up, list[2].itemCode, list[2].itemCount, 0.8f, 0.5f));
                StartCoroutine(CreateRewardObject(Point.Down, list[3].itemCode, list[3].itemCount, 0.8f, 0.5f));
                break;
        }
        
        EditorForceReloadSkeletonDataAssetAndComponent(GetComponent<SkeletonRenderer>());
    }

    private IEnumerator CreateRewardObject(Point point, int itemCode, int itemCount, float duration = 1, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        string boneName = string.Empty;
        switch (point)
        {
            case Point.Up:
                boneName = "textpoint2";
                break;
            
            case Point.Center:
                boneName = "textpoint1";
                break;
            
            case Point.Down:
                boneName = "textpoint";
                break;
        }
        
        GameObject rewardOjbect = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/Effect/Reward_Layout"), transform);
        rewardOjbect.GetComponent<BoneFollowerGraphic>().skeletonGraphic = GetComponent<SkeletonGraphic>();
        rewardOjbect.GetComponent<BoneFollowerGraphic>().boneName = boneName;
        rewardOjbect.GetComponent<Reward_Layout>().Initialize(itemCode, itemCount, duration);
    }

    private void DestroyEffect()
    {
        Destroy(this.gameObject);
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
