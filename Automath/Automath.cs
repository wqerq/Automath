using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automath
{
    internal class Automath
    {
        AutomathType type;
        string[] states;
        string[] alphavite;
        string initialState;
        string[] finalStates;
        Dictionary<string,List<string>> transitions=new Dictionary<string, List<string>>();
        public bool isCorrectedInisilize=true;

        public Automath() { }
        public Automath(string filepath) 
        {
            using(StreamReader file=new StreamReader(filepath))
            {
                while(!file.EndOfStream) {
                        string typestring=file.ReadLine();
                        switch (typestring)
                        {
                            case "НКА":
                                type = AutomathType.NKA; break;
                            case "ДКА":
                                type = AutomathType.DKA; break;
                            case "НКАе":
                                type = AutomathType.NKAe; break;
                            default:
                                isCorrectedInisilize = false;
                                break;
                        }
                    states = file.ReadLine().Trim('Q',':').Split(",").Select(s => s.Trim()).ToArray();
                    alphavite= file.ReadLine().Trim('S', ':').Split(",").Select(s => s.Trim()).ToArray();
                    initialState = file.ReadLine().Trim('Q','0',':',' ');
                    finalStates= file.ReadLine().Trim('F', ':').Split(",").Select(s => s.Trim()).ToArray();
                    if(file.ReadLine()=="Table:")
                    {
                        foreach (var state in states)
                        {
                            string[] transitionsElements = file.ReadLine().Split(" ");
                            transitions.Add(state, transitionsElements.ToList());
                        }

                    }
                    else
                    {
                        isCorrectedInisilize = false;
                    }
                }
            }
        }
        public void ShowInfo()
        {
            if (isCorrectedInisilize)
            {
                if (type == AutomathType.DKA) Console.WriteLine("Детерминированный КА");
                else if (type == AutomathType.NKA) Console.WriteLine("Недетерминированный КА");
                else Console.WriteLine("Недетерминированный КА с е-переходами");
                Console.ResetColor();


                Console.Write("Состояния: ");
                foreach (string word in states)
                {
                    Console.Write(word + " ");
                }
                Console.ResetColor();
                Console.WriteLine();


                Console.Write("Алфавит: ");
                foreach (string word in alphavite)
                {
                    Console.Write(word + " ");
                }
                Console.ResetColor();
                Console.WriteLine();

                Console.Write("Начальное состояние: ");
                Console.WriteLine(initialState);
                Console.ResetColor();


                Console.Write("Финальное(ые) состояние(я): ");
                foreach (string word in finalStates)
                {
                    Console.Write(word + " ");
                }
                Console.ResetColor();
                Console.WriteLine();

                Console.WriteLine("Таблица переходов автомата:");

                int maxLength = MaxLengthForTable();

                for (int i = 0; i < states.Length + 1; i++)
                {
                    if (i == 0) // для первой строки
                    {
                        for (int j = 0; j < alphavite.Length + 1; j++)
                        {
                            if (j == 0) Console.Write($" {{0, {-maxLength - 2}}}:\t", "");
                            else Console.Write($"|{{0, {-maxLength - 1}}}", alphavite[j - 1]);
                        }
                    }
                    else // для остальных строк
                    {
                        for (int j = 0; j < alphavite.Length + 1; j++)
                        {
                            if (j == 0)
                            {
                                if (states[i - 1] == initialState && finalStates.Contains(states[i - 1]))
                                {
                                    Console.Write($"->*{{0, {-maxLength}}}:\t", states[i - 1]);
                                }
                                else if (states[i - 1] == initialState)
                                {
                                    Console.Write($"->{{0, {-maxLength - 1}}}:\t", states[i - 1]);
                                }
                                else if (finalStates.Contains(states[i - 1]))
                                {
                                    Console.Write($" *{{0, {-maxLength - 1}}}:\t", states[i - 1]);
                                }
                                else Console.Write($"  {{0, {-maxLength - 1}}}:\t", states[i - 1]);
                            }
                            else Console.Write($"|{{0, {-maxLength - 1}}}", transitions[states[i - 1]][j - 1]);
                        }
                    }
                    Console.WriteLine();
                }
            }
        }

        private int MaxLengthForTable()
        {
            int maxLength = 0;
            foreach (var item in transitions.Values)
            {
                foreach (string state in item)
                {
                    if (maxLength < state.Length) maxLength = state.Length;
                }
            }
            return maxLength;
        }

        public void TryInputWord(string word)
        {
            bool isOk = false;
            if (isCorrectedInisilize)
            {
                bool EmergencyBreak = false;
                string currentState = initialState;
                List<string> inputsList = alphavite.ToList();

                Console.WriteLine($"\nТекущее состояние: {currentState}");

                foreach (char symbol in word)
                {
                    if (alphavite.Contains(symbol.ToString()))
                    {
                        Console.WriteLine($"Считан символ '{symbol}'");
                        string prevState = currentState;
                        currentState = transitions[currentState][inputsList.IndexOf(symbol.ToString())];
                        // Обработка неопределённого состояния
                        if (currentState == "~")
                        {
                            Console.WriteLine("Запрашиваемое входным символом состояние не определено.\n" +
                                $"Из состояния {prevState} нет перехода по символу {symbol}");
                            Console.ResetColor();
                            EmergencyBreak = true;
                            break;
                        }
                        Console.WriteLine($" - Текущее состояние теперь {currentState}");
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка! Считанный символ '{symbol}' не входит в алфавит!");
                        Console.ResetColor();
                        EmergencyBreak = true;
                        break;
                    }
                }

                if (!EmergencyBreak)
                {
                    Console.WriteLine($"\nВходное слово успешно прочитано. Автомат пришёл в состояние {currentState}");
                    if (finalStates.ToList().Contains(currentState))
                    {
                        Console.WriteLine($"Состояние {currentState} входит в число финальных состояний.");
                        Console.ResetColor();
                        isOk = true;
                    }
                    else
                    {
                        Console.WriteLine($"Состояние {currentState} не входит в число финальных состояний.");
                        Console.ResetColor();
                    }
                }
            }
            else
            {
                Console.WriteLine("Операция 'ProcessInputLine' не может быть выполнена: автомат не проинициализирован.");
                Console.ResetColor();
            }
        }
    }
}
