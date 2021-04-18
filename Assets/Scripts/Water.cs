using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public class Water : MonoBehaviour
{
    public float currentPollution = 0.0f;

    private Renderer _renderer;
    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }

    public enum Direction
    {
        STILL,
        NORTH,
        SOUTH,
        EAST,
        WEST
    }

    public Direction direction;

    private void Start()
    {
        if (direction != Direction.STILL)
        {
            GetComponentInChildren<WaterArrow>().SetDirection(direction);
        }
    }

    public void EndTurn(Dictionary<Vector3, GameObject> waterTiles)
    {
        if (currentPollution > 256.0f)
        {
            currentPollution = 256.0f;
        }

        //currentPollution -= 0.1f;

        if (currentPollution < 0.0f)
        {
            currentPollution = 0.0f;
        }

        _renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", new Color(1.0f, 1.0f - (currentPollution / 512.0f), 1.0f - (currentPollution / 256.0f), 1.0f));
        _renderer.SetPropertyBlock(mpb);
    }
}
