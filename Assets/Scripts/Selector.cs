using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selector : MonoBehaviour
{
    private Camera cam;
    public static Selector inst;
    float rayOut = 0.0f;

    Plane plane;
    Ray ray;
    Vector3 newPos;

    void Awake()
    {
        inst = this;
    }

    void Start()
    {
        cam = Camera.main;
    }

    public Vector3 GetCurTilePosition()
    {
        plane = new Plane(Vector3.up, Vector3.zero);
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if(plane.Raycast(cam.ScreenPointToRay(Input.mousePosition), out rayOut))
        {
            newPos = ray.GetPoint(rayOut) - new Vector3(0.5f, 0.0f, 0.5f);
            return new Vector3(Mathf.CeilToInt(newPos.x), 0.1f, Mathf.CeilToInt(newPos.z));
        }

        return new Vector3(0, -99, 0);
    }
}