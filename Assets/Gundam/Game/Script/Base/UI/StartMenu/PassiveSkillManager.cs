using System.Collections.Generic;
using Base.Managers;
using UnityEngine;

public class PassiveSkillManager : MonoBehaviour
{
    public Dictionary<SkillRarity, List<PassiveSkillSO>> skillDic = new();
    [SerializeField] private List<PassiveSkillSO> allSkillList; //딕셔너리 만들기 위한 풀
    [SerializeField] private List<SkillPresenter> presenters;
    public int[] battleRewards = new[] { 70, 20, 10 };
    public int[] eliteRewards = new[] { 50, 30, 20 };
    public int[] bossRewards = new[] { 10, 50, 40 };
    private SkillRarity rarity;

    private void Awake()
    {
        foreach (var skill in allSkillList)
        {
            //딕셔너리에 skill의 레어리티가 없으면 리스트 새로 만들기 
            if (!skillDic.TryGetValue(skill.rarity, out List<PassiveSkillSO> list))
            {
                list = new List<PassiveSkillSO>();
                skillDic.Add(skill.rarity,list);
            }
            list.Add(skill);
        }
    }

    /// <summary> 선택한 스킬을 플레이어 패시브 리스트에 추가</summary>
    public void ChoiceSkill(PassiveSkillSO selectSkill)
    {
        PlayerInfoManager.Instance.AddSkill(selectSkill);
    }
    
    public void MakeReward(Scenes clearScene)
    {
        List<PassiveSkillSO> pickedList = new();
        PassiveSkillSO pickedSkill;
        int count = 0;
        if (clearScene != Scenes.Battle)
        {
            Debug.LogWarning("전투 클리어 경로가 아닙니다. ");
            return;
        }

        int[] rewardWeight = battleRewards;
        switch (clearScene)
        {
            case Scenes.Battle:
                rewardWeight = battleRewards;
                break;
        }
        foreach (var skill in presenters)
        {
            if (!skill.gameObject.activeSelf) continue; //비활성화된 스킬옵션칸은 활성화 X
            do
            {
                pickedSkill = DrawSkill(rewardWeight);
            } while (!pickedList.Contains(pickedSkill) && ++count < 200);
            skill.SetSkill(pickedSkill);
            pickedList.Add(pickedSkill);
        }
    }
    /// <summary>클리어 난이도에 따른 변동확률로 보상스킬 뽑은 후 반환</summary>
    /// <param name="clearScene">클리어한 전투 난이도</param>
    PassiveSkillSO DrawSkill(int []rewardWeights)
    {
        rarity = (SkillRarity)CalcWeight(rewardWeights);
        
        if (!skillDic.TryGetValue(rarity, out var list) || list == null || list.Count == 0)
        {
            Debug.LogWarning($"{rarity}의 스킬이 없거나 리스트가 비어있음, 모든 스킬중에서 선택");
            return DrawSkillAllSkillpool();
        }
        int idx = UnityEngine.Random.Range(0, list.Count);
        return list[idx];
    }
    /// <summary> 희귀도 구분 없이 무조건 랜덤으로 뽑음 </summary>
    /// <returns></returns>
    PassiveSkillSO DrawSkillAllSkillpool()
    {
        if (allSkillList == null || allSkillList.Count == 0)
        {
            Debug.LogWarning("DB에 아무 스킬도 없음!");
            return null;
        }

        int idx = UnityEngine.Random.Range(0, allSkillList.Count);
        return allSkillList[idx];
    }
    public PassiveSkillSO DrawOneSkill(int[] rewardWeights)
    {
        return DrawSkill(rewardWeights);
    }
    public void TurnOff()
    {
        gameObject.SetActive(false);
    }
    /// <summary> 희귀도별 가중치를 입력받아 가중합 랜덤 결과를 반환</summary>
    /// <param name="weight">가중치 배열</param>
    /// <returns>배열의 인덱스중 1개</returns>
    public int CalcWeight(int[] weight)
    {
        //계산은 누적합 방식으로 계산
        int total = 0;
        for (int i = 0; i < weight.Length; i++)
        {
            if (weight[i] < 0)
            {
                Debug.LogWarning($"가중치 입력이 잘못되었습니다 weight[{i}] = {weight[i]}");
                return 0;
            }

            total += weight[i];
            //오류 발생 시 최하등급 부여
        }

        if (total == 0)
        {
            Debug.LogWarning("가중치 배열의 합이 0입니다!");
            return 0;
        }

        int random = UnityEngine.Random.Range(0, total);
        for (int i = 0; i < weight.Length; i++)
        {
            if (random >= weight[i])
            {
                random -= weight[i];
            }
            else return i;
        }

        Debug.Log("입력값 오류");
        return 0;
    }

}