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

    private int tolerance;
}
