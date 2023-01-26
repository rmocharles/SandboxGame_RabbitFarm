using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 * BunnyManager
 *
 * 1. 특정 지역 돌아다님
 * 2. 여러 패턴들 존재 (Idle[1-3], Walk, Harvest, Work, Reward)
 * 3. Harvest - 농장에 있는 캐릭터 중 한명 수확 애니메이션
 *
 * 캐릭터 - 위치, 행동(애니메이션), 이벤트 관리
 */

public class BunnyManager : MonoBehaviour
{
    [SerializeField] private GameObject bunnyPool;
    
    //실질적 캐릭터 데이터
    public readonly Dictionary<int, BunnyController> bunnies = new Dictionary<int, BunnyController>();
    
    //수확할 버니 체크
    public int harvestBunnyNumber;

    public void Initialize()
    {
        for (int i = 0; i < bunnyPool.transform.childCount; i++)
        {
            var bunnyController = bunnyPool.transform.GetChild(i).GetComponent<BunnyController>();
            bunnies.Add(i, bunnyController);
        }
        
        
    }

    void Update()
    {
        //알바생이 존재하지 않을 경우
        if(StaticManager.Backend.backendGameData.PartTimeData.Type == -1 && bunnies[3].nowState != BunnyController.State.Work)
            ChangeBunnyState(3, BunnyController.State.Work);
        
        if(StaticManager.Backend.backendGameData.PartTimeData.Type != -1 && bunnies[3].nowState == BunnyController.State.Work)
        {
            MoveBunny(3, new Vector3(-7, -4, 0));
            ChangeBunnyState(3, BunnyController.State.Idle);
        }
    }

    public void MoveBunny(int index, Vector3 pos)
    {
        bunnies[index].MovePosition(pos);
    }

    public void ChangeBunnyState(int index, BunnyController.State state)
    {
        bunnies[index].ChangeState(state);
    }

    public BunnyController GetWorkBunny()
    {
        for (int i = 0; i < bunnies.Count; i++)
        {
            if (bunnies[i].nowState == BunnyController.State.Work)
                return bunnies[i];
        }

        return null;
    }
}
