using System.Threading.Tasks;

namespace SCUMSLang.IO
{
    public class DirectAcyclicGraph
    {
        public string FilePath { get; }

        public DirectAcyclicGraph(string filePath) {
            FilePath = filePath ?? throw new System.ArgumentNullException(nameof(filePath));
        }

        public async Task LoadGraphAsync() { 
            
        }
    }
}
