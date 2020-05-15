[System.Serializable]
public class ActionOutcome
{
    public float risk;
    public float socialReward;
    public float errandReward;

    public ActionOutcome(float risk, float socialReward, float errandReward)
    {
        this.risk = risk;
        this.socialReward = socialReward;
        this.errandReward = errandReward;
    }
}