
//2021-02-10
namespace Code
{
    public static class Character
    {
        public static class TYPE_GRADE        //등급 구분 코드
        {
            public const int None = 0;  //	해당없음
            public const int Legend = 1;    //	레전드
            public const int Epic = 2;  //	에픽
            public const int Rare = 3;  //	레어
            public const int Normal = 4;	//	노멀
        }
        public static class TYPE_CLASS        //클래스 구분 코드
        {
            public const int None = 0;  //	해당없음
            public const int Striker = 1;    //	Striker
            public const int Tanker = 2; //	Knight
            public const int Caster = 3; //	Wizard
            public const int Supporter = 4; //	Priest
            public const int ClaAssassin = 5;   //	Assassin
            public const int ClaRanger = 6;	//	Ranger
        }
        public static class TYPE_REGION
        {
            public const int None = 0;
            public const int Gods = 1;
            public const int Elf = 2;
            public const int Human = 3;
            public const int Barren = 4;
            public const int Machine = 5;
            public const int Desperado = 6;
        }

        public static class TYPE_STATUS        //클래스 스텟
        {
            public const int None = (int)ENUM_TYPE_STATUS.None;                     //	해당없음
            public const int Hp = (int)ENUM_TYPE_STATUS.Hp;                         //	체력
            public const int Atk = (int)ENUM_TYPE_STATUS.Atk;                       //	공격력
            public const int Def = (int)ENUM_TYPE_STATUS.Def;                       //	방어력
            public const int Spd = (int)ENUM_TYPE_STATUS.Spd;                       //	속도            
            public const int Crit = (int)ENUM_TYPE_STATUS.Crit;                     //	치명타
            public const int CritPower = (int)ENUM_TYPE_STATUS.CritPower;           //	치명타 위력
            public const int Block = (int)ENUM_TYPE_STATUS.Block;                   //	블록
            public const int Dodge = (int)ENUM_TYPE_STATUS.Dodge;                   //	회피
            public const int Accuracy = (int)ENUM_TYPE_STATUS.Accuracy;             // 명중 확률
            public const int EffectRes = (int)ENUM_TYPE_STATUS.EffectRes;           // 효과 저항
            public const int EffectAcc = (int)ENUM_TYPE_STATUS.EffectAcc;           // 효과 적중
            public const int Pierce = (int)ENUM_TYPE_STATUS.Pierce;                 // 관통 비율
            public const int Reflect = (int)ENUM_TYPE_STATUS.Reflect;               // 반사 비율
            public const int EnhanceDamage = (int)ENUM_TYPE_STATUS.EnhanceDamage;   // 주는 피해 증가
            public const int AmplifyDamage = (int)ENUM_TYPE_STATUS.AmplifyDamage;   // 받는 피해 감소
            public const int Vamp = (int)ENUM_TYPE_STATUS.Vamp;                     // 흡혈 배율
            public const int HealOverTime = (int)ENUM_TYPE_STATUS.HealOverTime;     // 턴 당 회복량
            public const int Percept = (int)ENUM_TYPE_STATUS.Percept;               // 감각
            public const int Craft = (int)ENUM_TYPE_STATUS.Craft;                   // 기술
            public const int Max = (int)ENUM_TYPE_STATUS.Max;
        }
        public enum ENUM_TYPE_STATUS        //클래스 스텟
        {
            None = 0,       //	해당없음
            Hp,             //	체력
            Atk,            //	공격력
            Def,            //	방어력
            Spd,            //	속도            
            Crit,           //	치명타
            CritPower,      //	치명타 위력
            Block,	        //	블록
            Dodge,          //	회피            
            Accuracy,       // 명중 확률
            EffectRes,      // 효과 저항
            EffectAcc,      // 효과 적중
            Pierce,         // 관통 비율
            Reflect,        // 반사 비율
            EnhanceDamage,  // 주는 피해 증가
            AmplifyDamage,  // 받는 피해 감소
            Vamp,           // 흡혈 배율
            HealOverTime,   // 턴 당 회복량
            Percept,        // 감각
            Craft,          // 기술
            Max	            //	MAX
        }


        public enum ENUM_VOICE_SOUND
        {
            advent = 0, //	출현
            turn01, //	턴순서1
            turn02, //	턴순서2
            death,  //	사망
            skill01,    //	1번스킬사용
            skill02,    //	2번스킬사용
            shout01,    //	외침1
            shout02,    //	외침2
            Max		//	
        }
        public static class TYPE_HERO
        {
            public const int None = 0;
            public const int Harmony = 1;
            public const int Prosperity = 2;
            public const int Destruction = 3;
        }
        public static class KIND_MONSTER
        {
            public const int None = 0;
            public const int Monster = 1;
            public const int Boss = 2;
        }
    }

    public static class AI
    {
        public static class AITARGET_TYPE
        {
            public const int None = 0;          //	해당없음
            public const int Index = 1;         //	인덱스 순
            public const int Random = 2;        //	랜덤
            public const int Region = 3;        //	우위 대상
            public const int LifeLow = 4;       //	생명력 낮은 순
            public const int LifeHigh = 5;		//	생명력 높은 순
            public const int RegionLife = 6;	//	우위 대상 중 생명력 낮은 순
        }

