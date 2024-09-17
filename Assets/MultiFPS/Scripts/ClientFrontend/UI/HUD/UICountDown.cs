using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiFPS.UI.HUD
{
    public class UICountDown : MonoBehaviour
    {
        [SerializeField] Image _bar;
        [SerializeField] Text _msg;
        Coroutine _barProcedure;
        Coroutine _counterProcedure;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void StartUI(int typeClick)
        {
            gameObject.SetActive(true);
            if (typeClick == 0)
            {
                _bar.fillAmount = 1.0f;
            }
            else
            {
                _bar.fillAmount = 0.25f;
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
