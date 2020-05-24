using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Medical : Agent
{
    public override void Act()
    {
        base.Act();
        List<Agent> agents = SimulationManager.main.Agents.Where(a => a.Infection == InfectionState.OpenlyInfected).ToList();

        if (agents == null) 
            return;
        
        agents.Sort((a1, a2) => a1.Health.CompareTo(a2.Health));
        for (int i = 0; i < SimulationManager.main.agentValues.MedicalPatients; i++)
        {
            if(i<agents.Count)
                agents[i].Health = Mathf.Clamp01(agents[i].Health + SimulationManager.main.agentValues.MedicalRecovery.NextVal());
        }
    }
}