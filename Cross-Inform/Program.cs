using System.Diagnostics;
using FrequencyTextAnalyzer;

namespace Cross_Inform
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filePath = Console.ReadLine();
            var frequencyAnalyzer = new FrequencyTextAnalyzer.FrequencyTextAnalyzer(filePath);
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            var query = frequencyAnalyzer.TextAnalysis().Result.AsParallel().OrderByDescending(_ => _.Value).Take(10).ToArray();
            stopwatch.Stop();
            var time = stopwatch.Elapsed;
            
            foreach (var item in query.Take(9))
            {
                Console.Write($"{item.Key}, ");
            }
            Console.WriteLine(query[9].Key);
            Console.Write(String.Format("{0:00}:{1:00}.{2:00}", time.TotalMinutes, time.Seconds, time.Milliseconds));
        }
    }
}