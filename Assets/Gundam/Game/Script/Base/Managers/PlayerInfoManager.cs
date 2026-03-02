using System.Collections.Generic;
using SO.Weapon;
using SO.Mech;
using UnityEngine;

/// <summary> 플레이어의 보유 패시브, 골드, 체력, 손상도 관리
/// 전투 시작시 여기서 Hp와 손상도를 받아와 초기 세팅
/// 전투 종료시 Hp와 손상도를 여기(PlayerInfoManager)에 갱신</summary>
public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager Instance;

    [SerializeField] private MechArcheTypeSO archeTypeSo;
    public MechArcheTypeSO ArcheTypeSo => archeTypeSo;
    //public MechRangeType RangeType => rangeType;
    [SerializeField] private List<PassiveSkillSO> _gainedSkill;
    public IReadOnlyList<PassiveSkillSO> GainedSkills => _gainedSkill;
    [SerializeField] private List<WeaponData> weapons; //현재 장착중인 무기
    [SerializeField] private BonusStat runtimeStat = new();
    [SerializeField] private int playerHp;
    [SerializeField] private int playerMaxHp;
    public int PlayerHp => playerHp;
    public int PlayerMaxHp => playerMaxHp;
    private int playerBreakdown; // 손상도
    [field:SerializeField]public int Gold { get; private set; }

    public void Clear()
    {
        _gainedSkill.Clear();
        runtimeStat = new BonusStat();
        Gold = 0;
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _gainedSkill = new List<PassiveSkillSO>();
    }

    public List<WeaponData> GetPlayerWeaponSettings() => weapons;

    public void SelectType(MechArcheTypeSO rangeType)
    {
        archeTypeSo = rangeType;
        playerHp = playerMaxHp = archeTypeSo.mechBaseStatus.maxHp;
        weapons = archeTypeSo.weaponLoadOut.weapons;
        Debug.Log($"플레이어 기체 타입 선택 : {archeTypeSo.rangeType}");
    }

    public BonusStat GetStatus() => runtimeStat;

    /// <summary> 새로 획득한 스킬을 보유중인 스킬 리스트에 추가</summary>
    /// <param name="newSkill">획득한 스킬</param>
    public void AddSkill(PassiveSkillSO newSkill)
    {
        _gainedSkill.Add(newSkill);
        Debug.Log("새로운 스킬 추가. 현재 획득한 스킬");
        runtimeStat += newSkill.status;
        playerHp += (int)newSkill.status.increaseHp;
        playerMaxHp += (int)newSkill.status.increaseHp;
        playerHp = Mathf.Clamp(playerHp,0,playerMaxHp);
    }

    /// <summary> 상시 발동형인 패시브의 스탯 증가량을 다 더해서 전달</summary>
    /// <returns> 패시브 스킬이 적용된 스킬들의 </returns>

    public void SetHp(int hp, int maxHp)
    {
        playerHp = hp;
        playerMaxHp = maxHp;
    }
    public void GetGold(int gold) => Gold += gold;

    public bool UseGold(int gold)
    {
        if (Gold < gold)
        {
            Debug.Log("사용할 골드가 부족합니다");
            return false;
        }

        Gold -= gold;
        return true;
    }
}