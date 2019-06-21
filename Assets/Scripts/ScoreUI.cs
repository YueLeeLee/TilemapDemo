using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    Text mWinScore;

    [SerializeField]
    Text mLoseScore;

    void Start()
    {
        RefreshUI();

        BoomMapEngine.Instance.RegisterGameOverCallback(RefreshUI);
    }

    void RefreshUI()
    {
        mWinScore.text = string.Format("WIN:{0}", PlayerDataManager.Instance.GetWinScore());
        mLoseScore.text = string.Format("LOSE:{0}", PlayerDataManager.Instance.GetLoseScore());
    }
}