        public static class AIFAIL_ACTION
        {
            public const int None = 0;          //	해당없음
            public const int Left = 1;          //	좌측카드 사용
            public const int Right = 2;         //	우측카드 사용
            public const int Anything = 3;      //	랜덤카드 사용
            public const int TurnEnd = 4;		//	턴 종료
            public const int NextWeight = 5;	//	턴 유지 결정
            public const int Next0 = 6;	        //	0번 연결 시퀀스
            public const int Next1 = 7;     	//	1번 연결 시퀀스
            public const int Next2 = 8;	        //	2번 연결 시퀀스
        }





        public static class SKILL_CATEGORY
        {
            public const int None = 0;
            public const int Attack1 = 1;           // 단일 대상을 공격(피해, 디버프 등)하는 종류의 스킬.
            public const int Attack4 = 2;           // 복수의 대상을 공격(피해, 디버프 등)하는 종류의 스킬.
            public const int Protection = 3;        // 아군을 보호or지원 하는 종류의 스킬.
            public const int RemoveCC = 4;          // 아군에게 걸려있는 CC를 해제하는 종류의 스킬.
            public const int RemoveDebuff = 5;      // 아군에게 걸려있는 디버프를 해제하는 종류의 스킬.
            public const int RemoveBuff = 6;        // 아군에게 걸려 있는 버프를 해제하는 종류의 스킬.
        }

        public static class SCORE_TYPE
        {
            public const int None = 0;
            public const int ScoreSkill = 1;
            public const int ScoreTarget = 2;
        }

        public static class COND_SKILL
        {
            public const int None = 0;
            public const int StageBoss = 1;         // 자신이 스테이지에 지정된 보스일 경우 조건 달성
            public const int StageBossNot = 2;      // 자신이 스테이지에 지정된 보스가 아닐 경우 조건 달성
            public const int AllyLife40Under = 3;   // 생명력이 40% 이하인 아군이 있을 경우 조건 달성
            public const int AllyLife70Under = 4;   // 생명력이 70% 이하인 아군이 있을 경우 조건 달성
            public const int Enemy3Over = 5;        // 생존한 적이 3명 이상일 경우 조건 달성
            public const int Enemy2Under = 6;       // 생존한 적이 2명 이하일 경우 조건 달성
            public const int Mana3Under = 7;        // 보유한 마나가 3 이하일 경우 조건 달성
            public const int Card3RankNot = 8;      // 3단계 카드가 없을 경우 조건 달성
            public const int AllCC = 9;             // CC에 걸려 있는 아군이 있을 경우 조건 달성
            public const int AllyCCNot = 10;        // CC에 걸려 있는 아군이 없을 경우 조건 달성
            public const int AllyDebuff = 11;       // 디버프에 걸려 있는 아군이 있을 경우 조건 달성
            public const int AllyDebuffNot = 12;    // 디버프에 걸려 있는 아군이 없을 경우 조건 달성
            public const int EnemyBuff = 13;        // 버프에 걸려 있는 적이 있을 경우 조건 달성
            public const int EnemyBuffNot = 14;     // 버프에 걸려 있는 적이 없을 경우 조건 달성
        }

        public static class SCORE_SKILL
        {
            public const int None = 0;              // 대상이 없으므로, 아무도 점수를 얻지 못하는 것으로 처리한다.
            public const int RandomMax = 1;         // 조건을 달성하는 선택지들에 추가되는 랜덤 점수.0과 점수의 값 사이에서 랜덤하게 결정된다.			
            public const int RandomTurn = 2;        // 점수의 값을 현재 턴 수에 곱해서, 랜덤 점수에 더한다. [최대 랜덤 점수 + (턴 수 * 턴당 증가 랜덤 점수) ]가 최대값이 된다.			
            public const int Card3Rank = 3;         // 3단계 카드가 점수를 얻는다.
            public const int Card2Rank = 4;         // 2단계 카드가 점수를 얻는다.
            public const int Card1Rank = 5;         // 1단계 카드가 점수를 얻는다.
            public const int SameType = 6;          // 자신과 카드 타입이 일치하는 카드가 점수를 얻는다.
            public const int SameTypeBoss = 7;      // 보스와 카드 타입이 일치하는 카드가 점수를 얻는다.
            public const int SkillAttack1 = 8;      // 공격1 계열 주문 또는 카드가 점수를 얻는다.
            public const int SkillAttack4 = 9;      // 공격4 계열 주문 또는 카드가 점수를 얻는다.
            public const int SkillProtection = 10;  // 보호 계열 주문 또는 카드가 점수를 얻는다.
            public const int SkillRemoveCC = 11;    // CC 해제 계열 주문 또는 카드가 점수를 얻는다.
            public const int SkillRemoveDebuff = 12;// 디버프 해제 계열 주문 또는 카드가 점수를 얻는다.
            public const int SkillRemoveBuff = 13;  // 버프 해제 계열 주문 또는 카드가 점수를 얻는다.
        }

