using System.Collections;
using UnityEngine;

public class BuildingInst : MonoBehaviour
{
    public string displayName;

    public int cost;
    public int costPerTurn;

    public float maxFood;
    public int maxPopulation;
    public int maxJobs;

    public float food;
    public int population;
    public int jobs;

    public int happiness = 75;

    public int pollutionPerTurn;

    private int turnsWithoutWater;
    private int turnsWithoutElectricity;
    private int turnsWithoutFood;
    private int turnsIll;

    private RaycastHit hitObject;
    private GameObject hO;
    private Ray ray;

    private bool noWater = false;
    private bool noElectric = false;
    private bool noFood = false;
    private bool isIll = false;
    private bool isAbandoned = false;

    [SerializeField] GameObject warningIcon;
    [SerializeField] Sprite[] spriteArray;
    [SerializeField] Sprite electricWarning;
    [SerializeField] Sprite electricCritical;
    [SerializeField] Sprite waterWarning;
    [SerializeField] Sprite waterCritical;
    [SerializeField] Sprite foodWarning;
    [SerializeField] Sprite foodCritical;
    [SerializeField] Sprite illWarning;
    [SerializeField] Sprite illCritical;
    [SerializeField] Sprite abandoned;

    public void EndTurn(float water, float uncleanWater, float electricity, float curFood)
    {
        if (!isAbandoned)
        {
            StopAllCoroutines();

            if (pollutionPerTurn > 0)
            {
                ray.origin = gameObject.transform.position;
                ray.direction = new Vector3(0, 1, 0);
                if (Physics.Raycast(ray, out hitObject))
                {
                    hO = hitObject.transform.gameObject;
                    hO.GetComponent<Pollution>().currentPollution += pollutionPerTurn;
                }
            }

            if (maxFood > 0)
            {
                ray.origin = gameObject.transform.position;
                ray.direction = new Vector3(0, 1, 0);
                if (Physics.Raycast(ray, out hitObject))
                {
                    hO = hitObject.transform.gameObject;
                    food = maxFood * (1.0f - (hO.GetComponent<Pollution>().currentPollution / 256.0f / 2));
                }
            }

            if (gameObject.name == "House(Clone)")
            {
                ray.origin = gameObject.transform.position;
                ray.direction = new Vector3(0, 1, 0);
                if (Physics.Raycast(ray, out hitObject))
                {
                    hO = hitObject.transform.gameObject;
                    if (hO.GetComponent<Pollution>().currentPollution / 256 > 0.35f)
                    {
                        isIll = true;
                    }
                    else
                    {
                        isIll = false;
                    }
                }
            }

            if (gameObject.name == "SolarPanel(Clone)")
            {
                ray.origin = gameObject.transform.position;
                ray.direction = new Vector3(0, 1, 0);
                if (Physics.Raycast(ray, out hitObject))
                {
                    Debug.Log("Test");
                    hO = hitObject.transform.gameObject;
                    GetComponent<UtilityBuilding>().electricityProduction = (200 * (1 - hO.GetComponent<Pollution>().currentPollution / 256 / 2));
                }
            }

            if (water > 0 && !gameObject.CompareTag("util") && gameObject.name != "Road(Clone)")
            {
                City.inst.water -= population / 2;
                happiness += population / 4;
                noWater = false;
            }

            else if (uncleanWater > 0 && water < 0 && gameObject.name == "House(Clone)")
            {
                City.inst.uncleanWater -= population / 2;
                happiness -= population / 8;
                isIll = true;
                warningIcon.SetActive(true);
            }

            else if (water <= 0 && uncleanWater <= 0 && !gameObject.CompareTag("util") && gameObject.name != "Road(Clone)")
            {
                happiness -= population / 4;
                noWater = true;
                turnsWithoutWater++;
                warningIcon.SetActive(true);
            }

            if (curFood > 0 && gameObject.name == "House(Clone)")
            {
                if (population < maxPopulation)
                {
                    population += 1;
                }

                happiness += population / 4;
                City.inst.food -= population / 2;
                noFood = false;
            }

            else if (curFood <= 0 && gameObject.name == "House(Clone)")
            {
                happiness -= population / 4;
                noFood = true;
                turnsWithoutFood++;
                warningIcon.SetActive(true);
            }

            if (electricity > 0 && !gameObject.CompareTag("util") && gameObject.name != "Road(Clone)")
            {
                City.inst.electricity -= population / 4;
                happiness += population / 4;
                noElectric = false;
            }

            else if (electricity <= 0 && !gameObject.CompareTag("util") && gameObject.name != "Road(Clone)")
            {
                happiness -= population / 4;
                noElectric = true;
                turnsWithoutElectricity++;
                warningIcon.SetActive(true);
            }

            if (water > 0 && electricity > 0 && !isIll && curFood > 0)
            {
                StopAllCoroutines();
                warningIcon.SetActive(false);
            }

            SetWarningSymbol();
        }

        else
        {
            warningIcon.GetComponent<SpriteRenderer>().sprite = abandoned;
        }
    }

    void SetWarningSymbol()
    {
        if (noWater)
        {
            if (turnsWithoutWater < 10)
            {
                spriteArray[0] = waterWarning;
            }

            else if (turnsWithoutWater >= 10 && turnsWithoutWater < 20)
            {
                spriteArray[0] = waterCritical;
            }
            else
            {
                AbandonBuilding();
            }
        }

        if (noElectric)
        {
            if (turnsWithoutElectricity < 10)
            {
                spriteArray[1] = electricWarning;
            }

            else if (turnsWithoutElectricity >= 10 && turnsWithoutElectricity < 20)
            {
                spriteArray[1] = electricCritical;
            }
            else
            {
                AbandonBuilding();
            }
        }

        if (noFood)
        {
            if (turnsWithoutFood < 10)
            {
                spriteArray[2] = foodWarning;
            }

            else if (turnsWithoutFood >= 10 && turnsWithoutFood < 20)
            {
                spriteArray[2] = foodCritical;
            }
            else
            {
                AbandonBuilding();
            }
        }

        if (isIll)
        {
            if (turnsIll < 10)
            {
                spriteArray[3] = illWarning;
            }

            else if (turnsIll >= 10 && turnsIll < 20)
            {
                spriteArray[2] = illCritical;
            }
            else
            {
                AbandonBuilding();
            }
        }

        StartCoroutine(symbolSwap());
    }

    IEnumerator symbolSwap()
    {
        if (noWater && !isAbandoned)
        {
            warningIcon.GetComponent<SpriteRenderer>().sprite = spriteArray[0];
            yield return new WaitForSeconds(2);
        }

        if (noElectric && !isAbandoned)
        {
            warningIcon.GetComponent<SpriteRenderer>().sprite = spriteArray[1];
            yield return new WaitForSeconds(2);
        }

        if (noFood && !isAbandoned)
        {
            warningIcon.GetComponent<SpriteRenderer>().sprite = spriteArray[2];
            yield return new WaitForSeconds(2);
        }

        if (isIll && !isAbandoned)
        {
            warningIcon.GetComponent<SpriteRenderer>().sprite = spriteArray[3];
            yield return new WaitForSeconds(2);
        }

        if ((noWater || noElectric || noFood || isIll) && !isAbandoned)
        {
            StartCoroutine(symbolSwap());
        }
    }

    void AbandonBuilding()
    {
        isAbandoned = true;
        StopAllCoroutines();

        costPerTurn = 0;
        maxFood = 0;
        maxPopulation = 0;
        maxJobs = 0;
        food = 0;
        population = 0;
        jobs = 0;
        happiness = 0;
        pollutionPerTurn = 0;    
    }
}
