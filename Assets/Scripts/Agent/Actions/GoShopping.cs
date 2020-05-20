using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoShopping : Action
{
    public GoShopping(Agent agent) : base(agent)
    {

    }

    SimulationManager manager;

    public override void Execute()
    {   
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<SimulationManager>();

        if(agent.Infection == InfectionState.Healthy || agent.Infection == InfectionState.Cured)
        {
            manager.HealthyInStore.Add(agent);
        }

        else
        {
           manager.InfectedInStore.Add(agent);
        }

        agent.ErrandNeeds = 1;
    }
}