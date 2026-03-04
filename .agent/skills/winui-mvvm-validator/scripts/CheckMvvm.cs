using System;
using System.IO;
using System.Linq;

class MvvmChecker
{
    static void Main(string[] args)
    {
        string basePath = args[0];
        var xamlFiles = Directory.GetFiles(basePath, "*.xaml", SearchOption.AllDirectories);

        foreach (var view in xamlFiles)
        {
            string vm = Path.ChangeExtension(view, "ViewModel.cs");
            if (!File.Exists(vm))
            {
                Console.WriteLine($"Missing ViewModel for {view}");
            }
        }
    }
}
