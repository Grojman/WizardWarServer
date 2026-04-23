public class Always : EffectDuration
{
    public override EffectDuration Clone() => new Always();

    public override bool IsExpired() => false;

    public override void NotifyExecution() {}
}