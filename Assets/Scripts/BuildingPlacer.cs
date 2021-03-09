using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    private bool currentlyPlacing;
    private bool currentlyDeleting;
    private BuildingPreset curBuildingPreset;

    private float placementIndicatorUpdateRate = 0.05f;
    private float lastUpdateTime;
    private Vector3 curPlacementPos;
    private Vector3 radius = new Vector3(0.2f, 0.2f, 0.2f);

    public GameObject placementIndicator;
    public GameObject deleteIndicator;

    public static BuildingPlacer inst;
    public List<Vector3> placedBuildings = new List<Vector3>();

    void Awake()
    {
        inst = this;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            CancelBuildingPlacement();

        if(Time.time - lastUpdateTime > placementIndicatorUpdateRate && currentlyPlacing)
        {
            lastUpdateTime = Time.time;

            curPlacementPos = Selector.inst.GetCurTilePosition();
            placementIndicator.transform.position = curPlacementPos;
        }

        else if (Time.time - lastUpdateTime > placementIndicatorUpdateRate && deleteIndicator)
        {
            lastUpdateTime = Time.time;

            curPlacementPos = Selector.inst.GetCurTilePosition();
            deleteIndicator.transform.position = curPlacementPos;
        }

        if (currentlyPlacing && 
            Input.GetMouseButtonDown(0) && 
            !CheckForBuilding() && 
            City.inst.money >= curBuildingPreset.cost)
        {
            PlaceBuilding();
        }

        else if (currentlyDeleting && Input.GetMouseButtonDown(0) && CheckForBuilding())
        {
            DeleteBuilding();
        }
    }

    public bool CheckForBuilding()
    {

        if (placedBuildings.Contains(curPlacementPos))
        {
            return true;
        }

        placedBuildings.Add(curPlacementPos);

        return false;
    }

    public void DeleteBuilding()
    {
        if (placedBuildings.Contains(deleteIndicator.transform.position))
        {
            GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");
            foreach (var b in buildings)
            {
                if (b.transform.position == deleteIndicator.transform.position)
                {
                    Destroy(b, 0);
                }
            }
        }
    }

    public void BeginNewDelete()
    {       
        currentlyPlacing = false;
        currentlyDeleting = true;
        deleteIndicator.SetActive(true);
        placementIndicator.SetActive(false);
    }

    public void BeginNewBuildingPlacement(BuildingPreset buildingPreset)
    {
        if(City.inst.money < buildingPreset.cost)
            return;

        currentlyPlacing = true;
        currentlyDeleting = false;
        curBuildingPreset = buildingPreset;
        placementIndicator.SetActive(true);
        deleteIndicator.SetActive(false);
    }

    public void CancelBuildingPlacement()
    {
        currentlyPlacing = false;
        placementIndicator.SetActive(false);
    }

    void PlaceBuilding()
    {
        GameObject buildingObj = Instantiate(curBuildingPreset.prefab, curPlacementPos, Quaternion.identity);
        City.inst.OnPlaceBuilding(curBuildingPreset);
    }
}