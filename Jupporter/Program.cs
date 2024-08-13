using System;
using System.Threading;
using System.Windows.Forms;

namespace Jupporter
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            using (Mutex mutex = new Mutex(false, "Global\\3c81c1dd-9755-4c8a-a700-72f31fba8414"))
            {
                if (!mutex.WaitOne(500, false))
                {
                    return;
                }
                /*Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);*/
                Application.Run(new Main(args));
            }
        }
    }
}
