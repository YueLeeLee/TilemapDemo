using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoomMapEngine : MonoBehaviour
{
    public enum GameRoundLevel
    {
        EASY,
        NORMAL,
        HARD,
    }

    public static BoomMapEngine Instance;

    private BoundsInt mMapBounds;
    private int[][] mBoomData;
    private int mBoomCount;
    private int mOpenedCellCount;
    private int mTotalCellCount;
    private List<Vector3Int> mSpreadCellPool = new List<Vector3Int>();
    private List<Vector3Int> mBoomList = new List<Vector3Int>();
    private Vector3Int mMinPos;
    private Vector3Int mMaxPos;

    private UnityAction OnStartGame;
    private UnityAction OnGameOver;

    public List<Vector3Int> BoomList
    {
        get { return mBoomList; }
    }

    public Vector3Int MinPos
    {
        get { return mMinPos; }
    }

    public Vector3Int MaxPos
    {
        get { return mMaxPos; }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void StartGame(GameRoundLevel level)
    {
        SetGameLevel(level);

        GenerateNewMap();

        if (OnStartGame != null)
        {
            OnStartGame();
        }
    }

    public void GameOver()
    {
        if (OnGameOver != null)
        {
            OnGameOver();
        }
    }

    void SetGameLevel(GameRoundLevel level)
    {
        int x = 0;
        int y = 0;
        switch(level)
        {
            case GameRoundLevel.EASY:
                {
                    x = 4;
                    y = 3;
                }
                break;
            case GameRoundLevel.NORMAL:
                {
                    x = 6;
                    y = 4;
                }
                break;
            case GameRoundLevel.HARD:
                {
                    x = 7;
                    y = 5;
                }
                break;
        }
        mMinPos = new Vector3Int(-x, -y, 0);
        mMaxPos = new Vector3Int(x, y, 0);

        mBoomCount = x * y * 3 / 4;
        mMapBounds = new BoundsInt(-x, -y, 0, x * 2 + 1, y * 2 + 1, 0);

        mTotalCellCount = mMapBounds.size.x * mMapBounds.size.y;
        mOpenedCellCount = 0;

        RefreshMapData();

        Debug.Log(mMapBounds);
    }

    public void RegisterStartGameCallback(UnityAction act)
    {
        OnStartGame += act;
    }

    public void RegisterGameOverCallback(UnityAction act)
    {
        OnGameOver += act;
    }

    public bool RefreshOpenedCellCount(int newopen)
    {
        mOpenedCellCount += newopen;

        if (mOpenedCellCount + mBoomCount >= mTotalCellCount)
            return true;

        return false;
    }

    public int GetBoomCountOnPos(int x,int y)
    {
        x -= mMapBounds.xMin;
        y -= mMapBounds.yMin;

        if (x < mMapBounds.size.x && y < mMapBounds.size.y && x >= 0 && y >= 0)
        {
            return mBoomData[x][y];
        }

        Debug.LogError("GetBoomCountOnPos : " + x + " , " + y);
        return 0;
    }

    public List<Vector3Int> GetSpreadCells(Vector3Int v)
    {
        List<Vector3Int> list = new List<Vector3Int>();

        mSpreadCellPool.Clear();

        CheckCell(v, ref list);

        return list;
    }

    void CheckCell(Vector3Int v, ref List<Vector3Int> sumlist)
    {
        if (mSpreadCellPool.Contains(v))
            return;

        mSpreadCellPool.Add(v);

        Vector3Int[] arounds = { Vector3Int.left, Vector3Int.right, Vector3Int.up, Vector3Int.down
        ,new Vector3Int(-1,-1,0),new Vector3Int(1,-1,0),new Vector3Int(-1,1,0),new Vector3Int(1,1,0)};

        List<Vector3Int> templist = new List<Vector3Int>();

        for (int j = 0; j < arounds.Length; j++)
        {
            Vector3Int pos = v + arounds[j];
            if (pos.x < mMapBounds.xMax && pos.x >= mMapBounds.xMin
                && pos.y < mMapBounds.yMax && pos.y >= mMapBounds.yMin)
            {
                int bc = GetBoomCountOnPos(pos.x, pos.y);
                if (bc != -1 && !sumlist.Contains(pos)) 
                {
                    if (bc == 0)
                        templist.Add(pos);
                    //Debug.Log(bc);
                    sumlist.Add(pos);
                }
            }
        }

        for (int i = 0; i < templist.Count; i++) 
        {
            CheckCell(templist[i], ref sumlist);
        }
    }

    void GenerateNewMap()
    {
        //随机生成boom位置/计算每个位置数值
        Vector3Int[] arounds = { Vector3Int.left, Vector3Int.right, Vector3Int.up, Vector3Int.down
        ,new Vector3Int(-1,-1,0),new Vector3Int(1,-1,0),new Vector3Int(-1,1,0),new Vector3Int(1,1,0)};

        mBoomList.Clear();
        mBoomList = GenerateBooms();
        for (int i = 0; i < mBoomList.Count; i++)
        {
            //Debug.Log(booms[i]);
            SetBoomMapData(mBoomList[i], -1);

            for (int j = 0; j < arounds.Length; j++)
            {
                Vector3Int pos = mBoomList[i] + arounds[j];
                if (pos.x < mMapBounds.xMax && pos.x >= mMapBounds.xMin
                    && pos.y < mMapBounds.yMax && pos.y >= mMapBounds.yMin)
                {
                    SetBoomMapData(pos, 1);
                }
            }
        }

    }

    void RefreshMapData()
    {
        mBoomData = new int[mMapBounds.size.x][];
        for (int i = 0; i < mMapBounds.size.x; i++)
        {
            mBoomData[i] = new int[mMapBounds.size.y];
        }
    }

    void SetBoomMapData(Vector3Int pos, int addValue)
    {
        int x = pos.x - mMapBounds.xMin;
        int y = pos.y - mMapBounds.yMin;

        if (mBoomData[x][y] != -1)
        {
            if (addValue == -1)
            {
                mBoomData[x][y] = -1;
            }
            else
            {
                mBoomData[x][y] += addValue;
            }
        }
    }

    List<Vector3Int> GenerateBooms()
    {
        List<Vector3Int> booms = new List<Vector3Int>();

        System.Random rand = new System.Random();
        
        for (int i = 0; i < mBoomCount; i++)
        {
            int j = 0;
            while (j++ < 5)
            {
                int x = rand.Next(mMapBounds.xMin, mMapBounds.xMax);
                int y = rand.Next(mMapBounds.yMin, mMapBounds.yMax);
                //int x = Random.Range(mMapBounds.xMin, mMapBounds.xMax);
                //int y = Random.Range(mMapBounds.yMin, mMapBounds.yMax);

                Vector3Int vi = new Vector3Int(x, y, 0);
                if (!booms.Contains(vi))
                {
                    booms.Add(vi);
                    break;
                }
            }
        }

        return booms;
    }

    private void OnApplicationQuit()
    {
        PlayerDataManager.Instance.SavePlayerData();
    }
}
