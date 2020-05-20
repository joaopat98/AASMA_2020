using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Virus
{
    public int startingInfected = 1;
    public float Lethality = 1;
    public float Transmission = 1;
    public float IncubationTime = 5;

    SimulationManager manager;
    public List<Agent> toInfect = new List<Agent>();

    public void Init(SimulationManager manager)
    {
        this.manager = manager;
        foreach (var agent in manager.Agents.PickRandom(startingInfected))
        {
            agent.Infection = InfectionState.UnknowinglyInfected;
        }
    }

    public void Step()
    {
        //List<Agent> toInfect = new List<Agent>();
        foreach (var agent in manager.Agents)
        {
            if (agent.Infection == InfectionState.UnknowinglyInfected)
            {
                agent.DaysInfected++;
                if (agent.DaysInfected >= IncubationTime)
                {
                    agent.Infection = InfectionState.OpenlyInfected;
                }
            }
            else if (agent.Infection == InfectionState.OpenlyInfected)
            {
                agent.Health -= Lethality;
                if (agent.Health <= 0)
                {
                    agent.Health = 0;
                    agent.Infection = InfectionState.Dead;
                }
            }
            
            if (agent.Infection == InfectionState.UnknowinglyInfected || agent.Infection == InfectionState.OpenlyInfected)
            {
                List<Vector2Int> offsetCoords = new List<Vector2Int>() {
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 0),
                    new Vector2Int(0, -1),
                    new Vector2Int(-1, 0),
                };
                foreach (var offset in offsetCoords)
                {
                    Vector2Int coord = new Vector2Int(agent.x, agent.y) + offset;
                    Agent agentToInfect = manager.GetAgent(coord);
                    if (agentToInfect != null
                        && agentToInfect.Infection == InfectionState.Healthy
                        && !toInfect.Contains(agentToInfect))
                    {
                        if (Random.value < Transmission)
                            toInfect.Add(agentToInfect);
                    }
                }
            }
            
            if(toInfect.Contains(agent) && Random.Range(0.0f, 1.0f) < Transmission)
            {
                agent.Infection = InfectionState.UnknowinglyInfected;
            }
        }
        toInfect.Clear();
    }
}