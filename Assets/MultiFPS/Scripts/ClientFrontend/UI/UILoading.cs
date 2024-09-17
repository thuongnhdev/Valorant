using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiFPS.UI
{
    public class UILoading : UIMenu
    {
        private void OnEnable()
        {
            StartCoroutine(Loading());
        }

        private IEnumerator Loading()
        {
            float timeWait = 3f;
            while (timeWait > 0)
            {
                yield return new WaitForSeconds(1);
                timeWait--;
                if (timeWait == 0)
                {
                    this.Hide();
                    ClientInterfaceManager.Instance.UISelectMenu.gameObject.SetActive(true);
                }
            }
          
        }    
    }
}