using TMPro;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    [SerializeField] GameObject infoBar;
    Vector3 curTile;

    public TextMeshProUGUI statsText;
    public static TileInfo inst;
    BuildingInst instB;
    Ray ray;
    GameObject hO;
    RaycastHit hitObject;
    int mask = 1 << 11;

    private void Start()
    {
        inst = this;
    }

    public void Close()
    {
        infoBar.SetActive(false);
    }

    public void OpenInfoBar(GameObject tile)
    {
        infoBar.SetActive(true);
        ray.origin = tile.transform.position;
        ray.direction = new Vector3(0, 1, 0);
        if (Physics.Raycast(ray, out hitObject, 5, mask))
        {
            hO = hitObject.transform.gameObject;
        }

        if (!tile.CompareTag("water") && !tile.CompareTag("ground"))
        {
            if (tile.GetComponent<BuildingInst>().maxPopulation > 0)
            {
                statsText.text = string.Format("Population: {0} / Max Population: {1} \nHappiness: {2}% \nAir pollution: {3}% \nFood consumption per turn {4}:\nWater consumption per turn: {5}\nPopulation that is ill: {6}\nCost per Turn: {7}\n",
                    new object[8]
                    {
                        tile.GetComponent<BuildingInst>().population,
                        tile.GetComponent<BuildingInst>().maxPopulation,
                        tile.GetComponent<BuildingInst>().happiness,
                        (hO.GetComponent<Pollution>().currentPollution / 256 * 100),
                        tile.GetComponent<BuildingInst>().foodConsumption,
                        tile.GetComponent<BuildingInst>().waterConsumption,
                        tile.GetComponent<BuildingInst>().populationIsIll,
                        tile.GetComponent<BuildingInst>().costPerTurn,
                    });
            }
            else if (tile.GetComponent<BuildingInst>().maxFood > 0)
            {
                statsText.text = string.Format("Jobs: {0} / Max Jobs: {1} \nFood per Turn: {2} \nAir pollution: {3}% \nWater consumption per turn: {4}\nCost per Turn: {5}\n",
                    new object[6]
                    {
                        tile.GetComponent<BuildingInst>().jobs,
                        tile.GetComponent<BuildingInst>().maxJobs,
                        tile.GetComponent<BuildingInst>().food,
                        (hO.GetComponent<Pollution>().currentPollution / 256 * 100),
                        tile.GetComponent<BuildingInst>().waterConsumption,
                        tile.GetComponent<BuildingInst>().costPerTurn,
                    });
            }
            else if (tile.name == "Factory(Clone)")
            {           
                statsText.text = string.Format("Jobs: {0} / Max Jobs: {1} \nPollution per Turn: {2} \nAir pollution: {3}% \nWater consumption per turn: {4}\nCost per Turn: {5}\n",
                    new object[6]
                    {
                        tile.GetComponent<BuildingInst>().jobs,
                        tile.GetComponent<BuildingInst>().maxJobs,
                        tile.GetComponent<BuildingInst>().pollutionPerTurn,
                        (hO.GetComponent<Pollution>().currentPollution / 256 * 100),
                        tile.GetComponent<BuildingInst>().waterConsumption,
                        tile.GetComponent<BuildingInst>().costPerTurn,
                    });
            }

            else if (tile.name == "NuclearStation(Clone)")
            {
                statsText.text = string.Format("Jobs: {0} / Max Jobs: {1} \nPollution per Turn: {2} \nAir pollution: {3}% \nElectricity production per turn: {4}\nWater consumption per turn: {5}\nCost per Turn: {6}\n",
                    new object[7]
                    {
                        tile.GetComponent<BuildingInst>().jobs,
                        tile.GetComponent<BuildingInst>().maxJobs,
                        tile.GetComponent<BuildingInst>().pollutionPerTurn,
                        (hO.GetComponent<Pollution>().currentPollution / 256 * 100),
                        tile.GetComponent<UtilityBuilding>().electricityProduction,
                        tile.GetComponent<BuildingInst>().waterConsumption,
                        tile.GetComponent<BuildingInst>().costPerTurn,
                    });
            }
            else if (tile.CompareTag("util"))
            {
                if (tile.name == "WaterPump(Clone)")
                {
                    statsText.text = string.Format("Clean water production: {0}\nUnclean water production: {1}\n Air pollution: {2}%",
                            new object[3]
                            {
                                tile.GetComponent<UtilityBuilding>().waterProduction,
                                tile.GetComponent<UtilityBuilding>().uncleanWaterProduction,
                                hO.GetComponent<Pollution>().currentPollution / 256 * 100
                            });
                }

                else if (tile.name == "SolarPanel(Clone)")
                {
                    statsText.text = string.Format("Energy production: {0}\nProduction efficiency: {1}\n Air pollution: {2}%",
                            new object[3]
                            {
                                tile.GetComponent<UtilityBuilding>().electricityProduction,
                                100 - (hO.GetComponent<Pollution>().currentPollution / 256 / 2 * 100),
                                hO.GetComponent<Pollution>().currentPollution / 256 * 100
                            });
                }
            }

            else if (tile.name == "Road(Clone)")
            {
                statsText.text = string.Format("Air pollution: {0}%",
                        new object[1]
                        {                               
                                hO.GetComponent<Pollution>().currentPollution / 256 * 100
                        });
            }

            else if (tile.name == "Forest(Clone)")
            {
                statsText.text = string.Format("Reduces air pollution by: 5% per turn");
            }
        }

        else if (tile.CompareTag("ground"))
        {
            statsText.text = string.Format("Air pollution: {0}%",
                    new object[1]
                    {
                        (tile.GetComponentInChildren<Pollution>().currentPollution / 256 * 100)
                    });
        }

        else if (tile.CompareTag("water"))
        {
            statsText.text = string.Format("Air pollution: {0}%\n Water pollution: {1}%",
                    new object[2]
                    {
                        (tile.GetComponentInChildren<Pollution>().currentPollution / 256) * 100,
                        (tile.GetComponent<Water>().currentPollution / 256) * 100
                    });
        }       
    }
}
