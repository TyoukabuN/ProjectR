namespace PJR.Timeline
{
    public interface IErrorRecorder
    {
        public string Error { get; }
        public bool AnyError { get; }
    }
}
