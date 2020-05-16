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

    [HideInInspector]
    public List<Agent> Agents;
    float counter = 0;

    public static SimulationManager main;
    // Start is called before the first frame update
    void Start()
    {
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
}
