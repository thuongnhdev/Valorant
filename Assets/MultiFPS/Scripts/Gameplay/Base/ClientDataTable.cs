using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class EquipEnchant
{
    public float SuccRate = 0f;
    public float PenaltyRate = 0f;
    public float OptRate = 0f;
    public int[] MatIndex = new int[3];
    public int[] MatCount = new int[3];
}

[System.Serializable]
public class ProtoHero
{
    public Client_DataHero.Param DataHero;

    public Client_DataSkills.Param Skill1;
    public Client_DataSkills.Param Skill2;
    public Client_DataSkills.Param Skill3;
    public Client_DataSkills.Param EnhSkill1;
    public Client_DataSkills.Param EnhSkill2;
    public Client_DataSkills.Param EnhSkill3;
    public Client_DataSkills.Param TriSkill1;
    public Client_DataSkills.Param TriSkill2;
    public Client_DataSkills.Param TriSkill3;
    public Client_DataSkills.Param TriSkill4;
    public Client_DataSkills.Param TriSkill5;
    public Client_DataSkills.Param SumSkill;
    public Client_DataSkills.Param RetSkill;
    public Client_DataSkills.Param DeaSkill;

    public Client_DataSkills.Param TraSkill1;
    public Client_DataSkills.Param TraSkill2;
    public Client_DataSkills.Param TraSkill3;
    public Client_DataSkills.Param EnhTraSkill1;
    public Client_DataSkills.Param EnhTraSkill2;
    public Client_DataSkills.Param EnhTraSkill3;
    public Client_DataSkills.Param TraTriSkill1;
    public Client_DataSkills.Param TraTriSkill2;
    public Client_DataSkills.Param TraTriSkill3;
    public Client_DataSkills.Param TraTriSkill4;
    public Client_DataSkills.Param TraTriSkill5;
    public Client_DataSkills.Param TraRetSkill;
    public Client_DataSkills.Param TraDeaSkill;

    public Client_DataSkills.Param Skill2b;
    public Client_DataSkills.Param Skill2c;
    public Client_DataSkills.Param Skill3b;
    public Client_DataSkills.Param Skill3c;
    public Client_DataSkills.Param EnhSkill2b;
    public Client_DataSkills.Param EnhSkill2c;
    public Client_DataSkills.Param EnhSkill3b;
    public Client_DataSkills.Param EnhSkill3c;
}
public class ClientDataTable : MonoBehaviour
{

    public HeroSkill _heroSkill;
    public List<ScriptableObject> _clientDatas;
    public UnityDictionary<int, ProtoHero> DIC_HERO = new UnityDictionary<int, ProtoHero>();

    public List<int> TRANS_HERO_INDEX = new List<int>();
    public static ClientDataTable Instance { get; private set; }

    public int SELECT_TEAM_DEATH = 1;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

    }

    void Start()
    {
        //UpdateDataHero();
    }


    private void UpdateDataHero()
    {
        Client_DataHero dataHero = GetClientData<Client_DataHero>();

        for (int i = 0; i < dataHero.param.Count; i++)
        {
            ProtoHero hero = new ProtoHero();
            hero.DataHero = dataHero.param[i];

            DIC_HERO.Add(dataHero.param[i].Idx, hero);
        }
    }
    private Client_DataSkills.Param LoadSkillDataAndCheckError(int index)
    {
        Client_DataSkills.Param retSkill = GetSkill(index);
        if (index != 0)
        {
            if (retSkill == null)
            {
                Debug.LogError("UpdateDataHero dataSkills._dic.ContainsKey[" + index + "]");
            }
        }
        return retSkill;
    }

    public void LoadTransHeroIndex_Temp()
    {
        TRANS_HERO_INDEX.Clear();
        TRANS_HERO_INDEX.Add(104000);
        TRANS_HERO_INDEX.Add(112000);
        TRANS_HERO_INDEX.Add(134000);
        TRANS_HERO_INDEX.Add(143000);
    }

    public T GetClientData<T>() where T : ScriptableObject
    {
        ScriptableObject so = _clientDatas.Where(x => x.GetType() == typeof(T)).SingleOrDefault();
        return so as T;
    }

    public Client_DataHero.Param GetHero(int index)
    {
        Client_DataHero heros = GetClientData<Client_DataHero>();

        Client_DataHero.Param hero = null;

        if (heros != null)
        {
            if (heros._dic.ContainsKey(index) == true)
            {
                hero = heros._dic[index];
            }
        }

        return hero;
    }


    public Client_DataSkills.Param GetSkill(int skillIndex)
    {
        Client_DataSkills.Param skill = null;

        Client_DataSkills dataSkill = GetClientData<Client_DataSkills>();

        if (dataSkill != null)
        {
            if (dataSkill._dic.ContainsKey(skillIndex) == true)
            {
                skill = dataSkill._dic[skillIndex];
            }
        }

        return skill;
    }

    public int GetHeroIndex(string name)
    {
        ProtoHero protoHero = DIC_HERO.FirstOrDefault(x => x.Value.DataHero.Name.Equals(name)).Value;
        if (protoHero != null)
        {
            return protoHero.DataHero.Idx;
        }
        else
        {
            return 0;
        }
    }
}
