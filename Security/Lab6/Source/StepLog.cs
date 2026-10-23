

namespace Source
{
    public class StepLog
    {
        private readonly List<string> _steps = new();

        public IReadOnlyList<string> Steps => _steps;
        

        public void Add(string title, string details = "")
        {
            _steps.Add(string.IsNullOrEmpty(details) ? title : $"{title} {details}");

        }

        public void Clear() => _steps.Clear();

        public override string ToString()
        {
            return string.Join(Environment.NewLine, _steps);
        }
    }
}