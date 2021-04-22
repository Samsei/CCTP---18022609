using System.Collections.Generic;
using UnityEngine;

public class Pollution : MonoBehaviour
{
    [SerializeField] public float currentPollution = 0.0f;
    private float maxPollution = 256.0f;

    private Renderer _renderer;
    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }

    public void EndTurn(City.WindDirection windDirection, Dictionary<Vector3, GameObject> clouds)
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

        _renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", new Color(0.9f, 0.7f, 0.2f, currentPollution / maxPollution));
        _renderer.SetPropertyBlock(mpb);

        if (currentPollution > 32.0f)
        {
            switch (windDirection)
            {

                case City.WindDirection.NORTH:
                    {
                        if (clouds.ContainsKey(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1)))
                        {
                            MovePollution(clouds[new Vector3(transform.position.x, transform.position.y, transform.position.z + 1)], 10.0f);
                        }
                        break;
                    }
                case City.WindDirection.SOUTH:
                    {
                        if (clouds.ContainsKey(new Vector3(transform.position.x, transform.position.y, transform.position.z - 1)))
                        {
                            MovePollution(clouds[new Vector3(transform.position.x, transform.position.y, transform.position.z - 1)], 10.0f);
                        }
                        break;
                    }
                case City.WindDirection.EAST:
                    {
                        if (clouds.ContainsKey(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z)))
                        {
                            MovePollution(clouds[new Vector3(transform.position.x + 1, transform.position.y, transform.position.z)], 10.0f);
                        }
                        break;
                    }
                case City.WindDirection.WEST:
                    {
                        if (clouds.ContainsKey(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z)))
                        {
                            MovePollution(clouds[new Vector3(transform.position.x - 1, transform.position.y, transform.position.z)], 10.0f);
                        }
                        break;
                    }
                case City.WindDirection.STILL:
                    {
                        if (clouds.ContainsKey(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1)))
                        {
                            MovePollution(clouds[new Vector3(transform.position.x, transform.position.y, transform.position.z + 1)], 5.0f);
                        }
                        if (clouds.ContainsKey(new Vector3(transform.position.x, transform.position.y, transform.position.z - 1)))
                        {
                            MovePollution(clouds[new Vector3(transform.position.x, transform.position.y, transform.position.z - 1)], 5.0f);
                        }
                        if (clouds.ContainsKey(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z)))
                        {
                            MovePollution(clouds[new Vector3(transform.position.x + 1, transform.position.y, transform.position.z)], 5.0f);
                        }
                        if (clouds.ContainsKey(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z)))
                        {
                            MovePollution(clouds[new Vector3(transform.position.x - 1, transform.position.y, transform.position.z)], 5.0f);
                        }
                        break;
                    }
            }            
        }

        if (GetComponentInParent<Water>() && currentPollution >= (maxPollution / 16.0f))
        {
            GetComponentInParent<Water>().currentPollution += 10.0f;
            currentPollution -= 10.0f;
        }
    }

    void MovePollution(GameObject hit, float amount)
    {
        hit.GetComponent<Pollution>().currentPollution += amount;
        currentPollution -= amount;
    }
}
