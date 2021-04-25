using System.Collections.Generic;
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

    int water = 1 << 8;
    int border = 1 << 10;

    [SerializeField] GameObject placementIndicator;
    [SerializeField] GameObject deleteIndicator;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject textPrefab;
    [SerializeField] Camera cam;

    RaycastHit hitObject;
    Vector3 raycastOffset = new Vector3(0, 0.1f, 0);
    Vector3 rayDirection = new Vector3(0, -1, 0);
    [SerializeField] Material placeMat;
    [SerializeField] Material desMat;

    public static BuildingPlacer inst;
    public Dictionary<Vector3, GameObject> placedBuildings = new Dictionary<Vector3, GameObject>();
    public Dictionary<Vector3, GameObject> waterTiles = new Dictionary<Vector3, GameObject>();

    GameObject hO;

    void Awake()
    {
        inst = this;
    }

    private void Start()
    {
        waterTiles = City.inst.waterTiles;
    }

    void Update()
    {
        Logic();
    }

    void Logic()
    {
        curPlacementPos = Selector.inst.GetCurTilePosition();

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            DisableText();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelBuildingPlacement();
        }

        else if (currentlyPlacing)
        {
            UpdatePlace();
        }

        else if (deleteIndicator)
        {
            UpdateDelete();
        }

        if (CanPlaceBuilding() && !CheckForBuilding())
        {
            PlaceBuilding();
        }

        else if (currentlyDeleting && Input.GetMouseButtonDown(0) && CheckForBuilding())
        {
            DeleteBuilding();
        }

        else if (!currentlyDeleting && !currentlyPlacing && Input.GetMouseButtonDown(2))
        {
            FindObjectOnTile();
        }

        else if (currentlyPlacing && Input.GetKeyDown(KeyCode.F))
        {
            placementIndicator.transform.Rotate(0, 90, 0);
            ChangeDirection();
        }
    }

    void UpdatePlace()
    {
        placementIndicator.transform.position = curPlacementPos;
    }

    void UpdateDelete()
    {
        deleteIndicator.transform.position = curPlacementPos;
    }

    void DisableText()
    {
        TileInfo.inst.Close();
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

    bool CanPlaceBuilding()
    {
        if (currentlyPlacing &&
            Input.GetMouseButtonDown(0) &&           
            City.inst.money >= curBuildingPreset.cost &&
            !OverWater())
        {
            return true;
        }
        return false;
    }

    void FindObjectOnTile()
    {
        curPlacementPos = Selector.inst.GetCurTilePosition();

        if (placedBuildings.ContainsKey(curPlacementPos))
        {
            TileInfo.inst.OpenInfoBar(placedBuildings[curPlacementPos]);
        }
        else if (OverWater())
        {
            TileInfo.inst.OpenInfoBar(hO);
        }
        else if (OverGround())
        {
            TileInfo.inst.OpenInfoBar(hO);
        }
        return;
    }

    bool OverWater()
    {
        if (Physics.Raycast(curPlacementPos + raycastOffset, rayDirection, out hitObject, 2,  water))
        {
            hO = hitObject.transform.gameObject;
            return true;
        }
        if (Physics.Raycast(curPlacementPos + raycastOffset, rayDirection, out hitObject, 2, border))
        {
            hO = hitObject.transform.gameObject;
            return true;
        }

        else if (!Physics.Raycast(curPlacementPos + raycastOffset, rayDirection, 2))
        {
            return true;
        }

        return false;
    }

    bool OverGround()
    {
        int layerMask = 1 << 9;
        if (Physics.Raycast(curPlacementPos + raycastOffset, rayDirection, out hitObject, 2, layerMask))
        {
            hO = hitObject.transform.gameObject;
            return true;
        }

        return false;
    }

    public bool CheckForBuilding()
    {
        if (placedBuildings.ContainsKey(new Vector3(curPlacementPos.x, curPlacementPos.y, curPlacementPos.z)))
        {
            return true;
        }

        else if (curBuildingPreset.prefab.gameObject.name != "Road" && !CheckForRoad())
        {
            return true;
        }

        return false;
    }

    bool CheckForRoad()
    {
        GetForwardPos();
        if (curBuildingPreset.name == "Forest" && OverGround())
        {
            return true;
        }
        else if (curBuildingPreset.name == "WaterPump" && waterTiles.ContainsKey(vec) && waterTiles.ContainsKey(vec))
        {
            return true;
        }
        else if (placedBuildings.ContainsKey(vec) && placedBuildings[vec].name == "Road(Clone)" && curBuildingPreset.name != "WaterPump" && curBuildingPreset.name != "Forest")
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
    }
    
    public void DeleteBuilding()
    {
        if (placedBuildings.ContainsKey(curPlacementPos))
        {
            City.inst.RemoveBuilding(placedBuildings[curPlacementPos]);
            GameObject t = placedBuildings[curPlacementPos];

            placedBuildings.Remove(curPlacementPos);
            Destroy(t, 0);
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
        GameObject buildingObj = Instantiate(curBuildingPreset.prefab, curPlacementPos, Quaternion.identity);
        buildingObj.transform.rotation = placementIndicator.transform.rotation;
        BuildingInst bi = buildingObj.GetComponent<BuildingInst>();

        bi.cost = curBuildingPreset.cost;
        bi.costPerTurn = curBuildingPreset.costPerTurn;
        bi.maxFood = curBuildingPreset.food;
        bi.maxJobs = curBuildingPreset.jobs;
        bi.maxPopulation = curBuildingPreset.population;
        if (bi.maxPopulation > 0 && !City.inst.firstHouse)
        {
            bi.population = bi.maxPopulation;
            City.inst.firstHouse = true;
        }

        bi.pollutionPerTurn = curBuildingPreset.pollution;
        bi.waterConsumption = curBuildingPreset.waterConsumption;
        bi.electricityConsumption = curBuildingPreset.electricityConsumption;

        City.inst.OnPlaceBuilding(buildingObj);

        placedBuildings.Add(buildingObj.transform.position, buildingObj);

        GameObject text = Instantiate(textPrefab, Input.mousePosition, Quaternion.identity, canvas.transform);
        text.GetComponent<Text>().text = string.Format("-${0}", curBuildingPreset.cost);       
    }
}