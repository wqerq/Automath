using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Task1
{
    public class LexAnalyzer
    {
        string[] reservedWords = { "if", "then", "else", "end", "elseif", "or", "and", "not", "output" };
        string[] comparisonOperators = { "<", ">", "=", "<>", ">=", "<=" };
        string[] arithmeticOperators = { "+", "-", "*", "/" };
        string[] delimiters = { ";", "," };
        List<string> messages = new List<string>();
        public List<int> indexes = new List<int>();
        string originalText = "";

        Dictionary<string, List<string>> lexemes = new Dictionary<string, List<string>>();

        private enum States { S, ID, NUM, CMP, MATH, DLM, ERR, F }

        private States currentState;

        private string GetLexemeType(string input)
        {
            if (reservedWords.Contains(input)) return "Reserved Word";
            if (comparisonOperators.Contains(input)) return "Comparison Operator";
            if (arithmeticOperators.Contains(input)) return "Arithmetic Operator";
            if (delimiters.Contains(input)) return "Delimiter";
            if (char.IsDigit(input[0])) return "Constant";
            if (char.IsLetter(input[0])) return "Identifier";
            return "Unknown";
        }

        public List<string> Analyze(string path)
        {
            int i = 0;
            int start = 0;
            string text;
            bool isOk = true;
            string unknownLex = "";
            int unknownPos = -1;
            List<string> orderedLexemesOut = new List<string>();

            using (StreamReader file = new StreamReader(path))
            {
                text = file.ReadToEnd();
            }

            Console.Write($"Исходный текст: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ResetColor();
            Console.WriteLine("\n");

            originalText = text;
            text = text.ToLower() + " ";
            currentState = States.S;

            while (currentState != States.F && currentState != States.ERR)
            {
                States prevState = currentState;
                bool add = true;

                switch (currentState)
                {
                    case States.S:
                        if (char.IsLetter(text[i]))
                            currentState = States.ID;
                        else if (char.IsDigit(text[i]))
                            currentState = States.NUM;
                        else if (comparisonOperators.Any(op => op.StartsWith(text[i].ToString())))
                            currentState = States.CMP;
                        else if (arithmeticOperators.Contains(text[i].ToString()))
                            currentState = States.MATH;
                        else if (delimiters.Contains(text[i].ToString()))
                            currentState = States.DLM;
                        else if (char.IsWhiteSpace(text[i]))
                            add = false;
                        else
                        {
                            currentState = States.ERR;
                            isOk = false;
                            unknownLex = text[i].ToString();
                            unknownPos = i;
                        }
                        break;

                    case States.ID:
                        if (!char.IsLetterOrDigit(text[i]))
                        {
                            currentState = States.S;
                        }
                        else
                        {
                            add = false;
                        }
                        break;

                    case States.NUM:
                        if (!char.IsDigit(text[i]))
                        {
                            currentState = States.S;
                        }
                        else
                        {
                            add = false;
                        }
                        break;

                    case States.CMP:
                        string potentialCmp = text.Substring(start, i - start + 1);
                        if (!comparisonOperators.Contains(potentialCmp))
                        {
                            if (comparisonOperators.Any(op => op.StartsWith(potentialCmp)))
                            {
                                add = false;
                            }
                            else
                            {
                                currentState = States.S;
                            }
                        }
                        break;

                    case States.MATH:
                        currentState = States.S;
                        break;

                    case States.DLM:
                        currentState = States.S;
                        break;
                }

                if (add)
                {
                    string message = text.Substring(start, i - start).Trim();
                    if (message.Length > 0)
                    {
                        messages.Add(message);
                        indexes.Add(start);
                        Console.WriteLine($"{message} \t| {GetLexemeType(message)}");
                        orderedLexemesOut.Add($"{message} {GetLexemeType(message)}");
                    }
                    start = i;
                }

                if (currentState == States.ERR)
                {
                    Console.WriteLine($"Ошибка: неизвестная лексема '{unknownLex}' на позиции {unknownPos}");
                    return new List<string>();
                }

                if (currentState != States.F)
                    i++;
            }

            Console.WriteLine("\nТаблица лексем:");
            foreach (var message in messages.Distinct())
            {
                Console.WriteLine($"{message} \t| {GetLexemeType(message)}");
            }

            return orderedLexemesOut;
        }
    }
}
