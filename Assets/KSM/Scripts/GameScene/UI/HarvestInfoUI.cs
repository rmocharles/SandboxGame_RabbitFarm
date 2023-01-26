using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HarvestInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text harvestNameText;
    [SerializeField] private TMP_Text harvestInfoText;
    [SerializeField] private TMP_Text harvestEffectText;
    [SerializeField] private TMP_Text harvestCoolTimeText;

    public void Initialize(int harvestCode)
    {
        if (PlayerPrefs.GetInt("LangIndex") == 0)
        {
            harvestNameText.text = StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].HarvestName;
            harvestInfoText.text = StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Info;
            harvestEffectText.text = StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Effect;
            harvestCoolTimeText.text = StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].CoolTime.ToString() + "초";
        }
        else
        {
            harvestNameText.text = StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].HarvestName_EN;
            harvestInfoText.text = StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Info_EN;
            harvestEffectText.text = StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Effect_EN;
            harvestCoolTimeText.text = StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].CoolTime.ToString() + "s";
        }
    }
}
