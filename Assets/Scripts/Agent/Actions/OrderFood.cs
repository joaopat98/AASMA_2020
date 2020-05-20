using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderFood : Action
{
    public OrderFood(Agent agent) : base(agent)
    {

    }

    public override void Execute()
    {   
        agent.ErrandNeeds = 0.5f;
    }
}