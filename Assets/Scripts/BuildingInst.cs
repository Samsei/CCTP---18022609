using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInst : MonoBehaviour
{
    public string displayName;

    public int cost;
    public int costPerTurn;

    public int maxFood;
    public int maxPopulation;
    public int maxJobs;

    public int food;
    public int population;
    public int jobs;

    public int happiness;
    public int garbageBuildup;

    public int pollutionPerTurn;

    private int tolerance;

    private RaycastHit hitObject;
    private GameObject hO;

    public void EndTurn(int water, int electricity)
    {
        if (pollutionPerTurn > 0)
        {
            Ray ray = new Ray();
            ray.origin = gameObject.transform.position;
            ray.direction = new Vector3(0, 1, 0);
            if (Physics.Raycast(ray, out hitObject))
            {
                hO = hitObject.transform.gameObject;
                hO.GetComponent<Pollution>().currentPollution += pollutionPerTurn;
            }
        }

        if (water > 0)
        {
            City.inst.curWater -= population;
        }

        if (electricity > 0)
        {
            City.inst.curElectricity -= population;
        }
    }
}
