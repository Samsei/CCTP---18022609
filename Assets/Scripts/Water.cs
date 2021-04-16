using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public class Water : MonoBehaviour
{
    int stop = 0;
    public enum Direction
    {
        STILL,
        NORTH,
        SOUTH,
        EAST,
        WEST
    }

    public Direction direction;

    private void Update()
    {
        if (stop == 0 && direction != Direction.STILL)
        {
            GetComponentInChildren<WaterArrow>().SetDirection(direction);
            stop++;
        }
    }
}
