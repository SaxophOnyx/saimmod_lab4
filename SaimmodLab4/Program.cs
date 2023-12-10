using SaimmodLab4.Components;
using SaimmodLab4.Randoms;

namespace SaimmodLab4
{
    internal class Program
    {
        static void Main()
        {
            AnalyticsMain();
            Console.WriteLine();
            SimulationMain();
        }

        private static void AnalyticsMain()
        {
            const double lambda = 2;
            const double mu = 1;
            const double p = 0.2;
            const int n = 3;

            double tilda_mu = (1 - p) * mu;
            double rho = lambda / tilda_mu;
            double khi = rho / n;

            double p0 = CalculateP0(n, rho, khi);
            double pn = CalculatePn(rho, p0, n);
            double r = CalculateR(pn, khi);

            Console.WriteLine($"Абсолютная пропускная способность: {lambda}");
            Console.WriteLine($"Ср. длина очереди: {r}");
            Console.WriteLine($"Ср. время в очереди: {r / lambda}");
        }

        private static double CalculateR(double pn, double khi)
        {
            return khi * pn / Math.Pow(1 - khi, 2);
        }

        private static double CalculatePn(double rho, double p0, int n)
        {
            return Math.Pow(rho, n) / Factorial(n) * p0;
        }

        private static double CalculateP0(int n, double rho, double khi)
        {
            double denom = 1;

            for (int k = 1; k <= n; ++k)
                denom += Math.Pow(rho, k) / Factorial(k);

            double a = Math.Pow(rho, n + 1) / (n * Factorial(n));
            double b = 1 / (1 - khi);

            denom += a * b;

            return 1 / denom;
        }

        private static int Factorial(int n)
        {
            if (n < 0)
                throw new ArgumentException("Факториал определен только для неотрицательных чисел.", nameof(n));

            if (n == 0 || n == 1)
                return 1;

            int factorial = 1;
            for (int i = 2; i <= n; i++)
            {
                factorial *= i;
            }

            return factorial;
        }

        private static double CalculateAvgQueueLength(int n, double p, double p0)
        {
            double numer = Math.Pow(p, n + 1);
            double denom = n * Factorial(n) * Math.Pow(1 - p / n, 2);

            return numer / denom * p0;
        }

        private static void SimulationMain()
        {
            const double lambda = 2;
            const double mu = 1;
            const double p = 0.2;
            const double toElapse = 100000;

            AppGenerator generator = new(new ExponentialRandom(lambda));

            List<int> generated = new();
            List<int> wereEnqueue = new();
            Queue<int> awaiting = new();
            List<int> processed = new();
            List<int> rejected = new();
            List<int> queueLog = new();

            List<AppProcessor> processors = new()
            {
                new AppProcessor(new ExponentialRandom(mu), 1 - p),
                new AppProcessor(new ExponentialRandom(mu), 1 - p),
                new AppProcessor(new ExponentialRandom(mu), 1 - p)
            };

            List<IActiveComponent> components = new();
            components.Add(generator);
            components.AddRange(processors);
            components.Reverse();

            generator.OnAppGenerated += (int id) =>
            {
                generated.Add(id);

                AppProcessor? processor = processors.Where(pr => !pr.IsProcessing).FirstOrDefault();

                if (processor != null)
                    processor.StartProcessing(id);
                else
                {
                    awaiting.Enqueue(id);
                    wereEnqueue.Add(id);
                    queueLog.Add(awaiting.Count);
                }
            };

            processors.ForEach(pr =>
            {
                pr.OnAppProcessed += (id) =>
                {
                    processed.Add(id);

                    if (awaiting.Count > 0)
                    {
                        int newAppId = awaiting.Dequeue();
                        queueLog.Add(awaiting.Count);
                        pr.StartProcessing(newAppId);
                    }
                };

                pr.OnAppRejected += (id) =>
                {
                    rejected.Add(id);
                    awaiting.Enqueue(id);
                    int newAppId = awaiting.Dequeue();
                    queueLog.Add(awaiting.Count);
                    pr.StartProcessing(newAppId);
                };
            });

            double elapsed = 0;
            while (elapsed < toElapse)
            {
                double delta = components.Where(c => c.TimeBeforeAction > 0).MinBy(c => c.TimeBeforeAction)!.TimeBeforeAction;
                components.ForEach(c => c.SimulateTimeElapsed(delta));
                elapsed += delta;
            }

            //double avgQueueLength = wereEnqueue.Count * 1.0 / elapsed;
             double avgQueueLength = queueLog.Average();
            double a = processed.Count * 1.0 / elapsed;

            Console.WriteLine($"Сгенерировано: {generated.Count}");
            Console.WriteLine($"Обработано: {processed.Count}");
            Console.WriteLine($"В очереди: {awaiting.Count}");
            Console.WriteLine($"Обрабатывается: {processors.Where(pr => pr.IsProcessing).Count()}");
            Console.WriteLine($"Отказов: {rejected.Count}");
            Console.WriteLine();
            Console.WriteLine($"Абсолютная пропускная способность: {a}");
            Console.WriteLine($"Ср. длина очереди: {avgQueueLength}");
            Console.WriteLine($"Ср. время в очереди: {avgQueueLength / a}");
        }
    }
}
