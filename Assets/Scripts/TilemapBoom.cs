using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapBoom : MonoBehaviour
{
    Tilemap tilemap;

    [SerializeField]
    TileBase[] numberTiles;
    [SerializeField]
    TileBase emptyTile;
    [SerializeField]
    TileBase boomTile;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();

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
        tilemap.SetTile(BoomMapEngine.Instance.MinPos, emptyTile);
        tilemap.SetTile(BoomMapEngine.Instance.MaxPos, emptyTile);
        tilemap.ResizeBounds();

        //Debug.Log(tilemap.cellBounds);
        //Debug.Log(tilemap.cellBounds.xMin);
        //Debug.Log(tilemap.cellBounds.xMax);
        //Debug.Log(tilemap.cellBounds.yMin);
        //Debug.Log(tilemap.cellBounds.yMax);

        //填充格子
        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                int boomcount = BoomMapEngine.Instance.GetBoomCountOnPos(x, y);//(x - tilemap.cellBounds.xMin,y - tilemap.cellBounds.yMin);
                if (boomcount == -1)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), boomTile);
                }
                else if (boomcount == 0)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), emptyTile);
                }
                else if (boomcount <= numberTiles.Length)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), numberTiles[boomcount - 1]);
                }
                else
                {
                    Debug.LogError("Boom Count error : " + boomcount);
                }
            }
        }
    }
}
