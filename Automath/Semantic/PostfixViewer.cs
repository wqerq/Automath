using Automath.Semantic;
using System;
using System.Collections.Generic;
using Automath.Elements;

namespace Automath.Semantic
{
    internal class PostfixViewer
    {
        private readonly Dictionary<int, string> postfixes;
        private readonly Dictionary<int, PostfixEntry> postfixEntries;

        public PostfixViewer(Dictionary<int, string> postfixStrings, Dictionary<int, PostfixEntry> postfixEntries)
        {
            this.postfixes = postfixStrings;
            this.postfixEntries = postfixEntries;
        }


        public void DisplayPostfixWithIndex()
        {
            Console.WriteLine("ПОЛИЗ (с индексами):");
            foreach (var postfix in postfixes)
            {
                Console.WriteLine($"{postfix.Key}. {postfix.Value}");
            }
        }


        public void DisplayPostfixAsLine()
        {
            Console.Write("ПОЛИЗ: ");
            foreach (var postfix in postfixes)
            {
                Console.Write($"{postfix.Value} ");
            }
            Console.WriteLine();
        }

        public void DisplayDetailedEntries()
        {
            Console.WriteLine("Подробная информация о ПОЛИЗ:");
            foreach (var entry in postfixEntries)
            {
                Console.WriteLine($"Индекс: {entry.Key}, Тип: {entry.Value.Type}, Значение: {entry.Value.Index}");
            }
        }
    }
}
