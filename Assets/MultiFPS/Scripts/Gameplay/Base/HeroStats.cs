[System.Serializable]
public class HeroStats
{
    public float STAT_INFO_HP = 0f;        
    public float STAT_INFO_ATK = 0f;       
    public float STAT_INFO_DEF = 0f;       
    public float STAT_INFO_SPEED = 0f;    
    public float STAT_INFO_CRT = 0f;      
    public float STAT_INFO_CRT_POW = 0f;    
    public float STAT_INFO_BLOCK = 0f;       
    public float STAT_INFO_DODGE = 0f;     
    public float STAT_INFO_ACC = 0f;      
    public float STAT_INFO_EFF_RES = 0f;    
    public float STAT_INFO_EFF_ACC = 0f; 
    public float STAT_INFO_PIERCE = 0f;  
    public float STAT_INFO_REFLECT = 0f;   
    public float STAT_INFO_ENH_DMG = 0f;    
    public float STAT_INFO_AMP_DMG = 0f;   
    public float STAT_INFO_VAMP = 0f;     
    public float STAT_INFO_HEAL_OT = 0f; 
    public float STAT_INFO_PERCEPT = 0f;
    public float STAT_INFO_CRAFT = 0f;  

    public void Clear()
    {
        STAT_INFO_HP = 0f;
        STAT_INFO_ATK = 0f;
        STAT_INFO_DEF = 0f;
        STAT_INFO_SPEED = 0f;
        STAT_INFO_CRT = 0f;
        STAT_INFO_CRT_POW = 0f;
        STAT_INFO_BLOCK = 0f;
        STAT_INFO_DODGE = 0f;
        STAT_INFO_ACC = 0f;
        STAT_INFO_EFF_RES = 0f;
        STAT_INFO_EFF_ACC = 0f;
        STAT_INFO_PIERCE = 0f;
        STAT_INFO_REFLECT = 0f;
        STAT_INFO_ENH_DMG = 0f;
        STAT_INFO_AMP_DMG = 0f;
        STAT_INFO_VAMP = 0f;
        STAT_INFO_HEAL_OT = 0f;
        STAT_INFO_PERCEPT = 0f;
        STAT_INFO_CRAFT = 0f;
    }

    public static HeroStats operator +(HeroStats hs1, HeroStats hs2)
    {
        HeroStats hs = new HeroStats();

        hs.STAT_INFO_HP = hs1.STAT_INFO_HP + hs2.STAT_INFO_HP;
        hs.STAT_INFO_ATK = hs1.STAT_INFO_ATK + hs2.STAT_INFO_ATK;
        hs.STAT_INFO_DEF = hs1.STAT_INFO_DEF + hs2.STAT_INFO_DEF;
        hs.STAT_INFO_SPEED = hs1.STAT_INFO_SPEED + hs2.STAT_INFO_SPEED;
        hs.STAT_INFO_CRT = hs1.STAT_INFO_CRT + hs2.STAT_INFO_CRT;
        hs.STAT_INFO_CRT_POW = hs1.STAT_INFO_CRT_POW + hs2.STAT_INFO_CRT_POW;
        hs.STAT_INFO_BLOCK = hs1.STAT_INFO_BLOCK + hs2.STAT_INFO_BLOCK;
        hs.STAT_INFO_DODGE = hs1.STAT_INFO_DODGE + hs2.STAT_INFO_DODGE;
        hs.STAT_INFO_ACC = hs1.STAT_INFO_ACC + hs2.STAT_INFO_ACC;
        hs.STAT_INFO_EFF_RES = hs1.STAT_INFO_EFF_RES + hs2.STAT_INFO_EFF_RES;
        hs.STAT_INFO_EFF_ACC = hs1.STAT_INFO_EFF_ACC + hs2.STAT_INFO_EFF_ACC;
        hs.STAT_INFO_PIERCE = hs1.STAT_INFO_PIERCE + hs2.STAT_INFO_PIERCE;
        hs.STAT_INFO_REFLECT = hs1.STAT_INFO_REFLECT + hs2.STAT_INFO_REFLECT;
        hs.STAT_INFO_ENH_DMG = hs1.STAT_INFO_ENH_DMG + hs2.STAT_INFO_ENH_DMG;
        hs.STAT_INFO_AMP_DMG = hs1.STAT_INFO_AMP_DMG + hs2.STAT_INFO_AMP_DMG;
        hs.STAT_INFO_VAMP = hs1.STAT_INFO_VAMP + hs2.STAT_INFO_VAMP;
        hs.STAT_INFO_HEAL_OT = hs1.STAT_INFO_HEAL_OT + hs2.STAT_INFO_HEAL_OT;
        hs.STAT_INFO_PERCEPT = hs1.STAT_INFO_PERCEPT + hs2.STAT_INFO_PERCEPT;
        hs.STAT_INFO_CRAFT = hs1.STAT_INFO_CRAFT + hs2.STAT_INFO_CRAFT;

        return hs;
    }

