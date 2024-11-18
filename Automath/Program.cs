namespace Automath
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Введите вашу задачу");
            Console.WriteLine("Задать автомат по файлу - 1\nВвести входное слово - 2\nПоказать информацию о автомате - 3\nЗакончить работу - 4");           
            int zadacha = 0;
            Automath automath=null;
            while (zadacha!=4)
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
                            Console.WriteLine("Задача 3");
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