using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiFPS.Gameplay
{
    [DisallowMultipleComponent]
    [AddComponentMenu("MultiFPS/Items/Grenade")]
    public class Grenade : Item
    {
        public GameObject ThrowablePrefab;
        public float RigidBodyForce = 2500;
        public bool isSkill1 = true;

        protected override void Awake()
        {
            base.Awake();

            ChangeCurrentAmmoCount(CurrentAmmoSupply);
        }
        protected override void Use()
        {
            base.Use();

            if (CurrentAmmoSupply <= 0) return;

            if (isServer)
                SpawnThrowable(new Vector2(_myOwner.Input.LookX, _myOwner.Input.LookY));
            else
            {
                CmdSpawnThrowable(new Vector2(_myOwner.Input.LookX, _myOwner.Input.LookY));

                CurrentAmmoSupply--;
                ChangeCurrentAmmoCount(CurrentAmmoSupply);
            }

            if (CurrentAmmoSupply <= 0) _myOwner.CharacterItemManager.ChangeItemDelay(-1, 0.45f);
        }
        [Command]
        void CmdSpawnThrowable(Vector2 look) 
        {
            SpawnThrowable(look);
        }
        void SpawnThrowable(Vector2 look) 
        {
            if (CurrentAmmoSupply <= 0) return;

            CurrentAmmoSupply--;
            ChangeCurrentAmmoCount(CurrentAmmoSupply);

            GameObject throwable = Instantiate(ThrowablePrefab, _myOwner.Health.GetPositionToAttack(), Quaternion.Euler(_myOwner.Input.LookX, _myOwner.Input.LookY, 0));

            // just adjusted direction before throw the ganrade
            var dir = isSkill1 ? Vector3.up / 4f : Vector3.zero;
            Vector3 force = Quaternion.Euler(look.x, look.y, 0)*(Vector3.forward+ dir) * RigidBodyForce;

            throwable.GetComponent<Throwable>().Activate(_myOwner, force);
            NetworkServer.Spawn(throwable);
        }
        public override void Take()
        {
            base.Take();
            UpdateAmmoInHud(CurrentAmmo.ToString());
        }

        /*protected override void OnOwnerPickedupAmmo()
        {
            //UpdateAmmoInHud(CurrentAmmoSupply.ToString());
        }*/

        protected override void SingleUse()
        {
            //if we are out of granades, hide granade model from player hand
            

            _myOwner.CharacterItemManager.StartUsingItem(); //will disable ability tu run for 0.5 seconds
            _myOwner.CharacterAnimator.SetTrigger("recoil");
            _myAnimator.SetTrigger("recoil");
        }

        public override bool CanBeEquiped()
        {
            return base.CanBeEquiped() && CurrentAmmoSupply > 0;
        }

        protected override void OnCurrentAmmoChanged()
        {
            UpdateAmmoInHud(CurrentAmmo.ToString());
        }
    }
}