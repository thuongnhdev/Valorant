using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxSkillController : MonoBehaviour
{
    [SerializeField]
    public ParticleSystem[] ParticleFxSkill;

    public void StartFx()
    {
        for (int i = 0; i < ParticleFxSkill.Length; i++)
        {
            ParticleFxSkill[i].Play(true);
        }
     
    }

    public void StopFx()
    {
        for (int i = 0; i < ParticleFxSkill.Length; i++)
        {
            ParticleFxSkill[i].gameObject.SetActive(false);
            ParticleFxSkill[i].Stop();
        }
     
    }
}
