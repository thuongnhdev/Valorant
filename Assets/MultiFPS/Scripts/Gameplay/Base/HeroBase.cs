using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// 영웅 분해 State
public enum GachaDecomposeState
{
    NONE, // 분해 가능
    UPGRADE_RAR, //승급한 영웅
    LOCK, //잠긴 영웅
    USED, //활동중인 영웅
    LEVEL_LINK, // 레벨링크 사용
    IS_NFT, //NFT 영웅
    IS_NO_DISMANTLE, //분해불가 영웅
    IS_NFT_READY, //NFT READY 영웅
    NEXUS, // 넥서스 등록중
}

//영웅 리셋 State
public enum GachaResetState
{
    NONE, // 초기화 가능
    NOT_UPGRADE_RAR, //승급한 안한 영웅
    LOCK, //잠긴 영웅
    USED, //활동중인 영웅
    LEVEL_LINK, // 레벨링크 사용
    IS_NFT_READY, //NFT READY 영웅
    IS_NFT, //NFT 영웅
    NEXUS, // 넥서스 등록중
}

// 영웅 레벨교환 State
public enum GachaLevelExchangeState
{
    NONE, // 레벨교환 가능
    LOCK, //잠긴 영웅
    USED, //활동중인 영웅
    LEVEL_LINK, // 레벨링크 사용
    IS_NFT_READY, //NFT READY 영웅
    IS_NFT, //NFT 영웅
    NEXUS, // 넥서스 등록중
}

public enum IDSkill
{
    ShroudedStep = 1,
    Paranoia = 2,
    DarkCover = 3,
    FromTheShadows = 4,
    BoomBot = 5,
    BlastPack = 6,
    PaintShell = 7,
    Drift = 8,
    CloudBurst = 9,
    Updraft = 10,
    TailWind = 11,
    BladeStorm = 12
}

// 영웅 민팅스크롤 State
public enum GachaMintingState
{
    NONE, // 민팅 가능
    LOCK, //잠긴 영웅
    USED, //활동중인 영웅    
    LEVEL_LINK, // 레벨링크 사용
    NEXUS, // 넥서스 등록중
}

// 영웅 건축파견 State
public enum CastleBuildingState
{
    NONE, // 파견 가능
    USED //활동중인 영웅
}

public enum MintingState
{
    NONE, // NFT 아닌상태.
    READY, // NFT Ready 상태.
    NFT, // NFT 상태.
}

//User가 가지고 있는 영웅의 기본정보. raw data을 포함.
public class HeroBase
{

    public int slotIdx;
    public ProtoHero PROTO_HERO;

    public long UID;   //UserId
    public long HID;   //영웅 고유 인덱스_사전데이터 인덱스와 다름
    public bool IsHold;
    public int Remaining;
    public int RentStatus;
    //서버에 저장 된 내용
    private int LEVEL = 1;
    public int level
    {
        get
        {
            return LEVEL;
        }
        set
        {
            LEVEL = value;
            UpdateEquipment();
        }
    }

    private int RARITY;
    public int rarity
    {
        get
        {
            return RARITY;
        }
        set
        {
            RARITY = value;
            UpdateEquipment();
        }
    }

    public int percept;
    public int craft;

    public bool IsLock = false;

    public bool IsNoDismantle = false;

    public DateTime GetDate;
    public int GetRemainingTime;

    private bool IS_BOSS = false;

    private long[] EQUIP_LIST = new long[7];

    private Dictionary<int, int> EquipSetDic = new Dictionary<int, int>();

    //영웅 정보_VIEW
    private HeroStats EquipStatInfo = new HeroStats();

    public bool bNew = false;

    public HeroBase(long uid, HeroInfo heroInfo, bool bGet = false)
    {
        UID = uid;
        HID = heroInfo.Hid;

        LEVEL = heroInfo.ChaHeroLv;
        RARITY = heroInfo.ChaRarity;

        EQUIP_LIST[0] = heroInfo.Equip1;
        EQUIP_LIST[1] = heroInfo.Equip2;
        EQUIP_LIST[2] = heroInfo.Equip3;
        EQUIP_LIST[3] = heroInfo.Equip4;
        EQUIP_LIST[4] = heroInfo.Equip5;
        EQUIP_LIST[5] = heroInfo.Equip6;
        EQUIP_LIST[6] = heroInfo.Equip7;

        percept = heroInfo.Percept;
        craft = heroInfo.Craft;

        IsLock = Convert.ToBoolean(heroInfo.IsLock);
        IsNoDismantle = Convert.ToBoolean(heroInfo.IsNoDismantle);


        PROTO_HERO = ClientDataTable.Instance.DIC_HERO[heroInfo.ChaHero];


        UpdateEquipment();
    }

    public HeroBase(HeroBase heroBase)
    {
        UID = heroBase.UID;
        HID = heroBase.HID;

        LEVEL = heroBase.LEVEL;
        RARITY = heroBase.RARITY;

        Array.Copy(heroBase.EQUIP_LIST, EQUIP_LIST, EQUIP_LIST.Length);

        percept = heroBase.percept;
        craft = heroBase.craft;

        IsLock = heroBase.IsLock;
        IsNoDismantle = heroBase.IsNoDismantle;

        GetDate = heroBase.GetDate;

        PROTO_HERO = heroBase.PROTO_HERO;

        UpdateEquipment();
    }

