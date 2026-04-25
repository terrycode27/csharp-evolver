public partial class QuotedStringEventEmitter : ChainedEventEmitter
{
    public QuotedStringEventEmitter(IEventEmitter nextEmitter) : base(nextEmitter) { }
    public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
    {
        if (eventInfo.Source.Type == typeof(string))
        {
            eventInfo = new ScalarEventInfo(eventInfo.Source) { Style = ScalarStyle.DoubleQuoted };
        }
        base.Emit(eventInfo, emitter);
    }
}

