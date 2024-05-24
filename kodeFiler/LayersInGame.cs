using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayersInGame : MonoBehaviour
{


    [SerializeField] LayerMask objects; // used for Objects 
    [SerializeField] LayerMask longGrass; // used for random encounter
    [SerializeField] LayerMask interactionLayer; // used for interactions 
    [SerializeField] LayerMask playerLayer; // used for players
    [SerializeField] LayerMask fovLayer; // used for players


    public static LayersInGame layerRef { get; set; }


    private void Awake()
    {
        layerRef = this;
    }


    // to access the layers

    public LayerMask Objects
    {
        get => objects;
    }


    public LayerMask LongGrass
    {
        get => longGrass;
    }


    public LayerMask InteractionLayer
    {
        get => interactionLayer;
    }


    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }

    public LayerMask FovLayer
    {
        get => fovLayer;
    }



}