        public static class COND_TARGET
        {
            public const int None = 0;              // 조건이 없으므로, 무조건 조건 달성으로 처리한다.
            public const int UseAttack1 = 1;        // 공격1 계열 스킬 사용중일 경우 조건 달성
            public const int UseAttack4 = 2;        // 공격4 계열 스킬 사용중일 경우 조건 달성
            public const int UseProtection = 3;     // 보호 계열 스킬 사용중일 경우 조건 달성
            public const int UseRemoveCC = 4;       // CC 해제 계열 스킬 사용중일 경우 조건 달성
            public const int UseRemoveDebuff = 5;   // 디버프 해제 계열 스킬 사용중일 경우 조건 달성
            public const int UseRemoveBuff = 6;     // 버프 해제 계열 스킬 사용중일 경우 조건 달성
        }

        public static class SCORE_TARGET
        {
            public const int None = 0;              // 대상이 없으므로, 아무도 점수를 얻지 못하는 것으로 처리한다.
            public const int RandomMax = 1;         // 조건을 달성하는 선택지들에 추가되는 랜덤 점수. 0과 점수의 값 사이에서 랜덤하게 결정된다.
            public const int RandomTurn = 2;        // 점수의 값을 현재 턴 수에 곱해서, 랜덤 점수에 더한다. [최대 랜덤 점수 + (턴 수 * 턴당 증가 랜덤 점수) ]가 최대값이 된다.		
            public const int AllyLifeLow = 3;       // 현재 생명력이 가장 낮은 아군이 점수를 얻는다.
            public const int AllyATKHigh = 4;       // 공격력이 가장 높은 아군이 점수를 얻는다.
            public const int AllyDEFHigh = 5;       // 방어력이 가장 높은 아군이 점수를 얻는다.
            public const int AllySPDHigh = 6;       // 속도가 가장 높은 아군이 점수를 얻는다.
            public const int AllyCondHigh = 7;      // 현재 맵 컨디션이 가장 높은 아군이 점수를 얻는다.
            public const int EnemyLifeLow = 8;      // 현재 생명력이 가장 낮은 적이 점수를 얻는다.
            public const int EnemyATKHigh = 9;      // 공격력이 가장 높은 적이 점수를 얻는다.
            public const int EnemyDEFHigh = 10;     // 방어력이 가장 높은 적이 점수를 얻는다.
            public const int EnemySPDHigh = 11;     // 속도가 가장 높은 적이 점수를 얻는다.
            public const int EnemyCondHigh = 12;    // 현재 맵 컨디션이 가장 높은 적이 점수를 얻는다.
            public const int EnemyRegionWeak = 13;  // 진영상성에서 나에게 약한 적이 점수를 얻는다.
            public const int AllyCC = 14;           // CC에 걸려 있는 아군이 점수를 얻는다.
            public const int AllyDebuff = 15;       // 디버프에 걸려 있는 아군이 점수를 얻는다.
            public const int EnemyBuff = 16;        // 버프에 걸려 있는 적이 점수를 얻는다.
        }
    }

