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
        }
        
        toInfect.Clear();
    }
}