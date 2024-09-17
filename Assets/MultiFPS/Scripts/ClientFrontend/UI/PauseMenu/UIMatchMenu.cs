using MultiFPS;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace MultiFPS.UI
{
    public class UIMatchMenu : UIMenu
    {
        [SerializeField] private TextMeshProUGUI TmpCoolDown;
        [SerializeField] private Animator animatorCount;

        private const float CooldownDuration = 5f;
        private const string AnimatorCountBool = "IsCount";

        private void OnEnable()
        {
            StartCoroutine(CooldownCoroutine());
        }

        private IEnumerator CooldownCoroutine()
        {
            animatorCount.SetBool(AnimatorCountBool, true);
            float remainingCooldown = CooldownDuration;

            while (remainingCooldown > 0)
            {
                UpdateCooldownDisplay(remainingCooldown);
                yield return new WaitForSeconds(1f);
                remainingCooldown--;
            }

            EndCooldown();
        }

        private void UpdateCooldownDisplay(float remainingCooldown)
        {
            int totalSeconds = Mathf.CeilToInt(remainingCooldown);
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);

            if (timeSpan.Hours > 0)
            {
                TmpCoolDown.text = timeSpan.ToString(@"hh\:mm\:ss");
            }
            else if (timeSpan.Minutes > 0)
            {
                TmpCoolDown.text = timeSpan.ToString(@"mm\:ss");
            }
            else
            {
                TmpCoolDown.text = timeSpan.Seconds.ToString();
            }
        }

        private void EndCooldown()
        {
            ClientInterfaceManager.Instance.UISelectMenu.gameObject.SetActive(true);
            gameObject.SetActive(false);
            animatorCount.SetBool(AnimatorCountBool, false);
        }
    }
}
