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
        public Automath(AutomathType type, string[] states, string[] alphavite, string initialState, string[] finalStates, Dictionary<string, List<string>> transitions, bool isCorrectedInisilize)
        {
            this.type = type;
            this.states = states;
            this.alphavite = alphavite;
            this.initialState = initialState;
            this.finalStates = finalStates;
            this.transitions = transitions;
            this.isCorrectedInisilize = isCorrectedInisilize;
        }
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

                int maxLength = transitions.Values
                           .SelectMany(states => states)
                           .Max(state => state.Length);

                for (int i = 0; i < states.Length + 1; i++)
                {
                    if (i == 0) 
                    {
                        for (int j = 0; j < alphavite.Length + 1; j++)
                        {
                            if (j == 0) Console.Write($" {{0, {-maxLength - 2}}}:\t", "");
                            else Console.Write($"|{{0, {-maxLength - 1}}}", alphavite[j - 1]);
                        }
                    }
                    else 
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
        public void ProcessInputWord(string word)
        {
            switch (type)
            {
                case AutomathType.DKA:
                    ProcessInputWordDKA(word);
                    break;
                case AutomathType.NKA:
                    ProcessInputWordNKA(word);
                    break;
                case AutomathType.NKAe:
                    ProcessInputWordEpsilonNFA(word);
                    break;
                default:
                    Console.WriteLine("У автомата задан неизвестный тип автомата");
                    break;
            }

        }
        public void ProcessInputWordDKA(string word)
        {
            if (!isCorrectedInisilize)
            {
                Console.WriteLine("Ошибка: автомат не был проинициализирован.");
                return;
            }

            string currentState = initialState; 
            Console.WriteLine($"\nНачальное состояние: {currentState}");

            foreach (char symbol in word)
            {
                string symbolStr = symbol.ToString();
                if (!alphavite.Contains(symbolStr))
                {
                    Console.WriteLine($"Ошибка: символ '{symbol}' отсутствует в алфавите.");
                    return;
                }

                if (!transitions.TryGetValue(currentState, out var stateTransitions))
                {
                    Console.WriteLine($"Ошибка: для состояния '{currentState}' не определены переходы.");
                    return;
                }

                int symbolIndex = new List<string>(alphavite).IndexOf(symbolStr);
                if (symbolIndex >= stateTransitions.Count)
                {
                    Console.WriteLine($"Ошибка: для состояния '{currentState}' не существует перехода по символу '{symbol}'.");
                    return;
                }

                string nextState = stateTransitions[symbolIndex];
                if (nextState == "~")
                {
                    Console.WriteLine($"Ошибка: переход из состояния '{currentState}' по символу '{symbol}' не определён.");
                    return;
                }

                Console.WriteLine($"Символ '{symbol}' перевёл автомат из состояния '{currentState}' в '{nextState}'.");
                currentState = nextState;
            }

            Console.WriteLine($"\nКонечное состояние после обработки слова: {currentState}");

            if (finalStates.Contains(currentState))
            {
                Console.WriteLine($"Состояние '{currentState}' является конечным. Слово принимается автоматом.");
            }
            else
            {
                Console.WriteLine($"Состояние '{currentState}' не является конечным. Слово не принимается автоматом.");
            }
        }   

        private bool ProcessInputWordNKA(string word)
        {
            if (!isCorrectedInisilize)
            {
                Console.WriteLine("Ошибка: автомат не был проинициализирован.");
                return false;
            }


            HashSet<string> currentStates = new() { initialState };
            Console.WriteLine($"\nНачальное состояние: {initialState}");

            foreach (char symbol in word)
            {
                string symbolStr = symbol.ToString();

                if (!alphavite.Contains(symbolStr))
                {
                    Console.WriteLine($"Ошибка: символ '{symbol}' отсутствует в алфавите.");
                    return false;
                }

                HashSet<string> nextStates = new();

                foreach (string state in currentStates)
                {
                    if (!transitions.TryGetValue(state, out var stateTransitions))
                    {
                        Console.WriteLine($"Ошибка: для состояния '{state}' не определены переходы.");
                        continue;
                    }

                    int symbolIndex = alphavite.ToList().IndexOf(symbolStr);
                    if (symbolIndex >= stateTransitions.Count)
                    {
                        Console.WriteLine($"Ошибка: для состояния '{state}' не существует перехода по символу '{symbol}'.");
                        continue;
                    }

                    string transition = stateTransitions[symbolIndex];

                    if (transition.Contains("{"))
                    {
                        transition = transition.Trim('{', '}');
                        foreach (string nextState in transition.Split(','))
                        {
                            if (nextState != "~") 
                                nextStates.Add(nextState);
                        }
                    }
                    else if (transition != "~")
                    {
                        nextStates.Add(transition);
                    }
                }

                if (nextStates.Count == 0)
                {
                    Console.WriteLine($"Ошибка: автомат застрял, некуда переходить по символу '{symbol}'.");
                    return false;
                }

                currentStates = nextStates; 
                Console.WriteLine($"Символ '{symbol}' перевёл автомат в состояния: {string.Join(", ", currentStates)}");
            }

            Console.WriteLine($"\nТекущие состояния после обработки слова: {string.Join(", ", currentStates)}");

            if (currentStates.Overlaps(finalStates))
            {
                Console.WriteLine($"Одно из состояний ({string.Join(", ", currentStates.Intersect(finalStates))}) является конечным. Слово принимается автоматом.");
                return true;
            }
            else
            {
                Console.WriteLine($"Ни одно из состояний ({string.Join(", ", currentStates)}) не является конечным. Слово не принимается автоматом.");
                return false;
            }
        }
        private bool ProcessInputWordEpsilonNFA(string word)
        {
            if (!isCorrectedInisilize)
            {
                Console.WriteLine("Ошибка: автомат не был проинициализирован.");
                return false;
            }

            HashSet<string> currentStates = GetEpsilonClosure(new HashSet<string> { initialState });
            Console.WriteLine($"Начальное ε-замыкание: {string.Join(", ", currentStates)}");

            foreach (char symbol in word)
            {
                string symbolStr = symbol.ToString();
                if (!alphavite.Contains(symbolStr))
                {
                    Console.WriteLine($"Ошибка: символ '{symbol}' отсутствует в алфавите.");
                    return false;
                }

                HashSet<string> nextStates = new();
                foreach (string state in currentStates)
                {
                    if (transitions.TryGetValue(state, out var stateTransitions))
                    {
                        int symbolIndex = alphavite.ToList().IndexOf(symbolStr);
                        if (symbolIndex < stateTransitions.Count)
                        {
                            string transition = stateTransitions[symbolIndex];
                            if (transition != "~")
                            {
                                foreach (string nextState in transition.Trim('{', '}').Split(','))
                                    nextStates.Add(nextState);
                            }
                        }
                    }
                }

                currentStates = GetEpsilonClosure(nextStates);
                Console.WriteLine($"После символа '{symbol}': {string.Join(", ", currentStates)}");
            }

            if (currentStates.Overlaps(finalStates))
            {
                Console.WriteLine($"Слово принимается автоматом. Достигнуты конечные состояния: {string.Join(", ", currentStates.Intersect(finalStates))}");
                return true;
            }
            else
            {
                Console.WriteLine("Слово не принимается автоматом.");
                return false;
            }
        }

        private HashSet<string> GetEpsilonClosure(HashSet<string> states)
        {
            Stack<string> stack = new(states);
            HashSet<string> epsilonClosure = new(states);

            while (stack.Count > 0)
            {
                string state = stack.Pop();
                if (transitions.TryGetValue(state, out var stateTransitions))
                {
                    int epsilonIndex = alphavite.ToList().IndexOf("ε");
                    if (epsilonIndex < stateTransitions.Count)
                    {
                        string transition = stateTransitions[epsilonIndex];
                        if (transition != "~")
                        {
                            foreach (string nextState in transition.Trim('{', '}').Split(','))
                            {
                                if (epsilonClosure.Add(nextState)) 
                                    stack.Push(nextState);
                            }
                        }
                    }
                }
            }
            return epsilonClosure;
        }

    }
}
