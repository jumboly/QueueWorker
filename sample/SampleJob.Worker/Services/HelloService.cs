namespace SampleJob.Worker.Services
{
    public interface ICalcService
    {
        int Add(int a, int b);
        int Sub(int a, int b);
    }

    public class CalcService : ICalcService
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Sub(int a, int b)
        {
            return a - b;
        }
    }
}