using SaimmodLab4.Randoms;

namespace SaimmodLab4.Components
{
    public class AppGenerator : IActiveComponent
    {
        private readonly IRandom _timeRandom;
        private readonly Random _appIdRandom;

        public double TimeBeforeAction { get; private set; }
        
        public event Action<int>? OnAppGenerated;


        public AppGenerator(IRandom timeRandom)
        {
            _timeRandom = timeRandom;
            _appIdRandom = new Random();
            TimeBeforeAction = _timeRandom.Next();
        }


        public void SimulateTimeElapsed(double elapsed)
        {
            if (TimeBeforeAction < elapsed)
                throw new InvalidOperationException();

            TimeBeforeAction -= elapsed;

            if (TimeBeforeAction == 0)
            {
                TimeBeforeAction = _timeRandom.Next();
                int applicationId = _appIdRandom.Next();
                OnAppGenerated?.Invoke(applicationId);
            }
        }
    }
}
