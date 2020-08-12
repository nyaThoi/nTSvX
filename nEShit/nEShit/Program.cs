using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nEShit
{
    static class Program
    {
 

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.WriteLine($"nTSH Ported Versionn for Astral Realm Online Private Server");
            Console.WriteLine($"Search GameProcess...\n");
            Pinvoke.GetCurrentProccess();
            Console.WriteLine($"Enter the Number from the Game Process to Inject this Hack!");

            int.TryParse(Console.ReadLine(), out int msg);
            if (msg != 0)
            {
                if (Load_Pattern.RetrieveAddresses((uint)msg))
                {
                    MessageBox.Show("Pattern not found! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
                Hook.SetHook();
                var handle = Pinvoke.GetConsoleWindow();
#if RELEASE
                Pinvoke.ShowWindow(handle, 0);
#endif

            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainInterface());
        }
    }
}
