using System.Collections.Generic;
using UnityEngine;

public class HeroData
{
    public int idx;
    public string name;
    public int assetIdx;
    public bool isLock;
}

public class SkillDescriptionData
{
    public int SkillType;
    public int HeroIdx;
    public string Description;
}

    public class MasterCatcher : MonoBehaviour
{
    public static MasterCatcher Instance = null;

    private List<HeroData> heroDatas = new List<HeroData>();
    private List<SkillDescriptionData> skillDescriptionDatas = new List<SkillDescriptionData>();

    public List<HeroData> HeroDatas { get => heroDatas; set => heroDatas = value; }
    public List<SkillDescriptionData> SkillDescriptionDatas { get => skillDescriptionDatas; set => skillDescriptionDatas = value; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetDataHeroList();
        SetDataSkillDescriptionList();
    }


    private void SetDataHeroList()
    {
        var datas = ClientDataTable.Instance.GetClientData<Client_DataHero>();
        foreach (var data in datas.param)
        {
            var nData = new HeroData();
            nData.idx = data.Idx;
            nData.name = data.Name;
            nData.assetIdx = data.AssetIndex;
            nData.isLock = data.IsLock;
            heroDatas.Add(nData);
        }
    }

    private void SetDataSkillDescriptionList()
    {
        var datas = ClientDataTable.Instance.GetClientData<HeroSkillDescription>();
        foreach (var data in datas.param)
        {
            var nData = new SkillDescriptionData();
            nData.SkillType = data.IDSkill;
            nData.HeroIdx = data.HeroIdx;
            nData.Description = data.Description;

            skillDescriptionDatas.Add(nData);
        }
    }
}
