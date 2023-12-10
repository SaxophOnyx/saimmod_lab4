using SaimmodLab4.Randoms;

namespace SaimmodLab4.Components
{
    public class AppProcessor : IActiveComponent
    {
        private readonly Random _processingRandom;
        private readonly double _processingProbability;
        private readonly IRandom _timeRandom;
        private int? _applicationId;

        public bool IsProcessing => _applicationId != null;

        public double TimeBeforeAction { get; private set; }

        public event Action<int>? OnAppProcessed;

        public event Action<int>? OnAppRejected;


        public AppProcessor(IRandom timeRandom, double processingProbability)
        {
            _processingRandom = new();
            _processingProbability = processingProbability;
            _timeRandom = timeRandom;
        }


        public void StartProcessing(int applicationId)
        {
            if (IsProcessing)
                throw new InvalidOperationException();

            _applicationId = applicationId;
            TimeBeforeAction = _timeRandom.Next();
        }

        public void SimulateTimeElapsed(double elapsed)
        {
            if (IsProcessing)
            {
                if (TimeBeforeAction < elapsed)
                    throw new InvalidOperationException();

                TimeBeforeAction -= elapsed;

                if (TimeBeforeAction == 0)
                {
                    double probability = _processingRandom.NextDouble();
                    int appId = (int)_applicationId!;
                    _applicationId = null;

                    if (probability < _processingProbability)
                    {
                        OnAppProcessed?.Invoke(appId);
                    }
                    else
                    {
                        OnAppRejected?.Invoke(appId);
                    }
                }
            }
        }
    }
}
