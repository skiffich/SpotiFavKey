namespace SpotiHotKey
{
    public class Logger
    {
        private static bool firstLog = true;

        public static string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpotiHotKey", "veryTestLog.txt");

        public static void LogToFile(string message)
        {
            // Create the directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Use StreamWriter to append text to the file
            using (StreamWriter writer = new StreamWriter(filePath, append: !firstLog))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
                firstLog = false;
            }
        }
    }
}
