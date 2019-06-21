using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameEnter : MonoBehaviour
{
    Tilemap tilemap;

    [SerializeField]
    Animator aniWalker;

    BoomMapEngine.GameRoundLevel chosedLevel;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }


    private void OnMouseDown()
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int v = tilemap.WorldToCell(wp);


        int xpos = v.x - tilemap.cellBounds.xMin;
        Debug.Log(v + " , " + xpos, this);
        if (xpos < tilemap.cellBounds.size.x / 3)
        {
            chosedLevel = BoomMapEngine.GameRoundLevel.EASY;
            //StartCoroutine("PlayWalk", tilemap.cellBounds.xMin + tilemap.cellBounds.size.x / 3 - 3);
        }
        else if (xpos < tilemap.cellBounds.size.x * 2 / 3)
        {
            chosedLevel = BoomMapEngine.GameRoundLevel.NORMAL;
            //StartCoroutine("PlayWalk", tilemap.cellBounds.xMin + tilemap.cellBounds.size.x *2/ 3 - 3);
        }
        else
        {
            chosedLevel = BoomMapEngine.GameRoundLevel.HARD;
            //StartCoroutine("PlayWalk", tilemap.cellBounds.xMin + tilemap.cellBounds.size.x - 6);
        }

        BoomMapEngine.Instance.StartGame(chosedLevel);
        gameObject.SetActive(false);
    }

    private void OnMouseOver()
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aniWalker.transform.position = new Vector3(wp.x, wp.y, 0);
    }

}
