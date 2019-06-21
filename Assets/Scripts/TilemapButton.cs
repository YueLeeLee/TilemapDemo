using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapButton : MonoBehaviour
{
    Tilemap tilemap;
    TilemapCollider2D tileCollider;

    public Tile mTile;
    public TileBase mMarkTile;
    public GameObject mGameOver;
    public GameObject mGamePass;

    private bool mIsGameOver = false;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        tileCollider = GetComponent<TilemapCollider2D>();

        //Debug.Log(tilemap.cellBounds);

        BoomMapEngine.Instance.RegisterStartGameCallback(RecoverMap);
        BoomMapEngine.Instance.RegisterGameOverCallback(OnGameOver);
    }

    void OnGameOver()
    {
        gameObject.SetActive(false);
    }

    public void RecoverMap()
    {
        gameObject.SetActive(true);

        tilemap.ClearAllTiles();
        tilemap.SetTile(BoomMapEngine.Instance.MinPos, mTile);
        tilemap.SetTile(BoomMapEngine.Instance.MaxPos, mTile);
        tilemap.ResizeBounds();

        Debug.Log(tilemap.cellBounds);

        mIsGameOver = false;
        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                mTile.color = new Color(Mathf.Abs(x) * 0.2f, Mathf.Abs(y) * 0.2f, 0.2f, 1f);
                tilemap.SetTile(new Vector3Int(x, y, 0), mTile);
            }
        }

        if (tileCollider != null)
        {
            StartCoroutine("ReenableCollider");
        }
    }

    IEnumerator ReenableCollider()
    {
        tileCollider.enabled = false;
        yield return new WaitForEndOfFrame();
        tileCollider.enabled = true;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(1)) 
        {
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int v = tilemap.WorldToCell(wp);
            TileBase t = tilemap.GetTile(v);
            if (t != null)
            {
                tilemap.SetTile(v, null);
                tilemap.SetTile(v, mMarkTile);
            }
        }

    }

    private void OnMouseDown()
    {
        if (mIsGameOver)
        {
            return;
        }

        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int v = tilemap.WorldToCell(wp);
        Debug.Log(v);
        RefreshMap(v);
    }

    void RefreshMap(Vector3Int v)
    {
        tilemap.SetTile(v, null);
        int openedCount = 1;
        int boomcount = BoomMapEngine.Instance.GetBoomCountOnPos(v.x, v.y);
        if (boomcount == -1) 
        {
            mIsGameOver = true;
            ShowGG();
            return;
            //StartCoroutine("ShowGG");
        }
        else if(boomcount == 0)
        {
            List<Vector3Int> list = BoomMapEngine.Instance.GetSpreadCells(v);
            foreach (var p in list)
            {
                if(tilemap.HasTile(p))
                {
                    tilemap.SetTile(p, null);
                    openedCount++;
                }
            }
        }

        if(BoomMapEngine.Instance.RefreshOpenedCellCount(openedCount))
        {
            mIsGameOver = true;
            ShowPass();
        }
    }

    void ShowGG()
    {
        List<Vector3Int> boomlist = BoomMapEngine.Instance.BoomList;
        foreach(var b in boomlist)
        {
            tilemap.SetTile(b, null);
        }

        mGameOver.SetActive(true);

        PlayerDataManager.Instance.AddLoseScore();

        //yield return new WaitForSeconds(1f);

        //BoomMapEngine.Instance.GameOver();
    }

    void ShowPass()
    {
        List<Vector3Int> boomlist = BoomMapEngine.Instance.BoomList;
        foreach (var b in boomlist)
        {
            tilemap.SetTile(b, null);
        }

        mGamePass.SetActive(true);

        PlayerDataManager.Instance.AddWinScore();
    }
    
}
