using System.IO;

namespace Chat.Helpers
{
    public class PrepareCard
    {
        public static string ReadCard(string filename)
        {
            var fileRead = File.ReadAllText($"./TemplateCards/{filename}");
            return fileRead;
        }
    }
}
