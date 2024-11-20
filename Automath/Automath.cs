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
        Dictionary<string, List<string>> transitions;
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
                    if(type== AutomathType.NKAe)
                    {
                        alphavite = alphavite.Append("eps").ToArray();
                    }
                    initialState = file.ReadLine().Trim('Q','0',':',' ');
                    finalStates= file.ReadLine().Trim('F', ':').Split(",").Select(s => s.Trim()).ToArray();
                    if(file.ReadLine()=="Table:")
                    {
                        transitions = new Dictionary<string, List<string>>();
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
                    ProcessInputWordEpsilonNKA(word);
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
        private bool ProcessInputWordEpsilonNKA(string word)
        {
            if (!isCorrectedInisilize)
            {
                Console.WriteLine("Операция 'ProcessInputLine' не может быть выполнена: автомат не проинициализирован.");
                return false;
            }


            var inputsList = alphavite.ToList();
            var currentStates = EpsilonClosure(new HashSet<string> { initialState });
            Console.WriteLine($"\nТекущее состояние: {string.Join(", ", currentStates)}");

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
                    var epsTrans = stateTransitions[stateTransitions.Count() - 1];
                    
                    int symbolIndex = alphavite.ToList().IndexOf(symbolStr);
                    if (symbolIndex >= stateTransitions.Count)
                    {
                        Console.WriteLine($"Ошибка: для состояния '{state}' не существует перехода по символу '{symbol}'.");
                        continue;
                    }

                    string transition = stateTransitions[symbolIndex];

                    if (transition.Contains("{"))
                    {
                        foreach (string nextState in transition.Trim('{', '}').Split(','))
                        {
                            string transitionEps = transitions[state][symbolIndex];
                            if (transitionEps != "~")
                            {
                                nextStates.UnionWith(EpsilonClosure(new HashSet<string> { transitionEps }));
                            }
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

            if (currentStates.Overlaps(finalStates))
            {
                Console.WriteLine("Одно из достигнутых состояний входит в число финальных.");
                return true;
            }

            Console.WriteLine("Ни одно из достигнутых состояний не входит в число финальных.");
            return false;
        }

        private HashSet<string> EpsilonClosure(HashSet<string> states)
        {
            HashSet<string> closure = new HashSet<string>(states);
            Queue<string> queue = new Queue<string>(states);

            while (queue.Count > 0)
            {
                string state = queue.Dequeue();
                if (transitions.ContainsKey(state))
                {
                    string epsTransition = transitions[state].Last();

                    if (epsTransition == "~")
                        continue;

                    foreach (string nextState in epsTransition.Trim('{','}').Split(','))
                    {
                        if (!closure.Contains(nextState))
                        {
                            closure.Add(nextState);
                            queue.Enqueue(nextState);
                        }
                    }
                }
            }
            return closure;
        }

        public Automath ConvertNKAtoDKA()
        {
            if (type != AutomathType.NKA)
            {
                throw new InvalidOperationException("Автомат должен быть НКА для преобразования в ДКА.");
            }

            var dkaStates = new List<HashSet<string>>();
            var dkaTransitions = new Dictionary<string, List<string>>();
            var stateMapping = new Dictionary<string, HashSet<string>>();

            var initialSet = new HashSet<string> { initialState };
            dkaStates.Add(initialSet);
            stateMapping[GetStateName(initialSet)] = initialSet;

            var queue = new Queue<HashSet<string>>();
            queue.Enqueue(initialSet);

            while (queue.Count > 0)
            {
                var currentSet = queue.Dequeue();
                var currentStateName = GetStateName(currentSet);

                if (!dkaTransitions.ContainsKey(currentStateName))
                {
                    dkaTransitions[currentStateName] = new List<string>();
                }

                foreach (var symbol in alphavite)
                {
                    var nextStates = new HashSet<string>();

                    foreach (var state in currentSet)
                    {
                        if (transitions.TryGetValue(state, out var stateTransitions))
                        {
                            var index = Array.IndexOf(alphavite, symbol);
                            if (index >= 0 && index < stateTransitions.Count)
                            {
                                var transition = stateTransitions[index];
                                if (transition.Contains("{"))
                                {
                                    nextStates.UnionWith(transition.Trim('{', '}').Split(','));
                                }
                                else if (transition != "~")
                                {
                                    nextStates.Add(transition);
                                }
                            }
                        }
                    }

                    var nextStateName = GetStateName(nextStates);

                    if (!stateMapping.ContainsKey(nextStateName) && nextStates.Count > 0)
                    {
                        stateMapping[nextStateName] = nextStates;
                        dkaStates.Add(nextStates);
                        queue.Enqueue(nextStates);
                    }

                    dkaTransitions[currentStateName].Add(nextStateName);
                }
            }

            var dkaFinalStates = stateMapping.Where(pair => pair.Value.Overlaps(finalStates))
                                             .Select(pair => pair.Key)
                                             .ToArray();

            return new Automath
            {
                type = AutomathType.DKA,
                states = dkaStates.Select(GetStateName).ToArray(),
                alphavite = alphavite,
                initialState = GetStateName(initialSet),
                finalStates = dkaFinalStates,
                transitions = dkaTransitions,
                isCorrectedInisilize = true
            };
        }

        public Automath ConvertNKAeToNKA()
        {
            if (type != AutomathType.NKAe)
            {
                throw new InvalidOperationException("Автомат должен быть НКА-е для преобразования в НКА.");
            }

            var nkaTransitions = new Dictionary<string, List<string>>();

            foreach (var state in states)
            {
                nkaTransitions[state] = new List<string>(new string[alphavite.Length - 1]);

                foreach (var symbol in alphavite.Take(alphavite.Length - 1)) 
                {
                    var symbolIndex = Array.IndexOf(alphavite, symbol);
                    var reachableStates = EpsilonClosure(new HashSet<string> { state });
                    var nextStates = new HashSet<string>();

                    foreach (var reachableState in reachableStates)
                    {
                        if (transitions.TryGetValue(reachableState, out var stateTransitions))
                        {
                            var transition = stateTransitions[symbolIndex];
                            if (transition.Contains("{"))
                            {
                                nextStates.UnionWith(transition.Trim('{', '}').Split(','));
                            }
                            else if (transition != "~")
                            {
                                nextStates.Add(transition);
                            }
                        }
                    }

                    nkaTransitions[state][symbolIndex] = $"{{{string.Join(",", nextStates)}}}";
                }
            }

            return new Automath
            {
                type = AutomathType.NKA,
                states = states,
                alphavite = alphavite.Take(alphavite.Length - 1).ToArray(),
                initialState = initialState,
                finalStates = finalStates,
                transitions = nkaTransitions,
                isCorrectedInisilize = true
            };
        }
        private string GetStateName(HashSet<string> stateSet)
        {
            return "{" + string.Join(",", stateSet.OrderBy(s => s)) + "}";
        }
    }
}
