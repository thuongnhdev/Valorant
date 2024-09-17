using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiFPS.UI.HUD
{
    public class UICountDownSkillJett : MonoBehaviour
    {
        [SerializeField] Image _bar;
        [SerializeField] Text _msg;
        Coroutine _barProcedure;
        Coroutine _counterProcedure;

        private void Awake()
        {
            gameObject.SetActive(false);
        }
        public void StartUI(float cooldown, Action<bool> onComplete)
        {
            gameObject.SetActive(true);

            if (_barProcedure != null)
            {
                StopCoroutine(_barProcedure);
                StopCoroutine(_counterProcedure);
            }

            _barProcedure = StartCoroutine(Bar());
            _counterProcedure = StartCoroutine(Counter());

            IEnumerator Bar()
            {
                _bar.fillAmount = 1f;

                float barFillingSpeed = 1f / cooldown;
                float progress = 1;
                while (progress > 0)
                {
                    progress -= barFillingSpeed * Time.deltaTime;
                    _bar.fillAmount = progress;
                    yield return null;
                }

            }
            IEnumerator Counter()
            {
                while (cooldown > 0)
                {
                    //_msg.text = $"Skill will auto in {cooldown} second";
                    cooldown--;
                    yield return new WaitForSeconds(1f);
                }
                gameObject.SetActive(false);
                onComplete?.Invoke(true);
            }
        }
   
        public void HideUI()
        {
            if (_barProcedure != null)
            {
                StopCoroutine(_barProcedure);
                StopCoroutine(_counterProcedure);
            }
            gameObject.SetActive(false);
        }
    }
}