    public static class Skill
    {
        public static class SKILL_TYPE
        {
            public const int None = 0;   //	해당없음
            public const int Active = 1; //	액티브
            public const int Summon = 2; //	소환
            public const int Trigger = 3;    //	트리거
            public const int Retaliate = 4;		//	반격
            public const int DeathSkill = 5;		//	사망 스킬
            public const int Revive = 6;		//	환생
            public const int PlayerSpell = 7;		//	PlayerSpell
        }
        public static class TARGETABLE_RANGE
        {
            public const int None = 0;   //	해당없음
            public const int Self = 1;   //	스킬 시전자 본인
            public const int Allies = 2; //	아군 영웅
            public const int Enemies = 3;    //	적 영웅
            public const int All = 4;    //	모든 영웅
            public const int AllNoSelf = 5; 	//	본인 제외 아군
            public const int Retaliate = 6;		//	반격
        }
        public static class TARGETING_TYPE
        {
            public const int None = 0;   //	해당 없음
            public const int All = 1;    //	범위 모두
            public const int RandomOne = 2;  //	랜덤 1인
            public const int FullRandom = 3; //	랜덤 1인(매 히트별)
            public const int TargetOne = 4;  //	대상 1인
            public const int TargetBounce = 5;   //	대상 1인 튕기는 발사체
            public const int TargetContinuous = 6;   //	대상 1인 사망시 타겟 교체
            public const int RandomTwo = 7;  //	랜덤 2인
            public const int LowestHp = 8;		//	HP가 가장 낮은 1인
            public const int RandomTwoBounce = 9;  //	랜덤 2인 교차 대미지
        }
        public static class MOTION_TYPE
        {
            public const int None = 0;   //	해당 없음
            public const int MeleeClose = 1; //	근거리 시전 대상 앞
            public const int MeleeFront = 2; //	근거리 적 진영 앞
            public const int Range = 3;  //	제자리 시전
            public const int RangeProj = 4;  //	제자리 시전 발사체 발사
            public const int RangeChain = 5; //	제자리 시전 빔 연결
            public const int MeleeMid = 6;   //	근거리 적진 중앙
            public const int MeleeBehind = 7;    //	근거리 대상 뒤
            public const int SummonMelee1 = 8;   //	소환수 근거리(자피나)
            public const int SummonMelee2 = 9;		//	소환수 근거리(베라)
            public const int AllyMid = 10;     //아군 중앙
            public const int Mid = 11;      //정 중앙
            public const int RangeReturnProj = 12;   //제자리 돌아오는 발사체
            public const int RangeChainMid = 13;   //정 중앙 빔 연결
        }
        public static class TRIGGER_OBJECT
        {
            public const int None = 0;   //	해당없음
            public const int Self = 1;   //	스킬 시전자 본인
            public const int Allies = 2; //	아군 영웅
            public const int Enemies = 3;    //	적 영웅
            public const int All = 4;    //	모든 영웅
            public const int AllNoSelf = 5;		//	본인 제외 아군
        }
        public static class TRIGGER_CONDITION
        {
            public const int None = 0;   //	해당없음
            public const int Summoned = 1;   //	소환 되었을 때
            public const int Dead = 2;   //	사망 하였을 때
            public const int TurnStart = 3;  //	턴 시작 시
            public const int TurnEnd = 4;    //	턴 종료 시
            public const int SkillDamaged = 5;   //	스킬효과 대미지 입을 시
            public const int SkillHealed = 6;    //	스킬효과 힐을 받을 시
            public const int GainStatusEffect = 7;   //	상태이상 획득 시
            public const int LoseStatusEffect = 8;   //	상태이상 제거 시
            public const int CritDamaged = 9;    //	치명타 대미지를 입을 시
            public const int CritHealed = 10; //	치명타 힐을 받을 시
            public const int Dodge = 11;  //	회피 할 시
            public const int Block = 12;  //	블록 할 시
            public const int CastSkill = 13;  //	스킬 사용 시
            public const int DealCritDamage = 14; //	치명타 대미지를 줄 시
            public const int DealCritHeal = 15;   //	치명타 힐을 줄 시
            public const int OnDeath = 16;		//	사망 처리 되는 도중
            public const int SkillSpdGuageDown = 17; // 스킬효과로 행동력 감소 시
            public const int HpUpDown = 18; // HP 변동 시
            public const int OneAtkSuccess = 19; // 단일 공격 성공 시
            public const int AllAtkSuccess = 20; // 전체 공격 성공 시
            public const int NormalDamaged = 21; // 극대화 미발생 시
        }
        public static class TRIG_STATUS_EFFECT
        {
            public const int None = 0;   //	해당 없음
            public const int TargetIndex = 1;    //	특정 인덱스
            public const int TargetGroup = 2;    //	특정 그룹 #
            public const int IsBuff = 3; //	버프
            public const int IsDebuff = 4;   //	디버프
            public const int IsCC = 5;		//	CC
            public const int IsSystem = 6;		//	시스템
            public const int IsRelic = 7;		//	Relic
        }
        public static class PROJ_DESTINATION
        {
            public const int None = 0;   //	해당없음
            public const int TargetCenter = 1;   //	대상 중앙
            public const int TargetFloor = 2;    //	대상 바닥
            public const int TargetTeamCenter = 3;   //	팀 중앙
            public const int TargetTeamFloor = 4;		//	팀 중앙 바닥
        }
        public static class HIDE_CAM_DIRECTION
        {
            public const int None = 0;   //	해당없음
            public const int HideTargets = 1;   //	대상외 숨기기
            public const int HideAlly = 2;    //	대상외 아군 숨기기
            public const int HideEnemy = 3;   //	대상외 적군 숨기기 
        }
    }
    public static class SkillEffects
    {
        public static class EFFECT_TIMING
        {
            public const int None = 0;   //	해당없음
            public const int OnHit = 1;  //	hit키 마다 적용
            public const int OnStart = 2;    //	애니 시작 시 1회
            public const int OnEnd = 3;  //	야나 종료 시 1회
            public const int Instant = 4;		//	지정애니 없을 시 1회
            public const int StatusEffect = 100;
        }
        public static class EFFECT_TYPE
        {
            public const int None = 0;   //	해당없음
            public const int Damage = 1; //	대미지
            public const int Heal = 2;   //	힐
            public const int HealPercent = 3;    //	힐%
            public const int GainStatusEffect = 4;   //	상태이상 획득
            public const int LoseStatusEffect = 5;   //	상태이상 제거
            public const int InstantDeath = 6;   //	즉사
            public const int ResetTurn = 7;  //	턴 초기화
            public const int Transform = 8;  //	영웅 변신
            public const int CopyStatusEffect = 9;   //	상태이상 복사
            public const int DamagePercent = 10;    //	빈사
            public const int SpdGuage = 11;   //	행동력 게이지 증/감%
            public const int GainMana = 12;   //	마나 획득
            public const int OverhealShield = 13; //	힐 - 오버힐 실드 전환
            public const int CopyStatusEffectSelf = 14;		//	Fake 죽음
            public const int FxDummy = 15;//이펙트 출력용 타겟 
            public const int Vanish = 16;//영웅 숨기기  
            public const int EndVanish = 17;//영웅 숨기기 취소 
            public const int Revive = 18;//환생 

        }
        public static class TARGETABLE_RANGE
        {
            public const int None = 0;   //	해당없음
            public const int Self = 1;   //	스킬 시전자 본인
            public const int Allies = 2; //	아군 영웅
            public const int Enemies = 3;    //	적 영웅
            public const int All = 4;    //	모든 영웅
            public const int AllNoSelf = 5;  //	본인 제외 아군
            public const int Retaliate = 6;		//	반격 대상
            public const int SkillTarget = 7;   //스킬 대상   

        }
        public static class TARGETING_TYPE
        {
            public const int None = 0;   //	해당 없음
            public const int All = 1;    //	범위 모두
            public const int RandomOne = 2;  //	랜덤 1인
            public const int FullRandom = 3; //	랜덤 1인(매 히트별)
            public const int TargetOne = 4;  //	대상 1인
            public const int TargetBounce = 5;   //	대상 1인 튕기는 발사체
            public const int TargetContinuous = 6;   //	대상 1인 사망시 타겟 교체
            public const int RandomTwo = 7;  //	랜덤 2인
            public const int LowestHp = 8;		//	HP가 가장 낮은 1인
            public const int RandomTwoBounce = 9;  //	랜덤 2인 교차 대미지
            public const int HigherAtk = 10;         //  공격력이 높은 대상
            public const int AkumuGuard = 11;        //  아쿠무 호위 대상
            public const int HigherSpdGuage = 12;        //  행동력이 가장 높은 대상
            public const int LowestSpdGuage = 13;        //  행동력이 가장 낮은 대상
            public const int LastAction = 14;        //  직전 행동한 대상
            public const int LastAttackTarget = 15;        //  마지막에 피격당한 대상
        }
        public static class CONDITION_TARGET
        {
            public const int None = 0;   //	해당없음
            public const int Self = 1;   //	스킬 시전자 본인
            public const int Target = 2;		//	효과 시전 대상(들)
            public const int Allies = 3;    //모든 아군 영웅
            public const int Enemies = 4;   //모든 적 영웅
            public const int All = 5;   //모든 영웅
            public const int AllNoSelf = 6; //본인 제외 모든 아군 영웅
            public const int SkillTarget = 7;        //  아쿠무 호위 대상

        }
        public static class CONDITION_STATE
        {
            public const int None = 0;   //	해당없음
            public const int StatusEffectStackCount = 1; //	상태이상 스택 수
            public const int HpPercentCount = 2; //	HP%
            public const int SummonCostCount = 3;    //	소환 코스트
            public const int AlwaysOne = 4;  //	생존 턴수
            public const int ManaCount = 5;  //	내 마나
            public const int DeadAlliesCount = 6;    //	아군 사망 인원 수
            public const int DeadEnemiesCount = 7;   //	적군 사망 인원 수
            public const int AlliesCount = 8;    //	필드위 아군 수
            public const int EnemiesCount = 9;   //	필드위 적군 수
            public const int DiffHeroCount = 10;  //	필드위 아군 수 - 적군 수
            public const int StatusEffectCount = 11;		//	상태이상 수
            public const int SpdGuagePercent = 12;   //	행동력 %
            public const int IsMelee = 13;           //	TypeClass가 1,2 임
            public const int IsMagic = 14;           //	TypeClass가 3,4 임
            public const int StatusEffectDeleteCount = 15;   // 스킬효과로 제거한 상태이상 수
            public const int UseEnegyCardLv = 16;            // 사용된 에너지 카드 레벨
            public const int HpComUseEnegyCardLvpare = 17;   // 대상의 현재 체력-자신의 현재 체력
            public const int AtkCompare = 18;                // 대상의 공격력 - 자신의 공격력
            public const int MaxHpCompare = 19;              // 대상의 최대 체력 - 자신의 최대 체력
            public const int SpdCompare = 20;                // 대상의 속도-자신이 속도
        }
        public static class CON_STATUS_EFFECT_TYPE
        {
            public const int None = 0;              //	해당 없음
            public const int TargetIndex = 1;       //	특정 인덱스
            public const int TargetGroup = 2;       //	특정 그룹 #
            public const int IsBuff = 3;            //	버프
            public const int IsDebuff = 4;          //	디버프
            public const int IsCC = 5;              //	패시브 -> CC
            public const int IsSystem = 6;          //시스템
            public const int IsRelic = 7;           //	유물
        }
        public static class COMPARE_TYPE
        {
            public const int None = 0;   //	해당없음
            public const int Equal = 1;  //	동일한가
            public const int EqualMore = 2;  //	이상인가
            public const int EqualLess = 3;  //	이하인가
            public const int More = 4;   //	초과인가
            public const int Less = 5;      //	미만인가
            public const int Intervening = 6;        //  비교 값 사이인가

        }
        public static class BONUS_EFFECT_TYPE
        {
            public const int None = 0;   //	해당없음
            public const int CancelThisEffect = 1;   //	0일 시 효과 미발동
            public const int AmplifyValue = 2;   //	효과 값을 강화
            public const int AmplifyCrit = 3;    //	효과의 치명타 확률
            public const int AmplifyVamp = 4;    //	체력 흡수율
            public const int MakeSurehit = 5;		//	반드시 명중
            public const int AmplifyChance = 6;     //확률 증가
        }
        public static class STATUS_EFFECT_VALUE
        {
            public const int None = 0;   //	해당 없음
            public const int TargetIndex = 1;    //	특정 인덱스
            public const int TargetGroup = 2;    //	특정 그룹 #
            public const int IsBuff = 3; //	버프
            public const int IsDebuff = 4;   //	디버프
            public const int IsCC = 5;	 	//	CC
            public const int IsSystem = 6;      //	시스템
            public const int IsRelic = 7;		//	Relic

        }
    }
    public static class StatusEffects
    {
        public static class Category
        {
            public const int None = 0;  //	해당없음
            public const int Buff = 1;  //	버프
            public const int Debuff = 2;    //	디버프
            public const int CC = 3;   //	CC
            public const int System = 4;	//	시스템
            public const int Relic = 5;	//	유물
            public const int Aura = 6;	//	오라
            public const int Honor = 7;	//	Honor
        }
        public static class CountDownType
        {
            public const int None = 0;  //	해당없음
            public const int OnTurnStart = 1;   //	턴 시작
            public const int OnTurnEnd = 2;	//	턴 종료
        }
        public static class StatusEffectType
        {
            public const int None = 0;  //	None
            public const int ModifyStatus = 1;  //	ModifyStatus
            public const int DamageOverTime = 2;    //	DamageOverTime
            public const int HealOverTime = 3;  //	HealOverTime
            public const int HealOverTimePercent = 4;   //	HealOverTimePercent
            public const int Shield = 5;    //	Shield
            public const int ShieldPercent = 6; //	ShieldPercent
            public const int ManaGain = 7;  //	ManaGain
            public const int Reflect = 8;   //	Reflect
            public const int Vamp = 9;  //	Vamp
            public const int Revive = 10;   //	Revive
            public const int Blined = 11;   //	Blined
            public const int ModifyDamage = 12; //	ModifyDamage
            public const int DamagePercent = 13;  //	DamagePercent
            public const int VampAll = 14;  //	VampAll
            public const int Bomb = 15; //	Bomb
            public const int Stun = 16; //	Stun
            public const int Freeze = 17;   //	Freeze
            public const int Petrify = 18;  //	Petrify
            public const int Sleep = 19;    //	Sleep
            public const int Silence = 20;  //	Silence
            public const int Oblivion = 21; //	Oblivion
            public const int Invincible = 22;   //	Invincible
            public const int Bleed = 23;    //	Bleed
            public const int Surehit = 24;  //	Surehit
            public const int Immortal = 25; //	Immortal
            public const int Immune = 26;   //	Immune
            public const int Curse = 27;    //	Curse
            public const int Guard = 28;    //	Guard
            public const int Retaliate = 29;    //	Retaliate
            public const int Force = 30;    //	Force
            public const int Taunt = 31;    //	Taunt
            public const int Summon = 32;   //	Summon
            public const int ShieldOverheal = 33;   //	ShieldOverheal
            public const int ModifyRandomSkill = 34;    //	ModifyRandomSkill
            public const int DeathSentence = 35;	//	DeathSentence
            public const int DirectHit = 36; //직격 
            public const int ModifySkillMana = 37;  //마나코스트변경 
            public const int DragonRage = 38;	//	용의분노(록산느)   
            public const int TimeBack = 39;         //시간 역행
            public const int TimeAttack = 40;       //타임어택
            public const int ReceivingAssist = 41;  //공격 지원 받음
            public const int FireMode = 42;         //팔크 파이어 모드
            public const int DamageLimit = 43;      //	DamageLimit
            public const int CritBan = 44;          //	CritBan
            public const int CritInImmune = 45;     //	CritInImmune
            public const int DebuffImmune = 46;     //	DebuffImmune
            public const int AniImmune = 47;        //  AirUp, Knockdown Immune
            public const int AlliesRevive = 48;     //  Allies Revive
        }
        public static class StatusApplyType
        {
            public const int None = 0;  //	해당없음
            public const int Abs = 1;   //	절대값
            public const int Percent = 2;   //	퍼센트
            public const int LossHpPercent = 3;	//	잃은 체력 비례
        }
        public static class ConditonType
        {
            public const int None = 0;               //	해당없음
            public const int TarkgetOneDamage = 1;   //	단일 대미지 피격
            public const int SkillDamage = 2;        //	대미지 피격
            public const int AttackSuccess = 3;      //  공격 성공
            public const int DodgeSuccess = 4;       //  회피 성공
            public const int DebuffImmune = 5;       //  상태이상 저항
            public const int BlockSuccess = 6;       //  공격 블록
            public const int CritlSuccess = 7;       //  극대회 공격
            public const int CritlSkillDamage = 8;   //  극대화 피격
        }
        public static class AuraRange
        {
            public const int None = 0;  //	해당없음
            public const int AllAllies = 1; //	아군 전체
            public const int AllEnemeies = 2;   //	적군 전체
            public const int All = 3;	//	모든 영웅
        }
        public static class StatusEffectIcon
        {
            public const int None = (int)ENUM_STATUS_EFFECT_ICON.None;
            public const int atkup = (int)ENUM_STATUS_EFFECT_ICON.atkup;
            public const int spdup = (int)ENUM_STATUS_EFFECT_ICON.spdup;
            public const int defup = (int)ENUM_STATUS_EFFECT_ICON.defup;
            public const int immune = (int)ENUM_STATUS_EFFECT_ICON.immune;
            public const int reflect = (int)ENUM_STATUS_EFFECT_ICON.reflect;
            public const int critup = (int)ENUM_STATUS_EFFECT_ICON.critup;
            public const int hpregen = (int)ENUM_STATUS_EFFECT_ICON.hpregen;
            public const int retaliate = (int)ENUM_STATUS_EFFECT_ICON.retaliate;
            public const int invincible = (int)ENUM_STATUS_EFFECT_ICON.invincible;
            public const int blockup = (int)ENUM_STATUS_EFFECT_ICON.blockup;
            public const int dodgeup = (int)ENUM_STATUS_EFFECT_ICON.dodgeup;
            public const int immortal = (int)ENUM_STATUS_EFFECT_ICON.immortal;
            public const int guard = (int)ENUM_STATUS_EFFECT_ICON.guard;
            public const int shield = (int)ENUM_STATUS_EFFECT_ICON.shield;
            public const int revive = (int)ENUM_STATUS_EFFECT_ICON.revive;
            public const int force = (int)ENUM_STATUS_EFFECT_ICON.force;
            public const int vamp = (int)ENUM_STATUS_EFFECT_ICON.vamp;
            public const int surehit = (int)ENUM_STATUS_EFFECT_ICON.surehit;
            public const int ironwall = (int)ENUM_STATUS_EFFECT_ICON.ironwall;
            public const int directhit = (int)ENUM_STATUS_EFFECT_ICON.directhit;
            public const int defdown = (int)ENUM_STATUS_EFFECT_ICON.defdown;
            public const int atkdown = (int)ENUM_STATUS_EFFECT_ICON.atkdown;
            public const int critdown = (int)ENUM_STATUS_EFFECT_ICON.critdown;
            public const int dot = (int)ENUM_STATUS_EFFECT_ICON.dot;
            public const int spddown = (int)ENUM_STATUS_EFFECT_ICON.spddown;
            public const int blind = (int)ENUM_STATUS_EFFECT_ICON.blind;
            public const int mark = (int)ENUM_STATUS_EFFECT_ICON.mark;
            public const int sleep = (int)ENUM_STATUS_EFFECT_ICON.sleep;
            public const int taunt = (int)ENUM_STATUS_EFFECT_ICON.taunt;
            public const int silence = (int)ENUM_STATUS_EFFECT_ICON.silence;
            public const int stun = (int)ENUM_STATUS_EFFECT_ICON.stun;
            public const int freeze = (int)ENUM_STATUS_EFFECT_ICON.freeze;
            public const int bleed = (int)ENUM_STATUS_EFFECT_ICON.bleed;
            public const int curse = (int)ENUM_STATUS_EFFECT_ICON.curse;
            public const int delusion = (int)ENUM_STATUS_EFFECT_ICON.delusion;
            public const int petrify = (int)ENUM_STATUS_EFFECT_ICON.petrify;
            public const int special = (int)ENUM_STATUS_EFFECT_ICON.special;
            public const int bomb = (int)ENUM_STATUS_EFFECT_ICON.bomb;
            public const int spatkup = (int)ENUM_STATUS_EFFECT_ICON.spatkup;
            public const int spdefup = (int)ENUM_STATUS_EFFECT_ICON.spdefup;
            public const int spspdup = (int)ENUM_STATUS_EFFECT_ICON.spspdup;
            public const int spspddown = (int)ENUM_STATUS_EFFECT_ICON.spspddown;
            public const int spdodgeup = (int)ENUM_STATUS_EFFECT_ICON.spdodgeup;
            public const int auradefup = (int)ENUM_STATUS_EFFECT_ICON.auradefup;
        }

