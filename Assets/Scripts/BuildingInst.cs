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
    public int waterConsumption;
    public int electricityConsumption;
    public float food;
    public int population;
    public int populationIsIll;
    public int jobs;
    public int happiness = 75;
    public float pollutionPerTurn;
    public float pollutionPTMax;
    public int foodConsumption;

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
    private bool isIllFromWater = false;
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

    private void Start()
    {
        pollutionPTMax = pollutionPerTurn;
    }

    public void EndTurn(float water, float uncleanWater, float electricity, float curFood)
    {
        if (!isAbandoned && name != "Forest(Clone)")
        {
            StopAllCoroutines();
            warningIcon.SetActive(false);

            if (pollutionPerTurn != 0)
            {
                ray.origin = gameObject.transform.position;
                ray.direction = new Vector3(0, 1, 0);
                if (Physics.Raycast(ray, out hitObject))
                {
                    hO = hitObject.transform.gameObject;
                    hO.GetComponent<Pollution>().currentPollution += pollutionPerTurn;
                }

                if (pollutionPTMax > 0)
                pollutionPerTurn = pollutionPTMax * (0.5f + jobs / (maxJobs * 2));
            }

            if (maxFood > 0)
            {
                ray.origin = gameObject.transform.position;
                ray.direction = new Vector3(0, 1, 0);
                if (Physics.Raycast(ray, out hitObject))
                {
                    hO = hitObject.transform.gameObject;
                    if (water > waterConsumption)
                    {
                        if (jobs > 0)
                        {
                            food = 10.0f + (2*jobs) * (1.0f - (hO.GetComponent<Pollution>().currentPollution / 256.0f / 2.0f));
                        }

                        else
                        {
                            food = 5;
                        } 
                    }
                    else
                    {
                        food = 10.0f + (2 * jobs) * (1.0f - (hO.GetComponent<Pollution>().currentPollution / 256.0f / 2.0f)) / 2.0f;
                        if (food < 2.5f)
                        {
                            food = 2.5f;
                        }
                    }
                }
            }

            if (gameObject.name == "House(Clone)")
            {
                Debug.Log(isIll);
                Debug.Log(isIllFromWater);
                Debug.Log(turnsIll);

                ray.origin = gameObject.transform.position;
                ray.direction = new Vector3(0, 1, 0);
                if (Physics.Raycast(ray, out hitObject))
                {
                    hO = hitObject.transform.gameObject;
                    if (hO.GetComponent<Pollution>().currentPollution / 256.0f > 0.35f)
                    {
                        isIll = true;
                        warningIcon.SetActive(true);
                        turnsIll++;
                    }

                    else if (hO.GetComponent<Pollution>().currentPollution / 256.0f < 0.35f)
                    {
                        isIll = false;
                    }
                }

                foodConsumption = population;
            }

            if (gameObject.name == "SolarPanel(Clone)")
            {
                ray.origin = gameObject.transform.position;
                ray.direction = new Vector3(0, 1, 0);
                if (Physics.Raycast(ray, out hitObject))
                {
                    hO = hitObject.transform.gameObject;
                    GetComponent<UtilityBuilding>().electricityProduction = (200 * (1 - hO.GetComponent<Pollution>().currentPollution / 256 / 2));
                }
            }

            if (gameObject.name == "NuclearStation(Clone)")
            {
                GetComponent<UtilityBuilding>().electricityProduction = 400.0f * (0.5f + jobs / (maxJobs * 2));
            }

            if (uncleanWater > waterConsumption && gameObject.name == "House(Clone)")
            {
                City.inst.uncleanWater -= waterConsumption;
                happiness -= population / 8;
                isIllFromWater = true;
                turnsIll++;
                warningIcon.SetActive(true);         
            }

            else if (water > waterConsumption && !gameObject.CompareTag("util") && gameObject.name != "Road(Clone)")
            {
                City.inst.water -= waterConsumption;
                happiness += population / 4;
                isIllFromWater = false;
                noWater = false;
                turnsWithoutWater = 0;
            }

            else if (water < waterConsumption && uncleanWater <= waterConsumption && !gameObject.CompareTag("util") && gameObject.name != "Road(Clone)")
            {
                happiness -= population / 4;
                noWater = true;
                turnsWithoutWater++;
                warningIcon.SetActive(true);
            }

            if (curFood > foodConsumption && gameObject.name == "House(Clone)")
            {
                if (population < maxPopulation)
                {
                    population += 1;
                }

                happiness += population / 4;
                City.inst.food -= foodConsumption;
                noFood = false;
                turnsWithoutFood = 0;
            }

            else if (curFood < foodConsumption && gameObject.name == "House(Clone)")
            {
                happiness -= population;
                noFood = true;
                turnsWithoutFood++;
                warningIcon.SetActive(true);
            }

            if (electricity > electricityConsumption && gameObject.name != "Road(Clone)")
            {
                City.inst.electricity -= electricityConsumption;
                happiness += population / 4;
                noElectric = false;
                turnsWithoutElectricity = 0;
            }

            else if (electricity < electricityConsumption && gameObject.name != "Road(Clone)")
            {
                happiness -= population / 4;
                noElectric = true;
                turnsWithoutElectricity++;
                warningIcon.SetActive(true);
            }

            if (water > waterConsumption && electricity > electricityConsumption && !isIll && curFood > foodConsumption && !isIllFromWater)
            {
                StopAllCoroutines();
                warningIcon.SetActive(false);
            }

            SetWarningSymbol();

            if (!isIll && !isIllFromWater)
            {
                if (populationIsIll > 0)
                {
                    populationIsIll--;
                }
                else
                {
                    turnsIll = 0;
                }
            }
        }

        else if (name != "Forest(Clone)")
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

        if (isIll || isIllFromWater)
        {
            populationIsIll++;
            if (populationIsIll > population)
            {
                populationIsIll = population;
            }

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

        if (isIllFromWater && !isAbandoned)
        {
            warningIcon.GetComponent<SpriteRenderer>().sprite = spriteArray[3];
            yield return new WaitForSeconds(2);
        }

        if ((noWater || noElectric || noFood || isIll || isIllFromWater) && !isAbandoned)
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
