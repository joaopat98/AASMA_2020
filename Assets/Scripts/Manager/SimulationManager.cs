using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public GameObject CivilianPrefab, PolicePrefab, MedicalPrefab;
    [Range(0, 1)]
    public float AgentScale = 0.75f;
    public int NumCivilians = 25, NumPolice = 5, NumMedical = 5;
    public float StepsPerSecond = 1;
    public bool Playing;
    float timePerStep
    {
        get
        {
            return 1 / StepsPerSecond;
        }
    }
    int numAgents;
    int boardSide;

    [System.Serializable]
    public class ActionOutcomeSettings
    {
        public ActionOutcome GoShopping, OrderFood, GoOut, CallFriends;
    }
    public ActionOutcomeSettings ActionOutcomes;
    public Virus virus = new Virus();
    public Government government = new Government();

    [HideInInspector]
    public List<Agent> Agents;
    public List<Agent> Healthy;
    public List<Agent> Infected;
    public List<Agent> Dead;
    float counter = 0;

    public List<Agent> HealthyInStore;
    public List<Agent> InfectedInStore;
    public List<Agent> HealthyAtThePark;
    public List<Agent> InfectedAtThePark;

    public static SimulationManager main;
    // Start is called before the first frame update
    void Start()
    {
        HealthyInStore = new List<Agent>();
        InfectedInStore = new List<Agent>();

        SimulationManager.main = this;

        Agent.Outcomes[AgentAction.CallFriends] = ActionOutcomes.CallFriends;
        Agent.Outcomes[AgentAction.GoOut] = ActionOutcomes.GoOut;
        Agent.Outcomes[AgentAction.GoShopping] = ActionOutcomes.GoShopping;
        Agent.Outcomes[AgentAction.OrderFood] = ActionOutcomes.OrderFood;

        numAgents = NumCivilians + NumPolice + NumMedical;
        boardSide = Mathf.CeilToInt(Mathf.Sqrt(numAgents));

        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        float minX = bounds.min.x;
        float minY = bounds.min.y;
        float width = bounds.size.x;
        float height = bounds.size.y;
        float sizeX = width / boardSide, sizeY = height / boardSide;
        Vector3 topLeft = bounds.max + width * Vector3.left + new Vector3(sizeX / 2, -sizeY / 2, 0);

        float scaleFac = 1 / (float)boardSide * AgentScale;
        Agents = new List<Agent>();
        int i = 0;
        for (int j = 0; j < NumCivilians; j++)
        {
            int x = i % boardSide, y = i / boardSide;
            Agents.Add(Instantiate(CivilianPrefab, topLeft + new Vector3(x * sizeX, -y * sizeY, 0), Quaternion.identity, transform).GetComponent<Agent>());
            i++;
        }
        for (int j = 0; j < NumPolice; j++)
        {
            int x = i % boardSide, y = i / boardSide;
            Agents.Add(Instantiate(PolicePrefab, topLeft + new Vector3(x * sizeX, -y * sizeY, 0), Quaternion.identity, transform).GetComponent<Agent>());
            i++;
        }
        for (int j = 0; j < NumMedical; j++)
        {
            int x = i % boardSide, y = i / boardSide;
            Agents.Add(Instantiate(MedicalPrefab, topLeft + new Vector3(x * sizeX, -y * sizeY, 0), Quaternion.identity, transform).GetComponent<Agent>());
            i++;
        }

        for (int index = 0; index < numAgents; index++)
        {
            int x = index % boardSide, y = index / boardSide;
            Agents[index].transform.localScale *= scaleFac;
            Agents[index].x = x;
            Agents[index].y = y;
            Agents[index].i = i;
            Agents[index].Init();
        }

        virus.Init(this);
    }

    void Update()
    {
        if (Playing)
        {
            counter += Time.deltaTime;
            while (counter > timePerStep)
            {
                Step();
                counter -= timePerStep;
            }
        }
    }

    public void Step()
    {  
        virus.Step();
        government.Step(Agents.Count, Infected.Count, Dead.Count);

        foreach (var agent in Agents)
        {
            agent.SocialNeeds = Mathf.Clamp01(agent.SocialNeeds + 0.1f);
            agent.ErrandNeeds = Mathf.Clamp01(agent.ErrandNeeds + 0.1f);
        }
        foreach (var agent in Agents)
        {
            agent.AskNeighbors();
        }
        foreach (var agent in Agents)
        {
            agent.UpdateBeliefs();
        }
        foreach (var agent in Agents)
        {
            agent.UpdateIntention();
        }
        foreach (var agent in Agents)
        {
            agent.Act();
        }
        Calculate_StoreAndPark();
        UpdateAgentLists();
    }

    public Agent GetAgent(int x, int y)
    {
        if (y >= boardSide || x >= boardSide || y < 0 || x < 0 || y * boardSide + x > numAgents)
        {
            return null;
        }
        return Agents[y * boardSide + x];
    }

    public Agent GetAgent(Vector2Int v)
    {
        return GetAgent(v.x, v.y);
    }

    float CalculateInfectedPercentage(List<Agent> Infected, List<Agent> Healthy)
    {
        if (Infected.Count == 0 && Healthy.Count == 0)
            return 0.0f;
        return (float)Infected.Count / (float)(Infected.Count + Healthy.Count);
    }

    void CalculateInfections(List<Agent> HealthyInArea, float percentage)
    {
        foreach (Agent agent in HealthyInArea)
        {
            if (agent.Infection == InfectionState.Healthy && Random.Range(0.0f, 1.0f) < percentage)
            {
                virus.toInfect.Add(agent);
            }
        }
    }

    void Calculate_StoreAndPark()
    {

        float InfectedPercentageStore = CalculateInfectedPercentage(InfectedInStore, HealthyInStore);
        float InfectedPercentagePark = CalculateInfectedPercentage(InfectedAtThePark, HealthyAtThePark);

        CalculateInfections(HealthyInStore, InfectedPercentageStore);
        CalculateInfections(HealthyAtThePark, InfectedPercentagePark);

        InfectedInStore.Clear();
        HealthyInStore.Clear();

        HealthyAtThePark.Clear();
        InfectedAtThePark.Clear();
    }

    void UpdateAgentLists()
    {
        Healthy.Clear();
        Infected.Clear();
        Dead.Clear();
        foreach (var agent in Agents)
        {
            switch (agent.Infection)
            {
                case (InfectionState.Healthy): Healthy.Add(agent); break;
                case (InfectionState.Cured): Healthy.Add(agent); break;
                case (InfectionState.OpenlyInfected): Infected.Add(agent); break;
                case (InfectionState.UnknowinglyInfected): Infected.Add(agent); break;
                case (InfectionState.Dead): Dead.Add(agent); break;
            }
        }
    }
}
