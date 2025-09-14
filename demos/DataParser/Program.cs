// Data Parser - Half-baked Demo
// Features: Parse CSV and display results
using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.Write("Enter CSV file path: ");
        var path = Console.ReadLine();
        if (!File.Exists(path))
        {
            Console.WriteLine("File not found.");
            return;
        }
        var lines = File.ReadAllLines(path);
        foreach (var line in lines)
        {
            var fields = line.Split(',');
            Console.WriteLine(string.Join(" | ", fields));
        }
    }
}
