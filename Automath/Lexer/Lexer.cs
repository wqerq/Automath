using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Automath.Lexical
{
    public class LexAnalyzer
    {
        private readonly string[] reservedWords = { "if", "then", "elseif", "else", "end", "not", "and", "or", "output" };
        private readonly string[] comparisonOperators = { "<", ">", "==", "<>", ">=", "<=" };
        private readonly string[] arithmeticOperators = { "+", "-", "*", "/" };
        private readonly string[] delimiters = { ";", "," };
        private readonly List<string> messages = new List<string>();
        public List<int> indexes = new List<int>();
        private string originalText = "";
        private readonly Dictionary<string, List<string>> lexemes = new Dictionary<string, List<string>>();
        private bool useRussianLabels = false; 

        private enum States { S, ID, NUM, CMP, MATH, DLM, LOGIC, ERR, F }
        private States currentState;

        private string GetLexemeTypeEnglish(string input)
        {
            if (input == "if") return "IF";
            else if (input == "then") return "THEN";
            else if (input == "else") return "ELSE";
            else if (input == "end") return "END";
            else if (input == "elseif") return "ELSEIF";
            else if (input == "output") return "OUTPUT";
            else if (input == "and") return "AND";
            else if (input == "or") return "OR";
            else if (input == "not") return "NOT";
            else if (comparisonOperators.Contains(input)) return "REL";
            else if (arithmeticOperators.Contains(input)) return "MATH";
            else if (delimiters.Contains(input)) return "DLM";
            else if (input == "=") return "ASGN";
            else if (char.IsDigit(input[0])) return "NUM";
            else if (char.IsLetter(input[0])) return "ID";
            else return "UNKNOWN";
        }

        private string GetLexemeTypeRussian(string input)
        {
            if (reservedWords.Contains(input)) return "Ключевое слово";
            else if (comparisonOperators.Contains(input)) return "Операция сравнения";
            else if (arithmeticOperators.Contains(input)) return "Арифметическая операция";
            else if (delimiters.Contains(input)) return "Разделитель";
            else if (input == "=") return "Присваивание";
            else if (char.IsDigit(input[0])) return "Константа";
            else if (char.IsLetter(input[0])) return "Идентификатор";
            else return "Неизвестно";
        }

        private string GetLexemeType(string input)
        {
            return useRussianLabels ? GetLexemeTypeRussian(input) : GetLexemeTypeEnglish(input);
        }

        public List<string> Analyze(string path, bool useRussian = false)
        {
            useRussianLabels = useRussian;
            int i = 0, start = 0;
            string text;
            bool isOk = true;
            string unknownLex = "";
            int unknownPos = -1;
            var orderedLexemesOut = new List<string>();

            using (StreamReader file = new StreamReader(path))
            {
                text = file.ReadToEnd();
            }

            Console.WriteLine($"Исходный текст:");
            Console.WriteLine(text);
            Console.WriteLine();

            originalText = text;
            text = text.ToLower() + " ";
            currentState = States.S;

            while (currentState != States.F && currentState != States.ERR)
            {
                if (i >= text.Length - 1)
                {
                    if (currentState != States.S)
                    {
                        string message = text.Substring(start, i - start).Trim();
                        if (!string.IsNullOrEmpty(message))
                        {
                            messages.Add(message);
                            indexes.Add(start);
                        }
                    }
                    currentState = States.F;
                    break;
                }

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
                            string lexeme = text.Substring(start, i - start).Trim();
                            if (reservedWords.Contains(lexeme) && lexeme == "end")
                            {
                                currentState = States.F; 
                                add = false;
                                break;
                            }
                            currentState = States.S;
                        }
                        else
                            add = false;
                        break;

                    case States.NUM:
                        if (!char.IsDigit(text[i]))
                            currentState = States.S;
                        else
                            add = false;
                        break;

                    case States.CMP:
                        string potentialCmp = text.Substring(start, i - start + 1);
                        if (!comparisonOperators.Contains(potentialCmp))
                        {
                            if (comparisonOperators.Any(op => op.StartsWith(potentialCmp)))
                                add = false;
                            else
                                currentState = States.S;
                        }
                        break;

                    case States.MATH:
                        currentState = States.S;
                        break;

                    case States.DLM:
                        currentState = States.S;
                        break;

                    case States.ERR:
                        Console.WriteLine($"Ошибка: неизвестная лексема '{unknownLex}' на позиции {unknownPos}");
                        return new List<string>();
                }

                if (add)
                {
                    string message = text.Substring(start, i - start).Trim();
                    if (message.Length > 0)
                    {
                        messages.Add(message);
                        indexes.Add(start);
                    }
                    start = i;
                }

                i++;
            }

            if (isOk)
            {
                useRussianLabels = true;
                Console.WriteLine("\nКлассификация лексем:");
                int j = 0;
                foreach (string item in messages)
                {
                    useRussianLabels = true;
                    string type = GetLexemeType(item);
                    if (!lexemes.ContainsKey(type))
                    {
                        lexemes[type] = new List<string>();
                    }

                    if (!lexemes[type].Contains(item))
                    {
                        lexemes[type].Add(item);
                    }
                    Console.WriteLine($"{item} \t| {type}");
                    useRussianLabels = false;
                    orderedLexemesOut.Add($"{indexes[j]} {item} {GetLexemeType(item)}");
                    j++;
                }
            }

            Console.WriteLine("\nТаблица лексем:");
            foreach (var pair in lexemes)
            {
                Console.WriteLine($"{pair.Key}: {string.Join(", ", pair.Value)}");
            }

            return orderedLexemesOut;
        }

    }
}
