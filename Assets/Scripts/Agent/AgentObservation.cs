using System.Collections.Generic;
using UnityEngine;

public class AgentObservation
{
    public List<Agent> Dead, Infected;
    public AgentAction LastAction;
    public bool UsedMask;

    public AgentObservation(AgentAction lastAction, bool usedMask)
    {
        Dead = new List<Agent>();
        Infected = new List<Agent>();
        LastAction = lastAction;
        UsedMask = usedMask;
    }
}