        public enum ENUM_STATUS_EFFECT_ICON
        {
            None = 0,
            atkup,
            spdup,
            defup,
            immune,
            reflect,
            critup,
            hpregen,
            retaliate,
            invincible,
            blockup,
            dodgeup,
            immortal,
            guard,
            shield,
            revive,
            force,
            vamp,
            surehit,
            ironwall,
            directhit,
            defdown,
            atkdown,
            critdown,
            dot,
            spddown,
            blind,
            mark,
            sleep,
            taunt,
            silence,
            stun,
            freeze,
            bleed,
            curse,
            delusion,
            petrify,
            special,
            bomb,
            spatkup,
            spdefup,
            spspdup,
            spspddown,
            spdodgeup,
            auradefup
        }

        public enum ENUM_STATUS_EFFECT_FX
        {
            None = 0,
            atkup,
            spdup,
            defup,
            immune,
            reflect,
            critup,
            hpregen,
            retaliate,
            invincible,
            invincible2,
            invincible3,
            blockup,
            dodgeup,
            immortal,
            guard,
            shield,
            shield2,
            shield3,
            revive,
            force,
            vamp,
            surehit,
            ironwall,
            directhit,
            defdown,
            atkdown,
            critdown,
            dot,
            spddown,
            blind,
            mark,
            sleep2,
            taunt,
            silence,
            stun2,
            freeze,
            freeze2,
            bleed,
            curse,
            delusion,
            petrify,
            petrify2,
            durax,
            durax2,
            akrak3,
            ez052,
            gozas2,
            sneezy2,
            sneezy3,
            mirnoff,
            demias3,
            zafrina,
            viper3,
            blockdown,
            dodgedown,
            aide2,
            aide3,
            kage3,
            freeze3,
        }
    }
    public static class Resource
    {
        public static class Item
        {
            public const int None = 0;
            public const int CurRuby = 101;
            public const int CurGold = 102;
            public const int CurEssence = 103;
            public const int CurGuild = 104;
            public const int PointChampion = 105;
            public const int ArenaScore = 106;
            public const int UserLvExp = 107;
            public const int SpecialQuest0001 = 201;
            public const int BoxWood = 301;
            public const int BoxHuge = 302;
            public const int BoxLuxury = 303;
            public const int BoxIron = 311;
            public const int BoxBronze = 312;
            public const int BoxSilver = 313;
            public const int BoxGold = 314;
            public const int BoxPlatinum = 315;
            public const int BoxDiamond = 316;
            public const int BoxMaster = 317;
            public const int BoxGrandMaster = 318;
            public const int BoxChampions = 319;
            public const int PackTraining = 801;
            public const int PackIron = 802;
            public const int PackBronze = 803;
            public const int PackSilver = 804;
            public const int PackGold = 805;
            public const int PackPlatinum = 806;
            public const int PackDiamond = 807;
            public const int PackMaster = 808;
            public const int PackGrandMaster = 809;
            public const int PackChampions = 810;
            public const int FixedHero = 401;
            public const int ChoiceHero = 402;
            public const int RandomHero = 403;
            public const int GuildRandomHero = 404;
            public const int FixedSkin = 501;
            public const int ChoiceSkin = 502;
            public const int RandomSkin = 503;
        }
    }
    public static class BattleContents
    {
        public static class BattleType
        {
            public const int None = 0;
            public const int RankArena = 901;
            public const int NormalArena = 902;
            public const int Campaign = 911;
            public const int Tutorial = 912;
            public const int WeeklyBoss = 913;
            public const int ColossusBoss = 914;
            public const int GoldPillage = 921;
            public const int OutlandBoss = 922;
            public const int GuildWar = 931;
            public const int GuildBoss = 932;
        }
    }

