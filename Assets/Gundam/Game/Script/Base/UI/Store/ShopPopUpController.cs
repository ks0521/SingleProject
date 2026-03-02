using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopUpController : MonoBehaviour
{
    [Header("UI Root")]
    [SerializeField] private GameObject shopRoot; // 상점 패널(전체)

    [Header("Dependencies")]
    [SerializeField] private PassiveSkillManager passiveSkillManager;

    [Header("Cards (3)")]
    [SerializeField] private List<ShopSkillCard> cards = new(); // 3개 카드

    [Header("Draw Weights (Common/Rare/Unique)")]
    [SerializeField] private int[] shopWeights = new[] { 70, 20, 10 };
    
    [Header("Reroll")]
    [SerializeField] private Button rerollButton;
    [SerializeField] private int rerollCost = 100;
    [SerializeField] private RepairContext repairContext;

    
    private void Awake()
    {
        if (rerollButton != null)
        {
            rerollButton.onClick.RemoveAllListeners();
            rerollButton.onClick.AddListener(OnClickReroll);
        }
    }
    
    private void OnEnable()
    {
        if (shopRoot != null) shopRoot.SetActive(true);
        StartCoroutine(ShowAfterOneFrame(useEliteWeights: false));
 
    }
    private IEnumerator ShowAfterOneFrame(bool useEliteWeights)
    {
        yield return null; 
        Show3Cards(useEliteWeights);
    }
    
    private void OnClickReroll()
    {
        // 돈 없으면 무반응
        if (PlayerInfoManager.Instance == null) return;
        if (!PlayerInfoManager.Instance.UseGold(rerollCost)) return;
        repairContext.SetMoney();
        // 리롤은 엘리트 보상 가중치로
        Show3Cards(useEliteWeights: true);
    }
    
    private void Show3Cards(bool useEliteWeights)
    {
        if (passiveSkillManager == null || cards == null || cards.Count == 0) return;

        int[] weights = shopWeights;

        if (useEliteWeights)
        {
            // 엘리트 보상 비중 사용
            // PassiveSkillManager에 추가한 프로퍼티/메서드에 맞춰 한 줄만 맞춰주면 됨
            weights = passiveSkillManager.eliteRewards; // 또는 passiveSkillManager.GetEliteRewardWeights();
        }

        var picked = new HashSet<PassiveSkillSO>();
        int safety = 0;

        for (int i = 0; i < cards.Count; i++)
        {
            PassiveSkillSO skill = null;

            // 중복 방지 + 무한루프 방지
            do
            {
                skill = passiveSkillManager.DrawOneSkill(shopWeights);
                safety++;
                if (safety > 200) break;
            } while (skill != null && picked.Contains(skill));

            if (skill == null)
            {
                // 뽑기 실패면 카드 비워두기
                cards[i].Clear();
                continue;
            }

            picked.Add(skill);
            cards[i].SetForShop(skill);
        }
    }
}
