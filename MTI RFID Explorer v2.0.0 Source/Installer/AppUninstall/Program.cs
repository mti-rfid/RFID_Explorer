using System.Diagnostics;

namespace Uninstaller
{
    class Program
    {
        void UninstallProduct(string argList)
        {
            ProcessStartInfo startInfo
                = new ProcessStartInfo("msiexec.exe", argList);
            Process.Start(startInfo);
        }

        // Inside Installer project, the shortcut that runs this main,
        // needs to have "[ProductCode]" specified in Properties page.
        static void Main(string[] args)
        {
            if (args.Length <= 0) // missing input arg:  [ProductCode]
            {
                string Note = "\n\n";
                Note += "  Error: Missing command input argument [ProductCode]!\n";
                Note += "  This program typically called from Windows shortcut.\n";
                System.Console.Write(Note);
                return;
            }
            string argList = "/x "; // switch argument that uninstalls
            argList += args[0];     // attach argument:  [ProductCode]
            Program myProgram = new Program();
            myProgram.UninstallProduct(argList);

        }
    }
}
