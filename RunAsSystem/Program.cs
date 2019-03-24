using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace RunAsSystem
{
    static class Program
    {
        static void Main(string[] args)
        {
            var exec = new FileInfo("PsExec.exe");
            var notso = new FileInfo("NotSoCritical.exe");

            if (!exec.Exists || !notso.Exists)
                return;

            Process a = new Process();
            a.StartInfo.FileName = exec.FullName;
            a.StartInfo.Arguments = $"-i -s \"{notso.FullName}\"";
            a.Start();
        }
    }
}
