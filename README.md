# Mech Roguelike TPS

Unity/C# 기반 3D 메카 로그라이크 TPS 프로젝트입니다.  
게임 클라이언트 프로그래머 관점에서 전투 시스템, 책임 분리된 클래스 구조, ScriptableObject 데이터 설계를 중심으로 구현했습니다.

- **프로젝트 성격**: 1인 개발 포트폴리오 프로젝트
- **개발 기간**: 약 1개월 MVP 개발
- **개발 환경**: PC, Unity 2022 LTS+, C#
- [**기획서**](https://www.notion.so/2f110afad1cd80fd868de0a4df86b3fe?source=copy_link)
  
## 기술 스택

- **Engine**: Unity (URP)
- **Language**: C#
- **Async**: UniTask
- **Tween**: DOTween
- **Data**: ScriptableObject
- **Physics**: Rigidbody, Raycast, SphereCast, OverlapSphere
- **Runtime Pattern**: Object Pooling, Event-driven structure
  
## 개요

플레이어는 메카를 조작해 전투에 개입하고, 전투 보상으로 빌드를 성장시키며 노드 기반 맵을 진행합니다.

게임 제작간 핵심 로직은 다음과 같습니다.

- 플레이어('PlayerController')/NPC('NpcController') 조작 주체 분리와 공용 실행 계층('MechBehavior') 설계
- 무기 데이터 - 장착 구조 - 공격실행 책임 분리
- ScriptableObject 기반 데이터 중심 구조
- 상태 기반 AI와 거리중심 전투 로직 구현
- 오브젝트 풀링과 이벤트 기반 전투 런타임 구조 구성

## 조작법

| 입력 | 동작 |
| --- | --- |
| `WASD` | 이동 |
| `Mouse Left Click` | 공격 |
| `1 ~ 4` | 무기 변경 |
| `Ctrl` | 점프 |

## 핵심 특징

### 1) 전투 시스템
- Rigidbody 기반 이동 및 조준/사격 처리
- 카메라 중심 Raycast 기반 타겟팅
- 인터페이스 기반 피격 처리, 경직/사망 흐름 분리
- 전투 루프에서 플레이어 피드백 이벤트 연동

### 2) 무기 시스템
- `PlayerWeaponController`, `MechWeaponInventory`를 통한 장착/교체 구조
- `WeaponParts` 기반 공격 실행 책임 분리
- 공격 타입 분기: Raycast / Projectile / Explosion / Melee
- 탄약, 재장전, 발사 딜레이 및 스탯 반영 계산

### 3) AI 시스템
- 상태 기반 전투 로직(Seek, Approach, Attack, Retreat, Reposition, Stunned)
- 거리/시야/안전거리 기반 상태 전환
- 근/중/원거리 타입별 상태 전환 파라미터 분리
- 회전 속도 제한과 타겟 교체 조건으로 전투 템포 제어

### 4) 데이터 구조
- ScriptableObject 기반 데이터 자산 관리
  - `MechArcheTypeSO`: 기체 타입, 기본 스탯, 로드아웃, AI 파라미터
  - `MechBaseStatusSO`: 이동/체력 등 기본 수치
  - `WeaponLoadOut`: 기체별 무기 풀
  - `AIParameter`: 거리/공격/재배치 판단값
- `MechStatus`, `MechHealth`로 런타임 스탯/체력 상태 관리

### 5) 이벤트/풀링 구조
- `MechEventHub` 기반 이벤트 흐름 구성
- `PoolManager` 기반 Projectile/Explosion/NPC 오브젝트 재사용
- 풀 부족 시 확장, 수명 종료/충돌 시 반환 처리
- `MonsterSpawner`를 통한 적 생성 및 전투 흐름 연결

## 구조 설계

### 핵심 클래스 역할

| 영역 | 주요 클래스 | 역할 |
| --- | --- | --- |
| 입력/제어 | `PlayerController`, `NPCController` | 조작 주체 분리 (입력/AI 결정) |
| 공용 실행 | `MechBehavior` | 이동/회전/공격 공통 실행 |
| 무기 관리 | `PlayerWeaponController`, `MechWeaponInventory` | 장착 무기 관리 및 스왑 |
| 공격 실행 | `WeaponParts`, `WeaponAttack`, `ProjectileAttack` | 공격 타입별 실제 실행 |
| 상태/체력 | `MechStatus`, `MechHealth` | 런타임 스탯/피격/사망 처리 |
| 데이터 | ScriptableObject 계열 | 기체/무기/AI 수치와 규칙 데이터화 |
| 시스템 | `PoolManager`, `MonsterSpawner`, `MechEventHub` | 반복 생성 최적화, 스폰, 이벤트 전달 |

### 시스템 흐름 요약

```text
PlayerController / NPCController
        -> MechBehavior
        -> MechWeaponInventory / WeaponParts
        -> Hit & Event 처리(MechHealth, MechEventHub)
        -> PoolManager 재사용 / MonsterSpawner 전투 진행
```

## 진행도

### 구현 완료
- 플레이어 이동/점프 및 조준/공격
- 무기 교체 구조 및 4종 공격 타입 처리
- 상태 기반 NPC 전투 AI
- 체력/피격/사망 처리 및 전투 종료 판정
- 오브젝트 풀링, 기본 월드맵 흐름, 기본 상점 화면
- 패시브 기반 스탯 증가 구조

### 미구현 / 제한 사항
- **Start**: 옵션 설정 미구현
- **WorldMap**: Repair / Elite / Boss 노드 미구현
- **Store**: 장비 교체 / 강화 / 지침 UI 미구현




