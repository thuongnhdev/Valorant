using MultiFPS.Gameplay;
using MultiFPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class SkillDamage : MonoBehaviour
{
    CharacterInstance _character;
    GameObject _bloodEffect;
    int _damage = 10;
    // Start is called before the first frame update
    void Start()
    {
    
    }
    // Update is called once per frame
    void Update()
    {
        checkCollider();
    }
    public void setHitEffect(CharacterInstance character,GameObject bloodEff,int damage)
    {
        _character = character;
        _bloodEffect = bloodEff;
        _damage= damage;
    }
    public void checkCollider()
    {
        RaycastHit hit;
        float distance = 1;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.forward, out hit, distance, 1 << 6))
        {
            takeDamage();
        }
    }
    public void takeDamage()
    {
        Quaternion hitRotation = Quaternion.identity;
        RaycastHit[] hitScan = GameTools.HitScan(transform, _character.transform, GameManager.fireLayer, 250f);

        int penetratedObjects = 0;
        //we hit something, so we have to check what to do next
        if (hitScan.Length > 0)
        {
            hitRotation = Quaternion.FromToRotation(Vector3.forward, hitScan[0].normal);
            for (int i = 0; i < Mathf.Min(hitScan.Length, 2); i++)
            {
                penetratedObjects++;
                RaycastHit currentHit = hitScan[i];
                GameObject go = currentHit.collider.gameObject;
                // Debug.Log("hit name= " + go.name);
                HitBox hb = go.GetComponent<HitBox>();
                if (hb)
                {
                    if (!_character.BOT)
                    {
                        CmdDamage(hb._health.DNID, hb.part, 1f / (i + 1), i == 0 ? AttackType.hitscan : AttackType.hitscanPenetrated); //the more objects we penetrated the less damage we deal
                    }
                    else
                    {
                        ServerDamage(hb._health, hb.part, 1f / (i + 1), i == 0 ? AttackType.hitscan : AttackType.hitscanPenetrated);
                    }
                }
                if (go.layer == 0) //if we hitted solid wall, dont penetrate it further
                    break;
            }

        }
        PlayEffect();
        Destroy(gameObject);
    }
    void CmdDamage(byte damagedHealthID, CharacterPart hittedPart, float damagePercentage, AttackType attackType)
    {
        Health victim = GameSync.Singleton.Healths.GetObj(damagedHealthID);
        if (victim == null) return;

        //if (Server_CurrentAmmo > 0)
            ServerDamage(victim, hittedPart, damagePercentage, attackType);
    }

    /// <summary>
    /// apply damage to victim
    /// </summary>
     void ServerDamage(Health damagedHealth, CharacterPart hittedPart, float damagePercentage, AttackType attackType)
    {
        //damagedHealth.Server_ChangeHealthState(_damage,
        //    hittedPart, attackType, _character.Health, 100);
        //print($"MultiFPS: Damage given to {GameManager.GetHealthInstance(hittedHealthID).name}: {_damage * damagePercentage}");
    }
    public async void PlayEffect()
    {
        if (_bloodEffect == null)
            return;
        GameObject eff=GameObject.Instantiate(_bloodEffect, transform.position, Quaternion.identity);
        await Task.Delay(500);
        Destroy(eff);

    }
}
