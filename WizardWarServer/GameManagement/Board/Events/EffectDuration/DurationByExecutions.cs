
public class DurationByExecutions : EffectDuration
{
    public DurationByExecutions()
    {
    }

    public DurationByExecutions(int times)
    {
        this.times = times;
    }

    public DurationByExecutions(bool isExpired, int times, int triggerCounter)
    {
        this.isExpired = isExpired;
        this.times = times;
        this.triggerCounter = triggerCounter;
    }
    

    private bool isExpired { get; set; } = false;
    private int times { get; set; } = 1;
    private int triggerCounter { get; set; } = 0;
    public override EffectDuration Clone()
    {
        return new DurationByExecutions()
        {
            isExpired = false,
            times = times,
            triggerCounter = 0
        };
    }

    public override bool IsExpired() => isExpired;

    public override void NotifyExecution()
    {
        triggerCounter++;
        if (triggerCounter >= times)
        {
            isExpired = true;
        }
    }
}