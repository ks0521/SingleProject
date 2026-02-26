// Assets/Editor/CreateMvpPassiveSkills_Strong.cs
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CreatePassiveSkills
{
    private const string RootFolder = "Assets/Gundam/Game/ScriptableObject/Passive/MVP";

    [MenuItem("Tools/Passives/Create MVP PassiveSkills (12 SO)")]
    public static void CreateAll()
    {
        EnsureFolderPath("Assets/Gundam/Game/ScriptableObject/Passive");
        EnsureFolderPath("Assets/Gundam/Game/ScriptableObject/Passive/MVP");
        EnsureFolderPath($"{RootFolder}/common");
        EnsureFolderPath($"{RootFolder}/Rare");
        EnsureFolderPath($"{RootFolder}/Unique");

        var defs = BuildDefs();

        int created = 0;
        int skipped = 0;

        foreach (var def in defs)
        {
            string rarityFolder = def.rarity switch
            {
                SkillRarity.Common => $"{RootFolder}/common",
                SkillRarity.Rare => $"{RootFolder}/Rare",
                SkillRarity.Unique => $"{RootFolder}/Unique",
                _ => RootFolder
            };

            string fileName = MakeSafeFileName($"{def.id}_{def.name}.asset");
            string path = $"{rarityFolder}/{fileName}";

            // 이미 있으면 스킵(덮어쓰고 싶으면 아래 로직 바꾸면 됨)
            var existing = AssetDatabase.LoadAssetAtPath<PassiveSkillSO>(path);
            if (existing != null)
            {
                skipped++;
                continue;
            }

            var so = ScriptableObject.CreateInstance<PassiveSkillSO>();
            so.id = def.id;
            so.skillName = def.name;
            so.type = def.type;
            so.rarity = def.rarity;
            so.advantageText = def.advantageText;
            so.disAdvantageText = def.disAdvantageText;
            so.lore = def.lore;
            so.status = def.status;
            so.isConditional = false; // 전부 상시발동
            so.activeHp = 0f;

            AssetDatabase.CreateAsset(so, path);
            created++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[MVP PassiveSkills] 생성 {created}개 / 스킵 {skipped}개 / 총 {defs.Count}개");
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(RootFolder);
    }

    // -------------------------
    // Definitions
    // -------------------------

    private class Def
    {
        public int id;
        public string name;
        public SkillType type;
        public SkillRarity rarity;
        public string advantageText;
        public string disAdvantageText;
        public string lore;
        public BonusStat status;
    }

    private static List<Def> BuildDefs()
    {
        // 밸런스 목적 X / 체감 목적 O
        // multiple*는 "추가 배율"로 가정: 0.10 = +10%
        // increseDamageReduction은 0~1 범위 개념으로 가정(음수도 테스트용 허용)

        return new List<Def>
        {
            // ---------------- common (4) ----------------
            new Def{
                id=101,
                name="급조 탄종 교체 | 화력강화형",
                type=SkillType.Attack,
                rarity=SkillRarity.Common,
                advantageText="데미지 +10%",
                disAdvantageText="피해 감소 -5%",
                lore="정비병이 남는 부품으로 만든 탄두. 강해지긴 했는데… 좀 잘 부서진다.",
                status=new BonusStat{ multipleDmg=+0.10f, increseDamageReduction=-0.05f }
            },
            new Def{
                id=102,
                name="간이 장갑판 | 방어강화형",
                type=SkillType.Defense,
                rarity=SkillRarity.Common,
                advantageText="피해 감소 +10%",
                disAdvantageText="이동 속도 -0.5",
                lore="얇지만 넓게 덧댄 장갑판. 둔해지는 건 어쩔 수 없다.",
                status=new BonusStat{ increseDamageReduction=+0.10f, increseSpeed=-0.5f }
            },
            new Def{
                id=103,
                name="충격 흡수 프레임 | 방어강화형",
                type=SkillType.Defense,
                rarity=SkillRarity.Common,
                advantageText="피해 감소 +6%",
                disAdvantageText="연사 -5%",
                lore="진동을 잡아주는 대신 구동 반응이 살짝 느려진다.",
                status=new BonusStat{ increseDamageReduction=+0.06f, multipleFireRate=-0.05f }
            },
            new Def{
                id=104,
                name="윤활 최적화 | 기동보조형",
                type=SkillType.Utililty,
                rarity=SkillRarity.Common,
                advantageText="이동 속도 +1.0",
                disAdvantageText="데미지 -5%",
                lore="관절이 부드러워진다. 대신 출력 배분이 조금 줄었다.",
                status=new BonusStat{ increseSpeed=+1.0f, multipleDmg=-0.05f }
            },

            // ---------------- Rare (4) ----------------
            new Def{
                id=201,
                name="고압 발사 모듈 | 화력강화형",
                type=SkillType.Attack,
                rarity=SkillRarity.Rare,
                advantageText="데미지 +25%",
                disAdvantageText="피해 감소 -8%",
                lore="순간 출력이 폭발한다. 기체가 버티는 건 별개의 문제다.",
                status=new BonusStat{ multipleDmg=+0.25f, increseDamageReduction=-0.08f }
            },
            new Def{
                id=202,
                name="연사 가속 서보 | 화력강화형",
                type=SkillType.Attack,
                rarity=SkillRarity.Rare,
                advantageText="연사 +20%",
                disAdvantageText="데미지 -8%",
                lore="빗발치듯 쏟아붓는다. 한 발 한 발은 가벼워졌다.",
                status=new BonusStat{ multipleFireRate=+0.20f, multipleDmg=-0.08f }
            },
            new Def{
                id=203,
                name="복합 장갑 코팅 | 방어강화형",
                type=SkillType.Defense,
                rarity=SkillRarity.Rare,
                advantageText="피해 감소 +18%",
                disAdvantageText="이동 속도 -0.8",
                lore="표면이 단단해질수록, 발걸음은 무거워진다.",
                status=new BonusStat{ increseDamageReduction=+0.18f, increseSpeed=-0.8f }
            },
            new Def{
                id=204,
                name="추진 분배 최적화 | 기동보조형",
                type=SkillType.Utililty,
                rarity=SkillRarity.Rare,
                advantageText="이동 속도 +2.0",
                disAdvantageText="연사 -10%",
                lore="부스터 계통에 우선권을 준다. 무장은 조금 양보했다.",
                status=new BonusStat{ increseSpeed=+2.0f, multipleFireRate=-0.10f }
            },

            // ---------------- Unique (4) ----------------
            new Def{
                id=301,
                name="과충전 사격 프로토콜 | 화력강화형",
                type=SkillType.Attack,
                rarity=SkillRarity.Unique,
                advantageText="데미지 +45%",
                disAdvantageText="피해 감소 -15%",
                lore="기체 수명과 교환한 순간 화력. 살아남으면 그걸로 됐다.",
                status=new BonusStat{ multipleDmg=+0.45f, increseDamageReduction=-0.15f }
            },
            new Def{
                id=302,
                name="탄막 제어 AI | 화력강화형",
                type=SkillType.Attack,
                rarity=SkillRarity.Unique,
                advantageText="연사 +40%",
                disAdvantageText="이동 속도 -1.0",
                lore="조준과 발사 루틴이 공격에만 집착한다.",
                status=new BonusStat{ multipleFireRate=+0.40f, increseSpeed=-1.0f }
            },
            new Def{
                id=303,
                name="요새화 프레임 | 방어강화형",
                type=SkillType.Defense,
                rarity=SkillRarity.Unique,
                advantageText="피해 감소 +35%",
                disAdvantageText="이동 속도 -1.5",
                lore="움직이는 벙커. 느리지만, 잘 죽지 않는다.",
                status=new BonusStat{ increseDamageReduction=+0.35f, increseSpeed=-1.5f }
            },
            new Def{
                id=304,
                name="전장 기동 튜닝 | 기동특화형",
                type=SkillType.Utililty,
                rarity=SkillRarity.Unique,
                advantageText="이동 속도 +3.5",
                disAdvantageText="데미지 -15%",
                lore="맞기 전에 사라지는 게 최선의 방어라는 주의.",
                status=new BonusStat{ increseSpeed=+3.5f, multipleDmg=-0.15f }
            },
        };
    }

    // -------------------------
    // Helpers
    // -------------------------
    private static void EnsureFolderPath(string assetPath)
    {
        string[] parts = assetPath.Split('/');
        string current = parts[0]; // "Assets"

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }

    private static string MakeSafeFileName(string s)
    {
        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            s = s.Replace(c, '_');
        return s.Replace("/", "_").Replace("\\", "_").Trim();
    }
}
#endif