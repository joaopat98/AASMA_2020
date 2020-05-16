using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Agent : MonoBehaviour, IEquatable<Agent>
{
    private static Dictionary<System.Type, AgentType> types = new Dictionary<System.Type, AgentType>() {
        {typeof(Civilian), AgentType.Civilian},
        {typeof(Police), AgentType.Police},
        {typeof(Medical), AgentType.Medical},
    };
    public AgentType Type { get { return types[GetType()]; } }
    public int i, x, y;

    public static Dictionary<AgentAction, ActionOutcome> Outcomes = new Dictionary<AgentAction, ActionOutcome>();

    static List<Vector2Int> aroundDirs = new List<Vector2Int>() {
        new Vector2Int(1,0),
        new Vector2Int(1,1),
        new Vector2Int(1,-1),
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0),
        new Vector2Int(-1,1),
        new Vector2Int(-1,-1),
    };

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

    private int knownDead;
    private int knownInfected;
    private int knownUsingMask;
    private Dictionary<AgentAction, int> knownActions = new Dictionary<AgentAction, int>();
    private void ResetKnown()
    {
        foreach (AgentAction action in Enum.GetValues(typeof(AgentAction)))
        {
            knownActions[action] = 0;
        }
        knownDead = 0;
        knownInfected = 0;
        knownUsingMask = 0;
    }

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

    public virtual void AskNeighbors()
    {
        ResetKnown();
        List<Agent> dead = new List<Agent>(), infected = new List<Agent>();
        foreach (var dir in aroundDirs)
        {
            Agent agent = SimulationManager.main.GetAgent(new Vector2Int(x, y) + dir);
            AgentObservation obs = agent.ObserveNeighbors();
            dead.AddRange(obs.Dead);
            infected.AddRange(obs.Infected);
            knownUsingMask += obs.UsedMask ? 1 : 0;
            knownActions[obs.LastAction]++;
        }
        knownDead = dead.Distinct().Count();
        knownInfected = infected.Distinct().Count();
    }

    public virtual AgentObservation ObserveNeighbors()
    {
        var obs = new AgentObservation(CurrentAction, UsingMask);
        foreach (var dir in aroundDirs)
        {
            var agent = SimulationManager.main.GetAgent(new Vector2Int(x, y) + dir);
            if (agent.Infection == InfectionState.Dead) obs.Dead.Add(agent);
            if (agent.Infection == InfectionState.OpenlyInfected) obs.Infected.Add(agent);
        }
        return obs;
    }

    public virtual void UpdateBeliefs()
    {
        AskNeighbors();
    }

    public virtual void UpdateIntention()
    {

    }

    public virtual void Act()
    {

    }

    public bool Equals(Agent other)
    {
        return x == other.x && y == other.y;
    }
}
