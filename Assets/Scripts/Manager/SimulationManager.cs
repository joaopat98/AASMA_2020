﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public GameObject CivilianPrefab, PolicePrefab, MedicalPrefab;
    [Range(0, 1)]
    public float AgentScale = 0.75f;
    public int NumCivilians = 25, NumPolice = 5, NumMedical = 5;
    public float StepsPerSecond = 1;
    public bool Playing;
    private int currentStep = 1;
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
    [HideInInspector]
    public List<Agent> Healthy;
    [HideInInspector]
    public List<Agent> Infected;
    [HideInInspector]
    public List<Agent> Dead;

    public float averageTrust = 0.5f;

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
        CreateReport();
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
            if(agent.Infection != InfectionState.Dead)
                agent.Act();
        }
        virus.Step();
        
        InfectedAtThePark.Clear();
        InfectedInStore.Clear();
        HealthyInStore.Clear();
        HealthyAtThePark.Clear();

        UpdateAgentLists();
        UpdateReport();
        currentStep++;
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

    void CreateReport()
    {
        string parameters = averageTrust.ToString() + "," + government.boldness.ToString() + "," + NumCivilians.ToString() + "," + NumPolice.ToString() + "," + NumMedical.ToString();
        string virusParameters = virus.Lethality.ToString() + "," + virus.Transmission.ToString() + "," + virus.IncubationTime.ToString();
        CSVManager.CreateReport(parameters, virusParameters);
    }
    
    void UpdateReport()
    {
        string[] line = new string[4]
        {
            currentStep.ToString(),
            Healthy.Count.ToString(),
            Infected.Count.ToString(),
            Dead.Count.ToString()
        };
        CSVManager.AppendToReport(line);
    }
}