namespace SpotiHotKey
{
    public class Logger
    {
        public static string filePath = "log.txt";
        public static void LogToFile(string message)
        {
            // Use StreamWriter to append text to the file
            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }
    }
}
