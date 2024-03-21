namespace SpotiHotKey
{
    public class Logger
    {
        private static bool firstLog = true;

        public static string filePath = "log.txt";
        public static void LogToFile(string message)
        {
            // Use StreamWriter to append text to the file
            using (StreamWriter writer = new StreamWriter(filePath, append: !firstLog))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
                firstLog = false;
            }
        }
    }
}