    public static class ItemType
    {
        public const int None = 0;          //	해당없음
        public const int Token = 1;         //	토큰
        public const int Money = 2;         //	재화
        public const int Point = 3;         //	재화(포인트)
        public const int Ticket = 4;        //	재화(티켓)
        public const int Box = 5;           //	상자
        public const int Openbox = 6;	    //	개봉형 상자
        public const int SelectMet = 7;	    //	선택형 아이템
        public const int Used = 8;	        //	사용형 아이템(일반)
        public const int Usedidle = 9;	    //	아이들 보상 즉시 지급형
        public const int Meterial = 10;	    //	재료
        public const int Special = 11;	    //	특수 아이템
        public const int Battlepass = 12;	//	배틀패스
        public const int Contents = 13;	    //	컨텐츠형 아이템
    }

    public static class Dev
    {
        public enum ENUM_TARGET_UI
        {
            Ally = 0,
            Enemy
        }
        public enum ENUM_TARGET_VIEW
        {
            None = 0,
            Single,
            All,
            Random_1,
            Random_2,
            Random_Unknown
        }
    }

    public static class Equip
    {
        public static class EquipPart
        {
            public const int None = 0;          // 해당없음
            public const int Weapon = 1;        // 무기
            public const int Body = 2;          // 몸
            public const int Hands = 3;         // 손
            public const int Foot = 4;          // 발
            public const int Accessory = 5;     // 액세서리
            public const int Aura = 6;          // 오라
            public const int Pet = 7;          // 오라
        }
    }
}
