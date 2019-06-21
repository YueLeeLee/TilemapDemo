using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

public class PlayerDataManager
{
    private static PlayerDataManager mInstance;
    public static PlayerDataManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new PlayerDataManager();
            }

            return mInstance;
        }
    }

    private PlayerData mPlayerData;
    private string mDataPath;

    PlayerDataManager()
    {
        InitPlayerData();
    }

    void InitPlayerData()
    {

#if UNITY_EDITOR
        mDataPath = Application.dataPath + "/PlayerData.json";
#else
        mDataPath = Application.persistentDataPath + "/PlayerData.json";
#endif

        Debug.Log("PlayerData : " + mDataPath);

        if (!File.Exists(mDataPath)) 
        {
            FileStream fs = new FileStream(mDataPath, FileMode.Create, FileAccess.ReadWrite);
            fs.Close();
        }
        else
        {
            StreamReader sr = new StreamReader(mDataPath);
            string content = sr.ReadLine();
            sr.Close();

            if (!string.IsNullOrEmpty(content)) 
            {
                mPlayerData = JsonMapper.ToObject<PlayerData>( content);
            }
        }

        if (mPlayerData != null)
        {
            Debug.Log(mPlayerData.playerId);
        }
        else
        {
            mPlayerData = new PlayerData();
            mPlayerData.playerId = Application.identifier;
            mPlayerData.winCount = 0;
            mPlayerData.loseCount = 0;
        }
    }

    public int GetWinScore()
    {
        return mPlayerData.winCount;
    }

    public int GetLoseScore()
    {
        return mPlayerData.loseCount;
    }

    public void AddWinScore()
    {
        mPlayerData.winCount++;
    }

    public void AddLoseScore()
    {
        mPlayerData.loseCount++;
    }

    public void SavePlayerData()
    {
        StreamWriter sw = new StreamWriter(mDataPath);
        sw.WriteLine(JsonMapper.ToJson(mPlayerData));
        
        sw.Close();
    }
}
