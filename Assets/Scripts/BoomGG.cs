using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomGG : MonoBehaviour
{
    public GameObject gameEnter;

    private void OnMouseDown()
    {
        BoomMapEngine.Instance.GameOver();
        gameObject.SetActive(false);
        gameEnter.SetActive(true);
    }
}
