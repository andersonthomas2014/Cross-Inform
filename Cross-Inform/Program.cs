namespace Cross_Inform
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filePath = Console.ReadLine();
            var tokenSource = new CancellationTokenSource();
            var frequencyAnalyzer = new FrequencyTextAnalyzer.FrequencyTextAnalyzer(filePath, tokenSource.Token);
            var query = frequencyAnalyzer.TextAnalysis().Result;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
        }
    }
}