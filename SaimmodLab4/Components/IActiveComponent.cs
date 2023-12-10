namespace SaimmodLab4.Components
{
    public interface IActiveComponent
    {
        double TimeBeforeAction { get; }

        void SimulateTimeElapsed(double elapsed);
    }
}
