using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NotSoCritical
{
    class Program
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);

        static int total = 0, totalok = 0, totalerror = 0;
        static string[] WhiteList = { "System", "Idle", "Registry", "Memory Compression" };

        static void Main(string[] args)
        {
            Console.Title = $"Not So Critical [{Environment.UserDomainName}\\{Environment.UserName}]";

            if (File.Exists("WhiteList.txt"))
            {
                using (StreamReader r = new StreamReader("WhiteList.txt"))
                {
                    string[] white = r.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> check = new List<string>();
                    foreach(var str in white)
                    {
                        if (!str.StartsWith("#"))
                            check.Add(str);
                    }
                    WhiteList = check.ToArray();
                }
            }

            Console.WriteLine("All processes will become uncritical, so if you kill one of them no one will cause BSOD.");

            Console.Write("Continue? [y/n] ");
            var a = Console.ReadKey();

            if (a.Key != ConsoleKey.Y)
                return;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            var processes = Process.GetProcesses();
            total = processes.Length;
            foreach (var p in processes)
                MakeProcess(p);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Total: " + total);
            Console.WriteLine("OK: " + totalok);
            Console.WriteLine("Error: " + totalerror);

            Console.WriteLine("Press any key to close this window");
            Console.ReadKey();
        }

        static bool IsInWhiteList(string thing)
        {
            foreach(var w in WhiteList)
            {
                if (w.ToLower() == thing.ToLower())
                    return true;
            }

            return false;
        }

        static void MakeProcess(Process p)
        {
            int isCritical = 0;
            int BreakOnTermination = 0x1D;

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Process: " + p.ProcessName + " - ");

            if(IsInWhiteList(p.ProcessName))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("ignore");
                return;
            }

            int result = 1;
            try
            {
                result = NtSetInformationProcess(p.Handle, BreakOnTermination, ref isCritical, sizeof(int));
            }
            catch
            {
                result = -1;
            }

            if (result != 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("error");
                totalerror++;
            }
            else 
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("ok");
                totalok++;
            }
        }
    }
}
