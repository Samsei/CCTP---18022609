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

    public int happiness = 75;
    public int garbageBuildup;

    public int pollutionPerTurn;

    private int tolerance;
    private int turnsWithoutWater;
    private int turnsWithoutElectricity;

    private RaycastHit hitObject;
    private GameObject hO;

    [SerializeField] GameObject warningIcon;
    [SerializeField] Sprite eWarning;
    [SerializeField] Sprite eCritical;
    [SerializeField] Sprite wWarning;
    [SerializeField] Sprite wCritical;

    public void EndTurn(float water, float uncleanWater, int electricity, int food)
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

        if (water > 0 && !gameObject.CompareTag("util") && gameObject.name != "Road(Clone)")
        {
            City.inst.water -= population / 2;
            happiness += population / 4;
        }

        if (water <= 0 && !gameObject.CompareTag("util") && gameObject.name != "Road(Clone)")
        {
            happiness -= population / 4;
            warningIcon.SetActive(true);
            if (turnsWithoutWater < 10)
            {
                warningIcon.GetComponent<SpriteRenderer>().sprite = wWarning;
                turnsWithoutWater++;
            }

            else if (turnsWithoutWater >= 10 && turnsWithoutWater < 20)
            {
                warningIcon.GetComponent<SpriteRenderer>().sprite = wCritical;
                turnsWithoutWater++;
            }
            else
            {
                AbandonBuilding();
            }
            Debug.Log(turnsWithoutWater);
        }

        if (electricity > 0 && !gameObject.CompareTag("util") && gameObject.name != "Road(Clone)")
        {
            City.inst.electricity -= population/4;
            happiness += population / 4;
        }

        if (electricity <= 0 && !gameObject.CompareTag("util") && gameObject.name != "Road(Clone)")
        {
            happiness -= population / 4;

            if (turnsWithoutElectricity < 10)
            {
                warningIcon.GetComponent<SpriteRenderer>().sprite = eWarning;
                turnsWithoutElectricity++;
            }

            else if (turnsWithoutElectricity >= 10 && turnsWithoutElectricity < 20)
            {
                warningIcon.GetComponent<SpriteRenderer>().sprite = eCritical;
                turnsWithoutElectricity++;
            }
            else
            {
                AbandonBuilding();
            }
            Debug.Log(turnsWithoutElectricity);

        }
    }

    void AbandonBuilding()
    {

    }
}