    public HeroBase(ProtoHero hero, int cnt)
    {
        UID = cnt;
        HID = -1;

        LEVEL = 1;

        EQUIP_LIST[0] = 0;
        EQUIP_LIST[1] = 0;
        EQUIP_LIST[2] = 0;
        EQUIP_LIST[3] = 0;
        EQUIP_LIST[4] = 0;
        EQUIP_LIST[5] = 0;
        EQUIP_LIST[6] = 0;
        PROTO_HERO = hero;

        UpdateEquipment();
    }

    public void RefreshHero(HeroInfo heroInfo)
    {
        if (HID != heroInfo.Hid)
        {
            return;
        }

        LEVEL = heroInfo.ChaHeroLv;
        RARITY = heroInfo.ChaRarity;

        EQUIP_LIST[0] = heroInfo.Equip1;
        EQUIP_LIST[1] = heroInfo.Equip2;
        EQUIP_LIST[2] = heroInfo.Equip3;
        EQUIP_LIST[3] = heroInfo.Equip4;
        EQUIP_LIST[4] = heroInfo.Equip5;
        EQUIP_LIST[5] = heroInfo.Equip6;
        EQUIP_LIST[6] = heroInfo.Equip7;

        percept = heroInfo.Percept;
        craft = heroInfo.Craft;

        IsLock = Convert.ToBoolean(heroInfo.IsLock);
        IsNoDismantle = Convert.ToBoolean(heroInfo.IsNoDismantle);

        UpdateEquipment();
    }

    public ProtoHero GetProtoHero()
    {
        return PROTO_HERO;
    }

    public int GetDicHeroIndex()
    {
        return PROTO_HERO.DataHero.Idx;
    }

    public int GetDicHeroAssetIndex()
    {
        return PROTO_HERO.DataHero.AssetIndex;
    }

    public long[] GetEquipList()
    {
        return EQUIP_LIST;
    }

    public void RefeshEquipList(long[] equipList)
    {
        for (int i = 0; i < EQUIP_LIST.Length; i++)
        {
            EQUIP_LIST[i] = equipList[i];
        }
    }

    public void ClearEquipList()
    {
        for (int i = 0; i < EQUIP_LIST.Length; i++)
        {
            EQUIP_LIST[i] = 0;
        }
    }

    public void RemoveEquip(int idx)
    {
        EQUIP_LIST[idx] = 0;
    }

    public bool IsBoss()
    {
        return IS_BOSS;
    }

    public void UpdateEquipment()
    {
    
    }

    public Dictionary<int, int> GetEquipSetDic()
    {
        return EquipSetDic;
    }

    public int CheckEquipSetCnt(int index)
    {
        if (EquipSetDic.ContainsKey(index))
        {
            return EquipSetDic[index];
        }
        else
        {
            return 0;
        }
    }

    void SetStatOption(int TypeStatus, int TypePercent, float fValue)
    {
        HeroStats stats = EquipStatInfo;

        switch (TypeStatus)
        {
            case Code.Character.TYPE_STATUS.Atk:
                stats.STAT_INFO_ATK += fValue;
                break;
            case Code.Character.TYPE_STATUS.Hp:
                stats.STAT_INFO_HP += fValue;
                break;
            case Code.Character.TYPE_STATUS.Def:
                stats.STAT_INFO_DEF += fValue;
                break;
            case Code.Character.TYPE_STATUS.Crit:
                stats.STAT_INFO_CRT += fValue;
                break;
            case Code.Character.TYPE_STATUS.CritPower:
                stats.STAT_INFO_CRT_POW += fValue;
                break;
            case Code.Character.TYPE_STATUS.Spd:
                stats.STAT_INFO_SPEED += fValue;
                break;
            case Code.Character.TYPE_STATUS.Block:
                stats.STAT_INFO_BLOCK += fValue;
                break;
            case Code.Character.TYPE_STATUS.Dodge:
                stats.STAT_INFO_DODGE += fValue;
                break;
            case Code.Character.TYPE_STATUS.Accuracy:
                stats.STAT_INFO_ACC += fValue;
                break;
            case Code.Character.TYPE_STATUS.EffectRes:
                stats.STAT_INFO_EFF_RES += fValue;
                break;
            case Code.Character.TYPE_STATUS.EffectAcc:
                stats.STAT_INFO_EFF_ACC += fValue;
                break;
            case Code.Character.TYPE_STATUS.Pierce:
                stats.STAT_INFO_PIERCE += fValue;
                break;
            case Code.Character.TYPE_STATUS.Reflect:
                stats.STAT_INFO_REFLECT += fValue;
                break;
            case Code.Character.TYPE_STATUS.EnhanceDamage:
                stats.STAT_INFO_ENH_DMG += fValue;
                break;
            case Code.Character.TYPE_STATUS.AmplifyDamage:
                stats.STAT_INFO_AMP_DMG += fValue;
                break;
            case Code.Character.TYPE_STATUS.Vamp:
                stats.STAT_INFO_VAMP += fValue;
                break;
            case Code.Character.TYPE_STATUS.HealOverTime:
                stats.STAT_INFO_HEAL_OT += fValue;
                break;
            case Code.Character.TYPE_STATUS.Percept:
                stats.STAT_INFO_PERCEPT += fValue;
                break;
            case Code.Character.TYPE_STATUS.Craft:
                stats.STAT_INFO_CRAFT += fValue;
                break;
        }
    }

}
