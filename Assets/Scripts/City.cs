using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class City : MonoBehaviour
{
    public int money;
    private int day;
    private int curPopulation;
    private int curJobs;
    private int curFood;
    public int curWater;
    public int curElectricity;
    private int maxPopulation;
    private int maxJobs;
    private int incomePerJob;
    private int turnsUntilWindChange = 5;

    public TextMeshProUGUI statsText;
    private Dictionary<Vector3, GameObject> buildings = new Dictionary<Vector3, GameObject>();
    private Dictionary<Vector3, GameObject> pollutionClouds = new Dictionary<Vector3, GameObject>();
    public Dictionary<Vector3, GameObject> waterTiles = new Dictionary<Vector3, GameObject>();

    public static City inst;

    public enum WindDirection
    {
        STILL = 0,
        NORTH = 1,
        SOUTH = 2,
        EAST = 3,
        WEST = 4
    }

    public WindDirection windDirection;

    void Awake()
    {
        inst = this;
    }

    private void Start()
    {
        foreach (var p in GameObject.FindGameObjectsWithTag("pollution"))
        {
            pollutionClouds.Add(p.transform.position, p);
        }

        foreach (var p in GameObject.FindGameObjectsWithTag("water"))
        {
            waterTiles.Add(p.transform.position, p);
        }
    }

    public void OnPlaceBuilding(GameObject building)
    {
        maxPopulation += building.GetComponent<BuildingInst>().population;
        maxJobs += building.GetComponent<BuildingInst>().jobs;
        money -= building.GetComponent<BuildingInst>().cost;
        buildings.Add(building.transform.position, building);

        SetText();
    }

    public void EndTurn()
    {
        if (turnsUntilWindChange > 0)
        {
            turnsUntilWindChange--;
        }

        if (turnsUntilWindChange == 0)
        {
            turnsUntilWindChange = Random.Range(3, 7);
            int i= Random.Range(0, 4);
            switch (i)
            {
                case 0:
                    {
                        windDirection = WindDirection.STILL;
                        break;
                    }
                case 1:
                    {
                        windDirection = WindDirection.NORTH;
                        break;
                    }
                case 2:
                    {
                        windDirection = WindDirection.SOUTH;
                        break;
                    }
                case 3:
                    {
                        windDirection = WindDirection.EAST;
                        break;
                    }
                case 4:
                    {
                        windDirection = WindDirection.WEST;
                        break;
                    }
            }
        }

        day++;
        CalculateMoney();
        CalculatePopulation();
        CalculateJobs();
        CalculateFood();

        SetText();

        foreach (var p in buildings.Values)
        {            
            p.GetComponent<BuildingInst>().EndTurn(curWater, curElectricity);
        }
        foreach (var p in pollutionClouds.Values)
        {
            p.GetComponent<Pollution>().EndTurn(windDirection, pollutionClouds);
        }
        foreach (var p in waterTiles.Values)
        {
            p.GetComponent<Water>().EndTurn(waterTiles);
        }
    }

    public void SetText()
    {
        statsText.text = string.Format("Day: {0}   Money: ${1}   Pop: {2} / {3}   Jobs: {4} / {5}   Food: {6}   Wind Direction: {7}",
            new object[8]
            {
                day,
                money,
                curPopulation,
                maxPopulation,
                curJobs,
                maxJobs,
                curFood,
                windDirection
            });
    }

    void CalculateMoney()
    {
        money += curJobs * incomePerJob;

        foreach(var b in buildings.Values)
            money -= b.GetComponent<BuildingInst>().costPerTurn;
    }

    void CalculatePopulation()
    {
        maxPopulation = 0;

        foreach (var b in buildings.Values)
            maxPopulation += b.GetComponent<BuildingInst>().maxPopulation;

        if (curFood >= curPopulation && curPopulation < maxPopulation)
        {
            curFood -= curPopulation / 4;
            curPopulation = Mathf.Min(curPopulation + (curFood / 4), maxPopulation);
        }
        else if(curFood < curPopulation)
        {
            curPopulation = curFood;
        }
    }

    void CalculateJobs()
    {
        curJobs = 0;
        maxJobs = 0;

        foreach (GameObject building in buildings.Values)
            maxJobs += building.GetComponent<BuildingInst>().maxJobs;

        curJobs = Mathf.Min(curPopulation, maxJobs);
    }

    void CalculateFood()
    {
        curFood = 0;

        foreach (GameObject building in buildings.Values)
            curFood += building.GetComponent<BuildingInst>().food;
    }

    public void RemoveBuilding(GameObject b)
    {
        buildings.Remove(b.transform.position);
    }
}