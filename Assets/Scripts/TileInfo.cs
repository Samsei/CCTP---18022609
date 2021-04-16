using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    [SerializeField] GameObject infoBar;
    Vector3 curTile;

    public static TileInfo inst;

    private void Start()
    {
        inst = this;
    }

    public void OpenInfoBar(GameObject tile)
    {
        infoBar.SetActive(true);
    }
}
