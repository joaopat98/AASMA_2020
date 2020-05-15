public class AgentObservation
{
    public int NumDead, NumInfected;
    public AgentAction LastAction;
    public bool UsingMask;

    public AgentObservation(int numDead, int numInfected, AgentAction lastAction, bool usingMask)
    {
        NumDead = numDead;
        NumInfected = numInfected;
        LastAction = lastAction;
        UsingMask = usingMask;
    }
}