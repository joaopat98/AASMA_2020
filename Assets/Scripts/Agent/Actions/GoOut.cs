using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoOut : Action
{
    public GoOut(Agent agent) : base(agent)
    {

    }

    SimulationManager manager;

    public override void Execute()
    {   
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<SimulationManager>();

        if(agent.Infection == InfectionState.Healthy || agent.Infection == InfectionState.Cured)
        {
            manager.HealthyAtThePark.Add(agent);
        }

        else
        {
           manager.InfectedAtThePark.Add(agent);
        }

        agent.ErrandNeeds = 1;
    }
}