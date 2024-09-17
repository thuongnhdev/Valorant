using UnityEngine;
public class BattleEffectInfo : MonoBehaviour
{
    public int PosIndex;
    public Transform[] TargetScaleList;

    public void SetScale(Vector3 VecScale)
    {
        for (int i = 0; i < TargetScaleList.Length; i++)
        {
            if (TargetScaleList[i] == null)
            {
                Debug.LogError("[" + gameObject.name + "][" + gameObject.name + "][" + i + "]");
            }
            else
            {
                TargetScaleList[i].localScale = VecScale;
            }
        }
    }
}
