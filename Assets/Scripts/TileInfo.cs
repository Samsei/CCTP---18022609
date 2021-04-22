using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileInfo : MonoBehaviour
{
    [SerializeField] GameObject infoBar;
    Vector3 curTile;

    public TextMeshProUGUI statsText;
    public static TileInfo inst;
    BuildingInst instB;
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

        if (!tile.CompareTag("water") && !tile.CompareTag("ground"))
        {
            if (tile.GetComponent<BuildingInst>().maxPopulation > 0)
            {
                statsText.text = string.Format("Population: {0} / Max Population: {1} \nHappiness: {2}% \nCost per Turn: {3}\n",
                    new object[4]
                    {
                        tile.GetComponent<BuildingInst>().population,
                        tile.GetComponent<BuildingInst>().maxPopulation,
                        tile.GetComponent<BuildingInst>().happiness,
                        tile.GetComponent<BuildingInst>().costPerTurn,
                    });
            }
            else if (tile.GetComponent<BuildingInst>().maxFood > 0)
            {
                statsText.text = string.Format("Jobs: {0} / Max Jobs: {1} \nFood per Turn: {2} \nCost per Turn: {3}\n",
                    new object[4]
                    {
                        tile.GetComponent<BuildingInst>().jobs,
                        tile.GetComponent<BuildingInst>().maxJobs,
                        tile.GetComponent<BuildingInst>().food,
                        tile.GetComponent<BuildingInst>().costPerTurn,
                    });
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