    public static HeroStats operator -(HeroStats hs1, HeroStats hs2)
    {
        HeroStats hs = new HeroStats();

        hs.STAT_INFO_HP = hs1.STAT_INFO_HP - hs2.STAT_INFO_HP;
        hs.STAT_INFO_ATK = hs1.STAT_INFO_ATK - hs2.STAT_INFO_ATK;
        hs.STAT_INFO_DEF = hs1.STAT_INFO_DEF - hs2.STAT_INFO_DEF;
        hs.STAT_INFO_SPEED = hs1.STAT_INFO_SPEED - hs2.STAT_INFO_SPEED;
        hs.STAT_INFO_CRT = hs1.STAT_INFO_CRT - hs2.STAT_INFO_CRT;
        hs.STAT_INFO_CRT_POW = hs1.STAT_INFO_CRT_POW - hs2.STAT_INFO_CRT_POW;
        hs.STAT_INFO_BLOCK = hs1.STAT_INFO_BLOCK - hs2.STAT_INFO_BLOCK;
        hs.STAT_INFO_DODGE = hs1.STAT_INFO_DODGE - hs2.STAT_INFO_DODGE;
        hs.STAT_INFO_ACC = hs1.STAT_INFO_ACC - hs2.STAT_INFO_ACC;
        hs.STAT_INFO_EFF_RES = hs1.STAT_INFO_EFF_RES - hs2.STAT_INFO_EFF_RES;
        hs.STAT_INFO_EFF_ACC = hs1.STAT_INFO_EFF_ACC - hs2.STAT_INFO_EFF_ACC;
        hs.STAT_INFO_PIERCE = hs1.STAT_INFO_PIERCE - hs2.STAT_INFO_PIERCE;
        hs.STAT_INFO_REFLECT = hs1.STAT_INFO_REFLECT - hs2.STAT_INFO_REFLECT;
        hs.STAT_INFO_ENH_DMG = hs1.STAT_INFO_ENH_DMG - hs2.STAT_INFO_ENH_DMG;
        hs.STAT_INFO_AMP_DMG = hs1.STAT_INFO_AMP_DMG - hs2.STAT_INFO_AMP_DMG;
        hs.STAT_INFO_VAMP = hs1.STAT_INFO_VAMP - hs2.STAT_INFO_VAMP;
        hs.STAT_INFO_HEAL_OT = hs1.STAT_INFO_HEAL_OT - hs2.STAT_INFO_HEAL_OT;
        hs.STAT_INFO_PERCEPT = hs1.STAT_INFO_PERCEPT - hs2.STAT_INFO_PERCEPT;
        hs.STAT_INFO_CRAFT = hs1.STAT_INFO_CRAFT - hs2.STAT_INFO_CRAFT;

        return hs;
    }

    public float GetStatInfo(int type)
    {
        float retStat = 0;
        switch (type)
        {
            case Code.Character.TYPE_STATUS.Atk:
                retStat = STAT_INFO_ATK;
                break;
            case Code.Character.TYPE_STATUS.Hp:
                retStat = STAT_INFO_HP;
                break;
            case Code.Character.TYPE_STATUS.Def:
                retStat = STAT_INFO_DEF;
                break;
            case Code.Character.TYPE_STATUS.Crit:
                retStat = STAT_INFO_CRT;
                break;
            case Code.Character.TYPE_STATUS.CritPower:
                retStat = STAT_INFO_CRT_POW;
                break;
            case Code.Character.TYPE_STATUS.Spd:
                retStat = STAT_INFO_SPEED;
                break;
            case Code.Character.TYPE_STATUS.Block:
                retStat = STAT_INFO_BLOCK;
                break;
            case Code.Character.TYPE_STATUS.Dodge:
                retStat = STAT_INFO_DODGE;
                break;
            case Code.Character.TYPE_STATUS.Accuracy:
                retStat = STAT_INFO_ACC;
                break;
            case Code.Character.TYPE_STATUS.EffectRes:
                retStat = STAT_INFO_EFF_RES;
                break;
            case Code.Character.TYPE_STATUS.EffectAcc:
                retStat = STAT_INFO_EFF_ACC;
                break;
            case Code.Character.TYPE_STATUS.Pierce:
                retStat = STAT_INFO_PIERCE;
                break;
            case Code.Character.TYPE_STATUS.Reflect:
                retStat = STAT_INFO_REFLECT;
                break;
            case Code.Character.TYPE_STATUS.EnhanceDamage:
                retStat = STAT_INFO_ENH_DMG;
                break;
            case Code.Character.TYPE_STATUS.AmplifyDamage:
                retStat = STAT_INFO_AMP_DMG;
                break;
            case Code.Character.TYPE_STATUS.Vamp:
                retStat = STAT_INFO_VAMP;
                break;
            case Code.Character.TYPE_STATUS.HealOverTime:
                retStat = STAT_INFO_HEAL_OT;
                break;
            case Code.Character.TYPE_STATUS.Percept:
                retStat = STAT_INFO_PERCEPT;
                break;
            case Code.Character.TYPE_STATUS.Craft:
                retStat = STAT_INFO_CRAFT;
                break;
        }

        return retStat;
    }
}
