namespace SaimmodLab4.Randoms
{
    public class ExponentialRandom : IRandom
    {
        private readonly double _lambda;
        private readonly Random _random;


        public ExponentialRandom(double lambda, Random random)
        {
            _lambda = lambda;
            _random = random;
        }

        public ExponentialRandom(double lambda)
        {
            _lambda = lambda;
            _random = new();
        }


        public double Next()
        {
            double a = _random.NextDouble();
            return -1 * Math.Log(a) / _lambda;
        }
    }
}
