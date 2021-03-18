using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacer : MonoBehaviour
{
    private bool currentlyPlacing;
    private bool currentlyDeleting;
    private BuildingPreset curBuildingPreset;

    private float placementIndicatorUpdateRate = 0.05f;
    private float lastUpdateTime;
    private Vector3 curPlacementPos;
    private Vector3 radius = new Vector3(0.2f, 0.2f, 0.2f);
    private Vector3 vec;

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

        if (Time.time - lastUpdateTime > placementIndicatorUpdateRate && currentlyPlacing)
        {
            UpdatePlace();
        }

        else if (Time.time - lastUpdateTime > placementIndicatorUpdateRate && deleteIndicator)
        {
            UpdateDelete();
        }

        if (CanPlace())
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
        }
    }

    bool CanPlace()
    {
        if (currentlyPlacing &&
            Input.GetMouseButtonDown(0) &&
            !CheckForBuilding() &&
            City.inst.money >= curBuildingPreset.cost &&
            curPlacementPos.y >= -0.5f)
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
            Debug.Log("There is no road");
            return true;
        }

        placedBuildings.Add(curPlacementPos, curBuildingPreset.prefab.gameObject.name);
        return false;
    }

    bool CheckForRoad()
    {
        GetForwardPos();
        Debug.Log("Function Called");
        if (placedBuildings.ContainsKey(vec) && placedBuildings[vec].ToString() == "Road")
        {
            Debug.Log("Contains Road");
            return true;
        }
        return false;
    }

    void GetForwardPos()
    {
        switch (placementIndicator.transform.rotation.y)
        {
            case 0:
                vec = new Vector3(curPlacementPos.x, curPlacementPos.y, curPlacementPos.z + 1);
                break;
            case 90:
                vec = new Vector3(curPlacementPos.x + 1, curPlacementPos.y, curPlacementPos.z);
                break;
            case 180:
                vec = new Vector3(curPlacementPos.x, curPlacementPos.y, curPlacementPos.z - 1);
                break;
            case -90:
                vec = new Vector3(curPlacementPos.x - 1, curPlacementPos.y, curPlacementPos.z);
                break;
            default:
                break;
        }
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