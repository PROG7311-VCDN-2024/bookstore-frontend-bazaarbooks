using System;
using System.IO;
using System.Text;

namespace BazaarBooks_ProgSprint.Logger
{

    public sealed class Log : ILog
    {
        private Log()
        {
        }
        private static readonly Log LogInstance = new Log();

        public static Log GetInstance()
        {
            return LogInstance;
        }

        public void LogException(string message)
        {
            string fileName = "failed-login";

            string logFilePath = string.Format(@"{0}/{1}", AppDomain.CurrentDomain.BaseDirectory, fileName);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-------------------------------------------------------------------------------");
            sb.AppendLine(DateTime.Now.ToString() + " : " + message);

            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(sb.ToString());
                writer.Flush();
            }
        }
    }

}// end 
