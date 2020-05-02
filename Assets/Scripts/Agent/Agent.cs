using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    private static Dictionary<System.Type, AgentType> types = new Dictionary<System.Type, AgentType>() {
        {typeof(Civilian), AgentType.Civilian},
        {typeof(Police), AgentType.Police},
        {typeof(Medical), AgentType.Medical},
    };

    public AgentType Type { get { return types[GetType()]; } }
    public InfectionState Infection = InfectionState.Healthy;
    [Range(0, 1)]
    public float Health = 1;
    [Range(0, 1)]
    public float SocialNeeds = 0, ErrandNeeds = 0;
    [Range(0, 1)]
    public float Trust = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Step()
    {
        if (Health > 0)
        {
            Health -= 0.01f;
        }
        if (Health < 0)
        {
            Health = 0;
        }
    }
}
