using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFloat : MonoBehaviour
{
    Vector3 inc = new Vector3(0, 0.25f, 0);
    Color col = new Color(1, 0, 0, 1);
    Color incCol = new Vector4(0, 0, 0, 0.001f);

    void Update()
    {
        gameObject.transform.position += inc;
        col -= incCol;
        gameObject.GetComponent<Text>().color = col;

        Destroy(gameObject, 3);
    }
}
