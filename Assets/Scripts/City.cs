using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class City : MonoBehaviour
{
    public int money;
    private int day;
    private int population;
    private int jobs;
    [SerializeField] public float food;
    public float water;
    public float uncleanWater;
    public float electricity;
    private int maxPopulation;
    private int maxJobs;
    private int incomePerJob = 5;
    private int turnsUntilWindChange = 5;

    public TextMeshProUGUI statsText;
    public Dictionary<Vector3, GameObject> buildings = new Dictionary<Vector3, GameObject>();
    public Dictionary<Vector3, GameObject> pollutionClouds = new Dictionary<Vector3, GameObject>();
    public Dictionary<Vector3, GameObject> waterTiles = new Dictionary<Vector3, GameObject>();

    public static City inst;
    public GameObject pauseUI;

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
        CalculateUtilities();
        CalculateMoney();
        CalculateFood();
        CalculatePopulation();
        CalculateJobs();

        SetText();

        foreach (var p in buildings.Values)
        {            
            p.GetComponent<BuildingInst>().EndTurn(water, uncleanWater, electricity, food);
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

    private void CalculateUtilities()
    {
        water = 0;
        uncleanWater = 0;
        electricity = 0;
        foreach (var b in GameObject.FindGameObjectsWithTag("util"))
        {
            water += b.GetComponent<UtilityBuilding>().GetWaterProduction(waterTiles);
            uncleanWater += b.GetComponent<UtilityBuilding>().GetUncleanWaterProduction(waterTiles);
            electricity += b.GetComponent<UtilityBuilding>().electricityProduction;
        }
    }

    public void SetText()
    {
        statsText.text = string.Format("Day: {0}   Money: ${1}   Pop: {2} / {3}   Jobs: {4} / {5}   Food: {6}   Wind Direction: {7}",
            new object[8]
            {
                day,
                money,
                population,
                maxPopulation,
                jobs,
                maxJobs,
                food,
                windDirection
            });
    }

    void CalculateMoney()
    {
        money += jobs * incomePerJob;

        foreach (var b in buildings.Values)
        {
            money -= b.GetComponent<BuildingInst>().costPerTurn;
        }
    }

    void CalculatePopulation()
    {
        maxPopulation = 0;
        population = 0;
        foreach (var b in buildings.Values)
        {
            maxPopulation += b.GetComponent<BuildingInst>().maxPopulation;
            population += b.GetComponent<BuildingInst>().population;
        }
    }

    void CalculateJobs()
    {
        jobs = 0;
        maxJobs = 0;

        foreach (GameObject building in buildings.Values)
        {
            maxJobs += building.GetComponent<BuildingInst>().maxJobs;
            jobs += building.GetComponent<BuildingInst>().jobs;           
        }

        foreach (GameObject building in buildings.Values)
        {
            if (population > 0 && jobs < maxJobs &&
                building.GetComponent<BuildingInst>().maxJobs > building.GetComponent<BuildingInst>().jobs &&
                population > jobs)
            {
                building.GetComponent<BuildingInst>().jobs++;
            }
        }
    }

    void CalculateFood()
    {
        food = 0;

        foreach (GameObject building in buildings.Values)
        {
            food += building.GetComponent<BuildingInst>().food;
        }
    }

    public void RemoveBuilding(GameObject b)
    {
        buildings.Remove(b.transform.position);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            pauseUI.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void Unpause()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene("Title");
    }
}