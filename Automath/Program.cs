using System.IO;
using Automath.Lexical;
using Automath.Semantic;
using Automath.Syntax;

namespace Automath
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Введите вашу задачу");
            Console.WriteLine("Задать автомат по файлу - 1\nВвести входное слово - 2\nПоказать информацию о автомате - 3\n" +
                "Преобразовать НКА в ДКА - 4\nПреобразовать НКАе в НКА - 5\nЗадание 1 - 6\nЗадание 2 - 7\nЗадание 3 - 8\n" +
                "Закончить работу - 8" +
                "");           
            int zadacha = 0;
            Automath automath=null;
            while (zadacha!=9)
            {
                try
                {
                    zadacha = int.Parse(Console.ReadLine());
                    switch (zadacha)
                    {
                        case 1:
                            Console.WriteLine("Введите полный путь к файлу");
                            string path = Console.ReadLine();
                            if (path == "Отмена")
                            {
                                break;
                            }
                            else
                            {
                                automath = new Automath(path);
                                Console.WriteLine("Автомат успешно инициализирован");
                            }
                            break;
                        case 2:
                            Console.WriteLine("Введите слово");
                            if (automath != null && automath.isCorrectedInisilize)
                            {
                                automath.ProcessInputWord(Console.ReadLine());
                            }
                            else
                            {
                                Console.WriteLine("Автомат не инициализирован или инициализирован не корректно");
                            }
                            break;
                        case 3:
                            if (automath != null && automath.isCorrectedInisilize)
                            {
                                automath.ShowInfo();
                            }
                            else
                            {
                                Console.WriteLine("Автомат не инициализирован или инициализирован не корректно");
                            }
                            break;
                        case 4:
                            if (automath != null && automath.isCorrectedInisilize)
                            {
                                automath=automath.ConvertNKAtoDKA();
                                Console.WriteLine("Автомат успешно преобразован");
                            }
                            else
                            {
                                Console.WriteLine("Автомат не инициализирован или инициализирован не корректно");
                            }
                            break;
                        case 5:
                            if (automath != null && automath.isCorrectedInisilize)
                            {
                                automath = automath.ConvertNKAeToNKA();
                                Console.WriteLine("Автомат успешно преобразован");
                            }
                            else
                            {
                                Console.WriteLine("Автомат не инициализирован или инициализирован не корректно");
                            }
                            break;
                        case 6:
                            Console.WriteLine("Введите полный путь к файлу");
                            LexAnalyzer lexer = new LexAnalyzer();
                            string pathTask1 = Console.ReadLine();
                            lexer.Analyze(pathTask1);
                            break;
                        case 7:
                            Console.WriteLine("Введите полный путь к файлу");
                            LexAnalyzer lexerForTask2 = new LexAnalyzer();
                            string pathTask2 = Console.ReadLine();
                            var lexemesForTask2 = lexerForTask2.Analyze(pathTask2);
                            Syntax.Syntax syntax = new Syntax.Syntax(lexemesForTask2);
                            syntax.Parse();
                            break;
                        case 8:
                            Console.WriteLine("Введите полный путь к файлу");
                            LexAnalyzer lexerForTask3 = new LexAnalyzer();
                            string pathTask3 = Console.ReadLine();
                            var lexemesForTask3 = lexerForTask3.Analyze(pathTask3);
                            Syntax.Syntax syntaxForTask3 = new Syntax.Syntax(lexemesForTask3);
                            syntaxForTask3.Parse();
                            var postfixstring = syntaxForTask3.postfixProcessor.GetPostfixString(lexemesForTask3,);
                            break;
                        case 9:
                            Console.WriteLine("Работа завершена");
                            break;
                        default:
                            Console.WriteLine("Введите корректную задачу");
                            break;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Введите корректную задачу");
                }           
              
            }
            
        }
    }
}