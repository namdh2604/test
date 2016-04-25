namespace Voltage.Witches.Scheduling
{
    public interface ITask
    {
        void Execute();
        string Name { get; }
    }
}

