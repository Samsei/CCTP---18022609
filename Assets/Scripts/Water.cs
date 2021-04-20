using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public class Water : MonoBehaviour
{
    public float currentPollution = 0.0f;

    private float maxPollution = 256.0f;

    float r = 1.0f;
    float g = 1.0f;
    float b = 1.0f;

    private Renderer _renderer;
    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }

    public enum Direction
    {
        STILL,
        NORTH,
        SOUTH,
        EAST,
        WEST
    }

    public Direction direction;

    private void Start()
    {
        if (direction != Direction.STILL)
        {
            GetComponentInChildren<WaterArrow>().SetDirection(direction);
        }
    }

    public void EndTurn(Dictionary<Vector3, GameObject> waterTiles)
    {
        if (currentPollution > maxPollution)
        {
            currentPollution = maxPollution;
        }

        //currentPollution -= 0.1f;

        if (currentPollution < 0.0f)
        {
            currentPollution = 0.0f;
        }

        g = 1.0f - (currentPollution / (maxPollution / 4));
        if (g < 0.5f)
        {
            g = 0.5f;
        }

        b = 1.0f - (currentPollution / (maxPollution / 8));
        if (b < 0.0f)
        {
            b = 0.0f;
        }

        _renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", new Color(r, g, b, 1.0f));
        _renderer.SetPropertyBlock(mpb);

        if (currentPollution > 0.0f)
        {
            switch (direction)
            {

                case Direction.NORTH:
                    {
                        if (waterTiles.ContainsKey(new Vector3(transform.position.x, transform.position.y, transform.position.z - 1)))
                        {
                            MovePollution(waterTiles[new Vector3(transform.position.x, transform.position.y, transform.position.z - 1)], 10.0f);
                        }
                        break;
                    }
                case Direction.SOUTH:
                    {
                        Debug.Log("South found");
                        if (waterTiles.ContainsKey(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1)))
                        {
                            MovePollution(waterTiles[new Vector3(transform.position.x, transform.position.y, transform.position.z + 1)], 10.0f);
                            Debug.Log("Pollution Moved");
                        }
                        break;
                    }
                case Direction.EAST:
                    {
                        if (waterTiles.ContainsKey(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z)))
                        {
                            MovePollution(waterTiles[new Vector3(transform.position.x - 1, transform.position.y, transform.position.z)], 10.0f);
                        }
                        break;
                    }
                case Direction.WEST:
                    {
                        if (waterTiles.ContainsKey(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z)))
                        {
                            MovePollution(waterTiles[new Vector3(transform.position.x + 1, transform.position.y, transform.position.z)], 10.0f);
                        }
                        break;
                    }
                case Direction.STILL:
                    {
                        if (waterTiles.ContainsKey(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1)))
                        {
                            MovePollution(waterTiles[new Vector3(transform.position.x, transform.position.y, transform.position.z + 1)], 5.0f);
                        }
                        if (waterTiles.ContainsKey(new Vector3(transform.position.x, transform.position.y, transform.position.z - 1)))
                        {
                            MovePollution(waterTiles[new Vector3(transform.position.x, transform.position.y, transform.position.z - 1)], 5.0f);
                        }
                        if (waterTiles.ContainsKey(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z)))
                        {
                            MovePollution(waterTiles[new Vector3(transform.position.x + 1, transform.position.y, transform.position.z)], 5.0f);
                        }
                        if (waterTiles.ContainsKey(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z)))
                        {
                            MovePollution(waterTiles[new Vector3(transform.position.x - 1, transform.position.y, transform.position.z)], 5.0f);
                        }
                        break;
                    }
            }
        }
    }

    void MovePollution(GameObject hit, float amount)
    {
        hit.GetComponent<Water>().currentPollution += amount;
        currentPollution -= amount;
    }
}
