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
    public float SocialDistancing = 0;
    [Range(0, 1)]
    public float Trust = 0;

    [Range(0, 4)]
    public int AgeGroup = 0;

    public int DaysInfected = 0;
    public bool UsingMask;

    public AgentAction CurrentAction;
    public float WillRisk;
    private int knownDead;
    private int knownInfected;
    private int knownUsingMask;
    private int aliveNeighbors, totalNeighbors;
    private float SocialDistancingNeighbors;
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
        aliveNeighbors = 0;
        totalNeighbors = 0;
        SocialDistancing = 0;
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
        this.AgeGroup = UnityEngine.Random.Range(0, 5);
        this.Trust = UnityEngine.Random.value;
        AgentAction[] actions = (AgentAction[])Enum.GetValues(typeof(AgentAction));
        CurrentAction = RandomTools.PickRandom(new List<AgentAction>(actions), 1)[0];
    }

    //If you are >75 years old the lethality is 5 times as high as <25 years old.
    public virtual float LethalityFactor()//(Age)
    {
        return (0.2f + this.AgeGroup * 0.2f);
    }

    //Being medical staff or police increases your chance of being infected by 3.
    public virtual float InfectabilityFactor()//(Type)
    {
        if (this.Type == AgentType.Civilian)
            return 0.2f;
        else
            return 0.8f;
    }


    public virtual void AskNeighbors()
    {
        ResetKnown();
        List<Agent> dead = new List<Agent>(), infected = new List<Agent>();
        foreach (var dir in aroundDirs)
        {

            Agent agent = SimulationManager.main.GetAgent(new Vector2Int(x, y) + dir);
            if (agent != null)
            {
                AgentObservation obs = agent.ObserveNeighbors();
                dead.AddRange(obs.Dead);
                infected.AddRange(obs.Infected);
                knownUsingMask += obs.UsedMask ? 1 : 0;
                knownActions[obs.LastAction]++;
                aliveNeighbors += agent.Infection != InfectionState.Dead ? 1 : 0;
                totalNeighbors += 1;
                if (agent.Infection != InfectionState.Dead)
                {
                    SocialDistancingNeighbors += agent.SocialDistancing;
                }
            }
        }
        if (aliveNeighbors > 0)
            SocialDistancingNeighbors /= aliveNeighbors;
        knownDead = dead.Distinct().Count();
        knownInfected = infected.Distinct().Count();
    }

    public virtual AgentObservation ObserveNeighbors()
    {
        var obs = new AgentObservation(CurrentAction, UsingMask);
        foreach (var dir in aroundDirs)
        {

            var agent = SimulationManager.main.GetAgent(new Vector2Int(x, y) + dir);
            if (agent != null)
            {
                if (agent.Infection == InfectionState.Dead) obs.Dead.Add(agent);
                if (agent.Infection == InfectionState.OpenlyInfected) obs.Infected.Add(agent);
            }

        }
        return obs;
    }


    public virtual void UpdateBeliefs()
    {
        var govAdvice = SimulationManager.main.government.GetAdvice();
        float decision = Trust * (govAdvice.useMask ? 1 : 0);
        if (aliveNeighbors > 0)
            decision += (1 - Trust) * (knownUsingMask / (float)aliveNeighbors);
        UsingMask = decision >= UnityEngine.Random.value;
        SocialDistancing = Trust * govAdvice.socialDistancing;
        if (aliveNeighbors > 0)
            SocialDistancingNeighbors += (1 - Trust) * SocialDistancingNeighbors;
    }

    public virtual void UpdateIntention()
    {
        bool social;
        if (SocialNeeds > ErrandNeeds)
        {
            social = true;
        }
        else if (SocialNeeds < ErrandNeeds)
        {
            social = false;
        }
        else
        {
            social = UnityEngine.Random.value > 0.5;
        }

        if (social)
        {
            float sNeed = SocialNeeds * (1 - SocialDistancing);
            float thresh = 1 - knownActions[AgentAction.GoOut] / (float)aliveNeighbors;
            if (sNeed > thresh)
            {
                CurrentAction = AgentAction.GoOut;
            }
            else
            {
                CurrentAction = AgentAction.CallFriends;
            }
        }
        else
        {
            float eNeed = ErrandNeeds * (1 - SocialDistancing);
            float thresh = 1 - knownActions[AgentAction.GoShopping] / (float)aliveNeighbors;
            if (eNeed > thresh)
            {
                CurrentAction = AgentAction.GoShopping;
            }
            else
            {
                CurrentAction = AgentAction.OrderFood;
            }
        }
    }

    public void Act()
    {
        switch (CurrentAction)
        {
            case AgentAction.CallFriends:
                SocialNeeds -= 0.1f;
                Mathf.Clamp(SocialNeeds, 0.0f, 1.0f);
                break;
            case AgentAction.GoOut:
                if (Infection == InfectionState.Healthy || Infection == InfectionState.Cured)
                {
                    SimulationManager.main.HealthyAtThePark.Add(this);
                }
                else
                {
                    SimulationManager.main.InfectedAtThePark.Add(this);
                }
                SocialNeeds -= 0.2f;
                break;
            case AgentAction.OrderFood:
                ErrandNeeds -= 0.1f;
                Mathf.Clamp(ErrandNeeds, 0.0f, 1.0f);
                break;
            default:
                if (Infection == InfectionState.Healthy || Infection == InfectionState.Cured)
                {
                    SimulationManager.main.HealthyInStore.Add(this);
                }
                else
                {
                    SimulationManager.main.InfectedInStore.Add(this);
                }
                ErrandNeeds -= 0.2f;
                break;
        }
    }

    public bool Equals(Agent other)
    {
        return x == other.x && y == other.y;
    }
}
