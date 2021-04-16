using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaterArrow : MonoBehaviour
{
    public void SetDirection(Water.Direction direction)
    {
        if (direction == Water.Direction.NORTH)
        {
            gameObject.transform.rotation = Quaternion.Euler(90, 90, 0);
        }
        else if (direction == Water.Direction.SOUTH)
        {
            gameObject.transform.rotation = Quaternion.Euler(90, 270, 0);

        }
        else if (direction == Water.Direction.EAST)
        {
            gameObject.transform.rotation = Quaternion.Euler(90, 180, 0);
        }
        else if (direction == Water.Direction.WEST)
        {
            gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);

        }
        else if (direction == Water.Direction.STILL)
        {
            gameObject.SetActive(false);
        }
    }
}
