using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Virus
{
    public int startingInfected = 1;
    public NormalDist Lethality = new NormalDist();
    public NormalDist Transmission = new NormalDist();
    public NormalDist IncubationTime = new NormalDist();
    public NormalDist RecoveryTime = new NormalDist();

    SimulationManager manager;

    private float ParkTransmission;
    private float StoreTransmission;

    public void Init(SimulationManager manager)
    {
        this.manager = manager;
        foreach (var agent in manager.Agents.PickRandom(startingInfected))
        {
            agent.Infection = InfectionState.UnknowinglyInfected;
            agent.IncubationTime = Mathf.RoundToInt(IncubationTime.NextVal());
            agent.RecoveryTime = Mathf.RoundToInt(RecoveryTime.NextVal() * agent.RecoveryFactor);
        }
        StoreTransmission = 0.0f;
        ParkTransmission = 0.0f;
    }

    public void Step()
    {
        StoreTransmission = Transmission.NextVal() *
            CalculateInfectedPercentage(manager.InfectedInStore, manager.HealthyInStore);
        ParkTransmission = Transmission.NextVal() *
            CalculateInfectedPercentage(manager.InfectedAtThePark, manager.HealthyAtThePark);

        foreach (var agent in manager.Agents)
        {
            switch (agent.Infection)
            {
                case InfectionState.UnknowinglyInfected:
                    agent.DaysInfected++;
                    if (agent.DaysInfected >= agent.IncubationTime)
                    {
                        agent.Infection = InfectionState.OpenlyInfected;
                    }
                    break;

                case InfectionState.OpenlyInfected:
                    agent.Health -= Lethality.NextVal() * agent.LethalityFactor;
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
                    if (manager.HealthyInStore.Contains(agent) &&
                        Random.Range(0f, 1f) < StoreTransmission * agent.InfectabilityFactor)
                    {
                        agent.Infection = InfectionState.UnknowinglyInfected;
                        agent.IncubationTime = Mathf.RoundToInt(IncubationTime.NextVal());
                        agent.RecoveryTime = Mathf.RoundToInt(RecoveryTime.NextVal() * agent.RecoveryFactor);
                    }
                    else if (manager.HealthyAtThePark.Contains(agent) &&
                        Random.Range(0f, 1f) < ParkTransmission * agent.InfectabilityFactor)
                    {
                        agent.Infection = InfectionState.UnknowinglyInfected;
                        agent.IncubationTime = Mathf.RoundToInt(IncubationTime.NextVal());
                        agent.RecoveryTime = Mathf.RoundToInt(RecoveryTime.NextVal() * agent.RecoveryFactor);
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
            if ((agent.Infection == InfectionState.UnknowinglyInfected
                || agent.Infection == InfectionState.OpenlyInfected)
                && agent.DaysInfected > agent.RecoveryTime)
            {
                agent.Infection = InfectionState.Cured;
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