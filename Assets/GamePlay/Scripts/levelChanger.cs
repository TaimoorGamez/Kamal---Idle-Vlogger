using Core.DB.Variables;
using UnityEngine;

public class levelChanger : MonoBehaviour
{
    [SerializeField] int NextLvl;

    private void OnEnable()
    {
        DBVariablesHolder.LvlIndex.Value = NextLvl;
        DBVariablesHolder.LvlNum.Value = NextLvl;
        //DBVariablesHolder.MaxLvlCount.Value = NextLvl;
    }
}
