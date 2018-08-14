using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPSystem : MonoBehaviour { 

    int[] levelCosts  =  new int[]{12,72,240,600,1260,2352,4032,6480,9990};
    int[] xpGainPerLevel = new int[] { 2, 6, 12, 20, 30, 42, 56, 72, 90 };

    public static XPSystem instance { get; protected set; }

    Action<XPData> OnGainXP;



    private void Awake()
    {
        instance = this;
        
    }
    public void DoXPGainAction(XPData curXPData, int levelOfAction)
    {
        if (OnGainXP != null)
        {
            int newTotalXP = curXPData.TotalXP + GetXPGain(levelOfAction);
            OnGainXP(new XPData(newTotalXP, GetCurLevel(newTotalXP, curXPData.CurXPLevel)));
        }
    }
    int GetCurLevel(int curXP, int curLevel)
    {
        int levelGain = 0;
        if (curLevel >= levelCosts.Length)
        {
            return curLevel;
        }
        if (curXP > GetLevelCost(curLevel))
        {
            levelGain++;
            for (int i = curLevel + 1; i < levelCosts.Length; i++)
            {
                if (curXP > levelCosts[i])
                {
                    levelGain++;
                }
                else
                    break;
            }
        }
        return curLevel + levelGain;
    }

    int GetXPGain(int levelOfAction)
    {
        if (levelOfAction < 0 || levelOfAction >= xpGainPerLevel.Length)
            return 0;
        return xpGainPerLevel[levelOfAction];
    }
    public int GetLevelCost(int level)
    {
        if (level < 0)
            return 0;
        if (level >= levelCosts.Length)
        {
            return levelCosts[levelCosts.Length - 1];
        }
        return levelCosts[level];
    }

    public void RegisterGetXPCB(Action<XPData> cb)
    {
        OnGainXP += cb;
    }
    public void UnRegisterGetXPCB(Action<XPData> cb)
    {
        OnGainXP -= cb;
    }
}

public struct XPData
{
    private readonly int totalXP;
    private readonly int curXPLevel;

    public XPData(int totalXP, int curXPLevel)
    {
        this.totalXP = totalXP;
        this.curXPLevel = curXPLevel;
    }

    public int TotalXP
    {
        get
        {
            return totalXP;
        }
    }

    public int CurXPLevel
    {
        get
        {
            return curXPLevel;
        }
    }
}