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
    public int i, x, y;

    public static Dictionary<AgentAction, ActionOutcome> Outcomes = new Dictionary<AgentAction, ActionOutcome>();

    public InfectionState Infection = InfectionState.Healthy;

    [Range(0, 1)]
    public float Health = 1;

    [Range(0, 1)]
    public float SocialNeeds = 0, ErrandNeeds = 0;

    [Range(0, 1)]
    public float Trust = 1;

    [Range(0, 4)]
    public int AgeGroup = 0;

    public int DaysInfected = 0;
    public bool UsingMask;
    public AgentAction CurrentAction;
    public float WillRisk;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Init()
    {

    }

    //Calculate the lethality proper to this agent. 
    //Will act as a multiplier on the virus lethality
    public virtual float LethalityFactor()//(Age, Type)
    {
        Debug.Log("Lethality not implemented!");
        return 0;
    }

    public virtual float InfectabilityFactor()//(Age, Type)
    {
        Debug.Log("Infectability not implemented!");
        return 0;
    }

    public virtual void UpdateBeliefs()
    {

    }

    public virtual void UpdateIntention()
    {

    }

    public virtual void Act()
    {

    }

    public virtual void AskNeighbors()
    {

    }

    public virtual AgentObservation ObserveNeighbors()
    {
        return null;
    }

    public virtual void Step()
    {
        UpdateBeliefs();
        UpdateIntention();
        Act();
    }
}
