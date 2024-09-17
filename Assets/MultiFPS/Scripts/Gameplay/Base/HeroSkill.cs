using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HeroSkill", order = 1)]
public class HeroSkill : ScriptableObject
{
    public List<Param> param = new List<Param>();
    public UnityDictionary<int, Param> _dic = new UnityDictionary<int, Param>();

    [System.SerializableAttribute]
    public class Param
    {
        public int IDSkill;
        public float EquipTime;
        public float MaxDistance;
        public float MaxDeployDistance;
        public float UnequipTime;
        public float AreaSize;
        public float orbTravelSpeed;
        public float MaxDuration;
        public float TimeNeededToArm;
        public float DashDistance;
        public float Windup;
        public float EmpoweredDuration;
        public float DashDuration;
        public float InitialWindupTime;
        public float Ammunition;
        public float ReequipTime;
        public float QuickEquipTime;
        public float DamageMultipliers;
        public float SingleFire;
        public float MaxDetectionConDistance;
        public float proximityToTargetNeededToExplode;
        public float MaximumTravelTimeInTheAir;
        public float WindupTime;
        public float Radii;
        public float Damage;
        public float BoomBotHP;
        public float DeployTime;
        public float Radius;
        public float Duration;
        public float VisionRadius;
        public float NearsightDuration;
        public float TeleportDuration;
        public float AudioCueRadius;
        public float ArrivalAudioCueRadius;
        public int Active;
        public int Recharge;
        public int Credit;
        public int BlastPackHP;
    }

}
