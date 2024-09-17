using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Client_HeroSkillDescription", menuName = "ScriptableObjects/HeroSkillDescription", order = 2)]
public class HeroSkillDescription : ScriptableObject
{
    public List<Param> param = new List<Param>();
    public UnityDictionary<int, Param> _dic = new UnityDictionary<int, Param>();

    [System.SerializableAttribute]
    public class Param
    {
        public int IDSkill;
        public int HeroIdx;
        public string Description;
    }
}
