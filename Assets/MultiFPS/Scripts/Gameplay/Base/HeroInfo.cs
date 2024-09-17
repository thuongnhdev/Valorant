using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroInfo 
{
    public long Hid;
    public int ChaHero;
    public int ChaHeroLv;
    public int ChaRarity;
    public long Equip1;
    public long Equip2;
    public long Equip3;
    public long Equip4;
    public long Equip5;
    public long Equip6;
    public long Equip7;
    public int IsLock;
    public int IsNoDismantle;
    public int IsNftReady;
    public int Percept;
    public int Craft;
    public string GyriTokenKey;
    public int OriginalLevel;
    public long LevelLinkHid;
    public int GalaCoolTime;
    public long GetDate;

    public HeroInfo(HeroInfo info)
    {
        this.Hid = info.Hid;
        this.ChaHero = info.ChaHero;
        this.ChaHeroLv = info.ChaHeroLv;
        this.ChaRarity = info.ChaRarity;
        this.Equip1 = info.Equip1;
        this.Equip2 = info.Equip2;
        this.Equip3 = info.Equip3;
        this.Equip4 = info.Equip4;
        this.Equip5 = info.Equip5;
        this.Equip6 = info.Equip6;
        this.Equip7 = info.Equip7;
        this.IsLock = info.IsLock;
        this.IsNoDismantle = info.IsNoDismantle;
        this.IsNftReady = info.IsNftReady;
        this.Percept = info.Percept;
        this.Craft = info.Craft;
        this.GyriTokenKey = info.GyriTokenKey;
        this.OriginalLevel = info.OriginalLevel;
        this.LevelLinkHid = info.LevelLinkHid;
        this.GalaCoolTime = info.GalaCoolTime;
        this.GetDate = info.GetDate;
    }
}
