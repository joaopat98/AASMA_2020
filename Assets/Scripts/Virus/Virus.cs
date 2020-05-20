using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Virus
{
    public int startingInfected = 1;
    public float Lethality = 1;
    public float Transmission = 1;
    public float IncubationTime = 5;
    //public int RecoveryDays = 1;

    SimulationManager manager;

    private float ParkTransmission;
    private float StoreTransmission;

    public void Init(SimulationManager manager)
    {
        this.manager = manager;
        foreach (var agent in manager.Agents.PickRandom(startingInfected))
        {
            agent.Infection = InfectionState.UnknowinglyInfected;
        }
        StoreTransmission = 0.0f;
        ParkTransmission = 0.0f;
    }

    public void Step()
    {
        StoreTransmission = Transmission *
            CalculateInfectedPercentage(manager.InfectedInStore, manager.HealthyInStore);
        ParkTransmission = Transmission *
            CalculateInfectedPercentage(manager.InfectedAtThePark, manager.HealthyAtThePark);

        foreach (var agent in manager.Agents)
        {
            switch (agent.Infection)
            {
                case InfectionState.UnknowinglyInfected:
                    agent.DaysInfected++;
                    if (agent.DaysInfected >= IncubationTime)
                    {
                        agent.Infection = InfectionState.OpenlyInfected;
                    }
                    break;

                case InfectionState.OpenlyInfected:
                    agent.Health -= Lethality;
                    agent.DaysInfected++;
                    if (agent.Health <= 0)
                    {
                        agent.Health = 0;
                        agent.Infection = InfectionState.Dead;
                    }
                    /*
                    else if(agent.DaysInfected == RecoveryDays)
                        {
                            agent.Infection = InfectionState.Cured;
                        }
                    }*/
                    break;
            
                case InfectionState.Healthy:
                    if(manager.HealthyInStore.Contains(agent) &&
                        Random.Range(0f, 1f) < StoreTransmission)
                    {
                        agent.Infection = InfectionState.UnknowinglyInfected;
                    }
                    else if(manager.HealthyAtThePark.Contains(agent) &&
                        Random.Range(0f, 1f) < ParkTransmission)
                    {
                        agent.Infection = InfectionState.UnknowinglyInfected;
                    }
                    break;

                case InfectionState.Cured:
                    /*if(agent.Health < 1.0f)
                        agent.Health += Lethality;
                    */
                    break;
                
                //Dead
                default:
                    break;
            }
        }
    }
    

    float CalculateInfectedPercentage(List<Agent> Infected, List<Agent> Healthy)
    {
        if (Infected.Count == 0 && Healthy.Count == 0)
            return 0.0f;
        return (float)Infected.Count / (float)(Infected.Count + Healthy.Count);
    }    
}