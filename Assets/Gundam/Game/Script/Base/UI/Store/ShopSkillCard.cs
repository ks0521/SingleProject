using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShopSkillCard : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image frame;
    [SerializeField] private Image fill;

    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI rarity;
    [SerializeField] private TextMeshProUGUI advantage;
    [SerializeField] private TextMeshProUGUI disAdvantage;
    [SerializeField] private TextMeshProUGUI lore;

    [Header("Shop UI")]
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private RepairContext repairContext;
    private PassiveSkillSO _skill;
    private int _price;
    private Button _btn;

    // 기존 SkillPresenter 색상 유지
    private readonly Color32 offensiveFill = new Color32(255, 20, 20, 170);
    private readonly Color32 offensiveFrame = new Color32(255, 0, 0, 255);
    private readonly Color32 defensiveFill = new Color32(20, 20, 255, 175);
    private readonly Color32 defensiveFrame = new Color32(0, 0, 255, 255);
    private readonly Color32 utilityFill = new Color32(20, 255, 20, 175);
    private readonly Color32 utilityFrame = new Color32(0, 255, 0, 255);

    private void Awake()
    {
        _btn = GetComponent<Button>();
    }

    public void SetForShop(PassiveSkillSO skill)
    {
        if (_btn == null) _btn = GetComponent<Button>();
        if (_btn == null)
        {
            Debug.LogError($"ShopSkillCard: Button이 없습니다 -> {gameObject.name}");
            return;
        }
        _skill = skill;
        if (_skill == null)
        {
            Clear();
            return;
        }

        _price = RollPrice(_skill.rarity);

        // 버튼 세팅
        _btn.onClick.RemoveAllListeners();
        _btn.onClick.AddListener(TryBuy);
        _btn.interactable = true;

        // 텍스트/색상 세팅
        if (skillName) skillName.text = _skill.skillName;
        if (rarity) rarity.text = "[" + _skill.rarity.ToString() + "]";
        if (advantage) advantage.text = _skill.advantageText;
        if (disAdvantage) disAdvantage.text = _skill.disAdvantageText;
        if (lore) lore.text = _skill.lore;

        if (priceText) priceText.text = $"{_price}G";

        ApplyTypeColor(_skill.type);
        gameObject.SetActive(true);
    }

    public void Clear()
    {
        _skill = null;
        _price = 0;

        _btn.onClick.RemoveAllListeners();
        _btn.interactable = false;

        if (skillName) skillName.text = "";
        if (rarity) rarity.text = "";
        if (advantage) advantage.text = "";
        if (disAdvantage) disAdvantage.text = "";
        if (lore) lore.text = "";
        if (priceText) priceText.text = "";

        // 필요하면 비활성화
        // gameObject.SetActive(false);
    }

    private void TryBuy()
    {
        // 돈 부족 시 "아무 반응도 없음"
        if (_skill == null) return;
        if (PlayerInfoManager.Instance == null) return;

        if (!PlayerInfoManager.Instance.UseGold(_price))
            return;

        // 구매 성공
        PlayerInfoManager.Instance.AddSkill(_skill);
        repairContext.SetMoney();
        // 구매 후 처리(원하면 카드 비활성화/잠금)
        _btn.interactable = false;
        if (priceText) priceText.text = "SOLD";
    }

    private int RollPrice(SkillRarity rarity)
    {
        // Random.Range(int, int)는 max가 "미포함"이라 +1
        return rarity switch
        {
            SkillRarity.Common => Random.Range(90, 110 + 1),
            SkillRarity.Rare => Random.Range(180, 220 + 1),
            SkillRarity.Unique => Random.Range(260, 340 + 1),
            _ => Random.Range(90, 110 + 1),
        };
    }

    private void ApplyTypeColor(SkillType type)
    {
        if (fill == null || frame == null) return;

        if (type == SkillType.Attack)
        {
            fill.color = offensiveFill;
            frame.color = offensiveFrame;
        }
        else if (type == SkillType.Defense)
        {
            fill.color = defensiveFill;
            frame.color = defensiveFrame;
        }
        else
        {
            fill.color = utilityFill;
            frame.color = utilityFrame;
        }
    }
}