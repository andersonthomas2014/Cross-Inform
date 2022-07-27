using System.Collections.Concurrent;
using System.Text;

namespace FrequencyTextAnalyzer
{
    public class FrequencyTextAnalyzer
    {
        private string _text;

        private Task _loadData;

        private CancellationToken _token;

        private ConcurrentDictionary<string, int> _dictionary = new ConcurrentDictionary<string, int>();

        public string FilePath
        {
            set
            {
                _loadData = ReadFileAsync(value);
            }
        }

        public FrequencyTextAnalyzer(string filePath, CancellationToken? token)
        {
            FilePath = filePath;
            _token = token ?? default;
        }

        public FrequencyTextAnalyzer(string filePath)
            :this(filePath, null)
        {
        }

        /// <summary>
        /// Производит поиск триплетов в тексте.
        /// </summary>
        /// <returns>Возвращает словарь c парами: триплет - количество вхождений.</returns>
        public async Task<ConcurrentDictionary<string,int>> TextAnalysis()
        {
            await _loadData.ConfigureAwait(false);
            //j - смещение от начала текста
            //Если считывать по три символа, начиная с 0-го, 1-го, 2-го,
            //то получится перебрать все комбинации (триплеты) в тексте
            await Task.Run(() =>
            {
                for (int j = 0; j < 3; j++)
                {
                    int temp = j;
                    Parallel.ForEach(Partitioner.Create(0, _text.Length / 3), new ParallelOptions() { MaxDegreeOfParallelism = 2, CancellationToken = _token }, () => new Dictionary<string, int>(),
                        (range, parallelLoopState, dictionary) =>
                    {
                        int lastIndex;
                        for (int i = range.Item1; i < range.Item2; i++)
                        {
                            lastIndex = i * 3 + 2 + temp;
                            if (lastIndex < _text.Length)
                            {
                                var key = _text.Substring(lastIndex - 2, 3);

                                if (!dictionary.TryAdd(key, 1))
                                    dictionary[key]++;
                            }
                        }
                        return dictionary;
                    },
                        (dictionary) =>
                    {
                        foreach (var item in dictionary)

                            if (!_dictionary.TryAdd(item.Key, item.Value))
                                _dictionary[item.Key] += item.Value;
                    });
                }
            });
            return _dictionary;
        }

        private async Task ReadFileAsync(string filePath)
        {
            try
            {
                _text = await File.ReadAllTextAsync(filePath, cancellationToken: _token);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Filepath isn't valid: {e.Message}");
            }
        }
    }
}