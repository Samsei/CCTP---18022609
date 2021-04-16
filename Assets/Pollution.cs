using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pollution : MonoBehaviour
{
    [SerializeField] int currentPollution = 0;

    private Renderer _renderer;
    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }
    private void Start()
    {
        currentPollution = Random.Range(0, 127);
    }

    public void EndTurn()
    {
        _renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", new Color(0.1f, 0.1f, 0.1f, 0f));
        _renderer.SetPropertyBlock(mpb);
    }
}
