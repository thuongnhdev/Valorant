using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Client_DataSkills : ScriptableObject
{
	public List<Param> param = new List<Param>();
	public UnityDictionary<int, Param> _dic = new UnityDictionary<int, Param>();

	[System.SerializableAttribute]
	public class Param
	{

		public int Idx;
		public string Note;
		public string Name;
		public int SkillType;
		public int SkillHits;
		public int IncrementalDmg;
		public int Cost;
		public int CoolTime;
		public int RandomSkillRatio;
		public int IsPassiveSkill;
		public int TriggerObject;
		public int TriggerCondition;
		public int TrigStatusEffect;
		public int TrigStatusEffectValue;
		public float TriggerChance;
		public int TriggerCount;
		public int DeathSkillCounts;
		public int DeathSkillChance;
		public int TargetableRange;
		public int TargetingType;
		public int MotionType;
		public int SkillEffect1Index;
		public int SkillEffect2Index;
		public int SkillEffect3Index;
		public int SkillEffect4Index;
		public int SkillEffect5Index;
		public int SkillEffect6Index;
		public int SkillEffect7Index;
		public int SkillEffect8Index;
		public int IsSureHit;
		public int AddStatusInfo1;
		public int AddStatusInfo2;
		public int AddStatusInfo3;
		public int AnimClip;
		public int CameraType;
		public float CameraTrauma;
		public int CameraCullingType;
		public int ProjAngle;
		public float ProjVeolcity;
		public int ProjDestination;
		public int AddSkill;
		public int SkillCategoryEffect;
		public int SkillCategoryTarget;
		public int SkillOblivion;
	}
}