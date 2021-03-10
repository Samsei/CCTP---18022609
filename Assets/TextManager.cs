using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public GameObject houseBar;
    public GameObject jobBar;
    public GameObject factoryBar;

    public void ShowHouses()
    {
        if (!houseBar.activeSelf)
        {
            if (jobBar.activeSelf)
                jobBar.SetActive(false);
            if (factoryBar.activeSelf)
                factoryBar.SetActive(false);

            houseBar.SetActive(true);
        }       
    }

    public void ShowJobBuildings()
    {
        if (!jobBar.activeSelf)
        {
            if (houseBar.activeSelf)
                houseBar.SetActive(false);
            if (factoryBar.activeSelf)
                factoryBar.SetActive(false);

            jobBar.SetActive(true);
        }
    }

    public void ShowFactoryBuildings()
    {
        if (!factoryBar.activeSelf)
        {
            if (jobBar.activeSelf)
                jobBar.SetActive(false);
            if (houseBar.activeSelf)
                houseBar.SetActive(false);

            factoryBar.SetActive(true);
        }
    }

    public void OnRoadButton()
    {
        if (jobBar.activeSelf)
            jobBar.SetActive(false);
        if (houseBar.activeSelf)
            houseBar.SetActive(false);
        if (factoryBar.activeSelf)
            factoryBar.SetActive(false);
    }
}
