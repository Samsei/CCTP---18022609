using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityBuilding : MonoBehaviour
{
    [SerializeField]public float waterProduction;
    [SerializeField] public float uncleanWaterProduction;
    public float electricityProduction;

    public float GetWaterProduction(Dictionary<Vector3, GameObject> waterTiles)
    {
        Vector3 offset = gameObject.transform.position + gameObject.transform.forward;
        if (waterTiles.ContainsKey(offset))
        {
            waterProduction = 100.0f - (waterTiles[offset].GetComponent<Water>().currentPollution / 256.0f) * 100.0f;
        }
        return waterProduction;
    }

    public float GetUncleanWaterProduction(Dictionary<Vector3, GameObject> waterTiles)
    {
        Vector3 offset = gameObject.transform.position + gameObject.transform.forward;
        if (waterTiles.ContainsKey(offset))
        {
            uncleanWaterProduction = (waterTiles[offset].GetComponent<Water>().currentPollution / 256.0f) * 100.0f;
        }
        return uncleanWaterProduction;
    }
}
