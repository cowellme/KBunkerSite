namespace KbpApi.Works
{
    public class Logger
    {
        private static string PathLog = Environment.CurrentDirectory + @"\log.tmp";
        public static void Error(Exception exception)
        {
            var date = DateTime.Now;
            var message = exception.Message;
            var source = exception.Source ?? "no source";

            //File.AppendAllText(PathLog, $"{date:F}\n{message} : {source}\n\n");
        }

        public static void Access(string message)
        {
            var date = DateTime.Now;
            //File.AppendAllText(PathLog, $"{date:F}\n{message}\n\n");
        }
    }
}
