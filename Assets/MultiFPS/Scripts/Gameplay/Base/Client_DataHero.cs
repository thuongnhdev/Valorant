using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Client_DataHero", order = 1)]
public class Client_DataHero : ScriptableObject
{
	public List<Param> param = new List<Param>();
	public UnityDictionary<int, Param> _dic = new UnityDictionary<int, Param>();

	[System.SerializableAttribute]
	public class Param
	{
		public int Idx;
		public string Name;
		public int RoleCategory;
		public bool IsLock;
		public int AssetIndex;
	}
}