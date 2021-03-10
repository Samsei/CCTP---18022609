using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFloat : MonoBehaviour
{
    Vector3 inc = new Vector3(0, 0.5f, 0);
    Color col = new Color(1, 0, 0, 1);
    Color incCol = new Vector4(0, 0, 0, 0.002f);

    void Update()
    {
        gameObject.transform.position += inc;
        col -= incCol;
        gameObject.GetComponent<Text>().color = col;
        if (gameObject.GetComponent<Text>().color.a == 0)
            Destroy(this, 0);
    }
}
