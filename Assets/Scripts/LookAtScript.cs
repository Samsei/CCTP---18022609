using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class LookAtScript : MonoBehaviour
{
    ConstraintSource cam;

    void Start()
    {
        cam.sourceTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        cam.weight = 1;
        GetComponent<LookAtConstraint>().AddSource(cam);
        gameObject.SetActive(false);
    }
}
