using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Action
{
    public static Dictionary<System.Type, AgentAction> Types = new Dictionary<System.Type, AgentAction>()
    {
        {typeof(GoShopping),AgentAction.GoShopping},
    };
    public static Action FromType(AgentAction type, Agent agent)
    {
        switch (type)
        {
            case AgentAction.GoShopping:
                return new GoShopping(agent);
            case AgentAction.GoOut:
                return new GoOut(agent);
            case AgentAction.OrderFood:
                return new OrderFood(agent);
            default:
                return new CallFriend(agent);
        }
    }

    public AgentAction type { get { return Types[GetType()]; } }
    protected Agent agent;

    public Action(Agent agent)
    {
        this.agent = agent;
    }

    public abstract void Execute();

}