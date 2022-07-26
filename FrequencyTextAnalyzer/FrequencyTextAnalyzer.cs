using System.Collections.Concurrent;
using System.Text;

namespace FrequencyTextAnalyzer
{
    public class FrequencyTextAnalyzer
    {
        private string _text;

        private object _lock = new object();

        private CancellationToken _token;

        private ConcurrentDictionary<string, int> _dictionary;

        public string Text
        {
            get { return _text; }
            set
            {
                if (value.Length != 0)
                    _text = value;
                else throw new ArgumentException("The size of text should be greater than zero");
            }
        }

        public string FilePath
        {
            set
            {
                try
                {
                    Text = ReadFileAsync(value).Result;
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Filepath isn't valid: {e.Message}");
                }

            }
        }

        public FrequencyTextAnalyzer(string text, CancellationToken? token)
        {
            Text = text;
            _lock = new object();
            _token = token ?? default;
            _dictionary = new ConcurrentDictionary<string, int>();
        }

        public FrequencyTextAnalyzer(string text)
            :this(text, null)
        {
        }

        public async Task<Dictionary<string,int>> TextAnalysis()
        {
            for (int j = 0; j < 3; j++)
            {
                Parallel.ForEach(Partitioner.Create(0, _text.Length / 3), new ParallelOptions() { MaxDegreeOfParallelism = 4 }, () => new Dictionary<string, int>(),
                    (range, parallelLoopState, dictionary) =>
                {
                    int lastIndex;
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        lastIndex = i * 3 + 2 + j;
                        if (lastIndex < _text.Length)
                        {
                            var key = _text.Substring(lastIndex - 2, 3);

                            if (dictionary.TryGetValue(key, out var result))
                                dictionary[key]++;
                            else dictionary.Add(key, 1);
                        }
                    }
                    return dictionary;
                },
                    (dictionary) =>
                {
                    foreach (var item in dictionary)
                        lock (_lock)
                        {
                            if(!_dictionary.TryAdd(item.Key, item.Value))
                                dictionary[item.Key]+=item.Value;
                        }
                });
            }
            return _dictionary.AsParallel().OrderByDescending(_ => _.Value).Take(10).ToDictionary(_=>_.Key, _=>_.Value);
        }

        private async Task<string> ReadFileAsync(string filePath) => await File.ReadAllTextAsync(filePath, cancellationToken: _token);
    }
}