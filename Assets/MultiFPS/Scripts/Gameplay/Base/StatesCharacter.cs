using System;
using System.Collections.Generic;

public static class StatesCharacter
{
    public static Dictionary<long, HeroBase> HERO_LIST = new Dictionary<long, HeroBase>();

    public const int BATTLE_DECK_HERO_COUNT = 10;

    public static long[] BATTLESETTING_HERO = new long[BATTLE_DECK_HERO_COUNT] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

    public static List<HeroBase> BATTLE_HERO = new List<HeroBase>();

    public static int[] UnlockDicHeroIndexList;

    public static void InitData()
    {

    }
    public static HeroBase GetMyHero(long hid)
    {
        HeroBase retHero = null;

        if (HERO_LIST.ContainsKey(hid))
        {
            retHero = HERO_LIST[hid];
        }

        return retHero;
    }


    public static HeroBase GetMyHero(int dicIndex)
    {
        HeroBase retHero = null;

        foreach (HeroBase hero in HERO_LIST.Values)
        {
            if (hero.GetDicHeroAssetIndex() == dicIndex)
            {
                retHero = hero;
                break;
            }
        }

        return retHero;
    }

    //같은 인덱스의 영웅들 가져오기.
    public static List<HeroBase> GetHero(int dicIndex, bool bMaxGrade = false)
    {
        List<HeroBase> baseList = new List<HeroBase>();

        return baseList;
    }

    public static ProtoHero GetDicDataHero(int dicIndex)
    {
        return ClientDataTable.Instance.DIC_HERO[dicIndex];
    }

    public static HeroBase GetHero_InBattle(long uid, int slotIndex)
    {
        HeroBase retHero = null;

        for (int i = 0; i < BATTLE_HERO.Count; i++)
        {
            if (BATTLE_HERO[i].UID == uid && BATTLE_HERO[i].slotIdx == slotIndex)
            {
                retHero = BATTLE_HERO[i];
                break;
            }
        }
        return retHero;
    }

}
