using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pollution : MonoBehaviour
{
    [SerializeField] public float currentPollution = 0.0f;

    private Renderer _renderer;
    private MaterialPropertyBlock mpb;

    private RaycastHit hitObject;

    private Ray ray;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }

    public void EndTurn(City.WindDirection windDirection, Dictionary<Vector3, GameObject> clouds)
    {
        if (currentPollution > 256.0f)
        {
            currentPollution = 256.0f;
        }

        //currentPollution -= 0.1f;

        if (currentPollution < 0.0f)
        {
            currentPollution = 0.0f;
        }

        _renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", new Color(0.9f, 0.7f, 0.2f, currentPollution / 256.0f));
        _renderer.SetPropertyBlock(mpb);

        if (currentPollution > 32.0f && windDirection != City.WindDirection.STILL)
        {
            ray.origin = gameObject.transform.position;

            switch (windDirection)
            {

                case City.WindDirection.NORTH:
                    {
                        if (clouds.ContainsKey(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1)))
                        {
                            MovePollution(clouds[new Vector3(transform.position.x, transform.position.y, transform.position.z + 1)]);
                        }
                        break;
                    }
                case City.WindDirection.SOUTH:
                    {
                        if (clouds.ContainsKey(new Vector3(transform.position.x, transform.position.y, transform.position.z - 1)))
                        {
                            MovePollution(clouds[new Vector3(transform.position.x, transform.position.y, transform.position.z - 1)]);
                        }
                        break;
                    }
                case City.WindDirection.EAST:
                    {
                        if (clouds.ContainsKey(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z)))
                        {
                            MovePollution(clouds[new Vector3(transform.position.x + 1, transform.position.y, transform.position.z)]);
                        }
                        break;
                    }
                case City.WindDirection.WEST:
                    {
                        if (clouds.ContainsKey(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z)))
                        {
                            MovePollution(clouds[new Vector3(transform.position.x - 1, transform.position.y, transform.position.z)]);
                        }
                        break;
                    }
            }            
        }

        if (GetComponentInParent<Water>() && currentPollution >= 32.0f)
        {
            GetComponentInParent<Water>().currentPollution += 10.0f;
            currentPollution -= 10.0f;
        }
    }

    void MovePollution(GameObject hit)
    {
        hit.GetComponent<Pollution>().currentPollution += 10;
        currentPollution -= 10;
    }
}
