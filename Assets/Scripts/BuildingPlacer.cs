﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacer : MonoBehaviour
{
    private bool currentlyPlacing;
    private bool currentlyDeleting;
    private BuildingPreset curBuildingPreset;

    private float lastUpdateTime;
    private Vector3 curPlacementPos;
    private Vector3 radius = new Vector3(0.2f, 0.2f, 0.2f);
    private Vector3 vec;
    private string direction = "South";
    private int dirNum = 0;

    [SerializeField] GameObject placementIndicator;
    [SerializeField] GameObject deleteIndicator;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject textPrefab;
    [SerializeField] Camera cam;

    [SerializeField] Material placeMat;
    [SerializeField] Material desMat;

    public static BuildingPlacer inst;
    public Dictionary<Vector3, string> placedBuildings = new Dictionary<Vector3, string>();

    void Awake()
    {
        inst = this;
    }

    void Update()
    {
        Logic();
    }

    void Logic()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelBuildingPlacement();

        if (currentlyPlacing)
        {
            UpdatePlace();
        }

        else if (deleteIndicator)
        {
            UpdateDelete();
        }

        if (CanPlace() && !CheckForBuilding())
        {
            PlaceBuilding();
        }

        else if (currentlyDeleting && Input.GetMouseButtonDown(0) && CheckForBuilding())
        {
            DeleteBuilding();
        }

        else if (currentlyPlacing && Input.GetKeyDown(KeyCode.F))
        {
            placementIndicator.transform.Rotate(0, 90, 0);
            ChangeDirection();
        }

    }

    void ChangeDirection()
    {
        dirNum++;
        if (dirNum > 3)
        {
            dirNum = 0;
        }

        switch (dirNum)
        {
            case 0:
                direction = "South";
                    break;
            case 1:
                direction = "West";
                break;
            case 2:
                direction = "North";
                break;
            case 3:
                direction = "East";
                break;
            default:
                break;
        }       
    }

    bool CanPlace()
    {
        if (currentlyPlacing &&
            Input.GetMouseButtonDown(0) &&           
            City.inst.money >= curBuildingPreset.cost &&
            curPlacementPos.y >= -0.5f &&
            !OverWater())
        {
            return true;
        }
        return false;
    }

    bool OverWater()
    {
        int layerMask = 1 << 8;

        if (Physics.Raycast(placementIndicator.transform.position, new Vector3(0, -2, 0), 2, layerMask))
        {
            return true;
        }

        return false;
    }

    public bool CheckForBuilding()
    {
        if (placedBuildings.ContainsKey(curPlacementPos))
        {
            return true;
        }

        else if (curBuildingPreset.prefab.gameObject.name != "Road" && !CheckForRoad())
        {
            return true;
        }

        placedBuildings.Add(curPlacementPos, curBuildingPreset.prefab.gameObject.name);
        return false;
    }

    bool CheckForRoad()
    {
        GetForwardPos();
        Debug.Log(placementIndicator.transform.localRotation.y);
        if (placedBuildings.ContainsKey(vec) && placedBuildings[vec].ToString() == "Road")
        {
            return true;
        }
        return false;
    }

    void GetForwardPos()
    {
        switch (direction)
        {
            case "South":
                vec = new Vector3(curPlacementPos.x, curPlacementPos.y, curPlacementPos.z + 1);
                break;
            case "West":
                vec = new Vector3(curPlacementPos.x + 1, curPlacementPos.y, curPlacementPos.z);
                break;
            case "North":
                vec = new Vector3(curPlacementPos.x, curPlacementPos.y, curPlacementPos.z - 1);
                break;
            case "East":
                vec = new Vector3(curPlacementPos.x - 1, curPlacementPos.y, curPlacementPos.z);
                break;
            default:
                vec = new Vector3(0, 1, 0);
                break;
        }
        Debug.Log(vec);
    }

    void UpdatePlace()
    {
        lastUpdateTime = Time.time;

        curPlacementPos = Selector.inst.GetCurTilePosition();
        placementIndicator.transform.position = curPlacementPos;       
    }

    void UpdateDelete()
    {
        lastUpdateTime = Time.time;

        curPlacementPos = Selector.inst.GetCurTilePosition();
        deleteIndicator.transform.position = curPlacementPos;
    }

    public void DeleteBuilding()
    {
        if (placedBuildings.ContainsKey(deleteIndicator.transform.position))
        {
            GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");
            foreach (var b in buildings)
            {
                if (b.transform.position == deleteIndicator.transform.position)
                {
                    //City.inst.RemoveBuilding(b.GetComponent<BuildingPreset>());
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
        if (City.inst.money < buildingPreset.cost)
        {
            return;
        }

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
        currentlyDeleting = false;
        deleteIndicator.SetActive(false);
    }

    void PlaceBuilding()
    {
        GameObject buildingObj = Instantiate(curBuildingPreset.prefab, curPlacementPos - new Vector3( 0, 0.1f, 0), Quaternion.identity);
        buildingObj.transform.rotation = placementIndicator.transform.rotation;
        City.inst.OnPlaceBuilding(curBuildingPreset);

        GameObject text = Instantiate(textPrefab, Input.mousePosition, Quaternion.identity, canvas.transform);
        text.GetComponent<Text>().text = string.Format("-${0}", curBuildingPreset.cost);       
    }
}