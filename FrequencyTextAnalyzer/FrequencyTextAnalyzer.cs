namespace FrequencyTextAnalyzer
{
    public class FrequencyTextAnalyzer
    {
        private string _text;
        
        private FileStream _stream;

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
                    _stream = new FileStream(value, FileMode.Open, FileAccess.Read);
                    Text = ReadFileAsync(_stream).Result;
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Filepath isn't valid: {e.Message}");
                }

            }
        }

        public FrequencyTextAnalyzer(string text)
        {
            Text = text;
        }

        public async Task<string[]> TextAnalysis()
        {

        }

        private async Task<string> ReadFileAsync(FileStream stream)
        {
            using
            byte[] buffer = new byte[1024];
            int bytesRead;

            stream.D
        }
    }
}