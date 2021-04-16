using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class City : MonoBehaviour
{
    public int money;
    public int day;
    public int curPopulation;
    public int curJobs;
    public int curFood;
    public int maxPopulation;
    public int maxJobs;
    public int incomePerJob;

    public TextMeshProUGUI statsText;

    [SerializeField]
    private List<GameObject> buildings = new List<GameObject>();

    public static City inst;

    void Awake()
    {
        inst = this;
    }

    public void OnPlaceBuilding(GameObject building)
    {
        maxPopulation += building.GetComponent<BuildingInst>().population;
        maxJobs += building.GetComponent<BuildingInst>().jobs;
        money -= building.GetComponent<BuildingInst>().cost;
        buildings.Add(building);

        SetText();
    }

    public void EndTurn()
    {
        day++;
        CalculateMoney();
        CalculatePopulation();
        CalculateJobs();
        CalculateFood();

        SetText();

        foreach (var p in GameObject.FindGameObjectsWithTag("pollution"))
        {
            p.GetComponent<Pollution>().EndTurn();
        }
    }

    public void SetText()
    {
        statsText.text = string.Format("Day: {0}   Money: ${1}   Pop: {2} / {3}   Jobs: {4} / {5}   Food: {6}", 
            new object[7] 
            {
                day,
                money,
                curPopulation,
                maxPopulation,
                curJobs,
                maxJobs,
                curFood
            });
    }

    void CalculateMoney()
    {
        money += curJobs * incomePerJob;

        foreach(GameObject building in buildings)
            money -= building.GetComponent<BuildingInst>().costPerTurn;
    }

    void CalculatePopulation()
    {
        maxPopulation = 0;

        foreach (GameObject building in buildings)
            maxPopulation += building.GetComponent<BuildingInst>().maxPopulation;

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

        foreach (GameObject building in buildings)
            maxJobs += building.GetComponent<BuildingInst>().maxJobs;

        curJobs = Mathf.Min(curPopulation, maxJobs);
    }

    void CalculateFood()
    {
        curFood = 0;

        foreach (GameObject building in buildings)
            curFood += building.GetComponent<BuildingInst>().food;
    }

    public void RemoveBuilding(GameObject b)
    {
        buildings.Remove(b);
    }
}