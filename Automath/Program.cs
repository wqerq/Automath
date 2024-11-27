using System.IO;
using Task1;

namespace Automath
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Введите вашу задачу");
            Console.WriteLine("Задать автомат по файлу - 1\nВвести входное слово - 2\nПоказать информацию о автомате - 3\n" +
                "Преобразовать НКА в ДКА - 4\nПреобразовать НКАе в НКА - 5\nЗадание 1 - 6\nЗакончить работу - 7");           
            int zadacha = 0;
            Automath automath=null;
            while (zadacha!=7)
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
                            Console.WriteLine("Введиет полный путь к файлу");
                            LexAnalyzer lexer = new LexAnalyzer();
                            string pathTask1 = Console.ReadLine();
                            var lexemes = lexer.Analyze(pathTask1);
                            foreach (var item in lexemes)
                            {
                                Console.WriteLine(item);
                            }
                            break;
                        case 7:
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