using System;
using System.Numerics;

class Zd1
{
    static void Main()
    {
        Console.WriteLine("=== Программа для сложения больших целых чисел и вычисления функции ===\n");

        // Часть 1: Сложение больших целых чисел с проверкой переполнения
        TryBigIntegerAddition();

        Console.WriteLine("\n" + new string('=', 60) + "\n");

        // Часть 2: Вычисление функции f(x) = 7*sin²(x) - (1/(2x))*cos(x)
        TryFunctionCalculation();
    }

    static void TryBigIntegerAddition()
    {
        Console.WriteLine("ЧАСТЬ 1: Сложение больших целых чисел (> 10^9)");

        try
        {
            Console.Write("Введите первое большое целое число: ");
            string input1 = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input1))
                throw new ArgumentException("Ввод не может быть пустым.");

            Console.Write("Введите второе большое целое число: ");
            string input2 = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input2))
                throw new ArgumentException("Ввод не может быть пустым.");

            BigInteger num1, num2;
            if (!BigInteger.TryParse(input1, out num1))
                throw new FormatException($"Неверный формат первого числа: {input1}");
            if (!BigInteger.TryParse(input2, out num2))
                throw new FormatException($"Неверный формат второго числа: {input2}");

            // Проверка: числа должны быть больше 10^9
            if (num1 <= BigInteger.One * 1000000000)
                throw new ArgumentException($"Первое число ({num1}) не больше 10^9.");
            if (num2 <= BigInteger.One * 1000000000)
                throw new ArgumentException($"Второе число ({num2}) не больше 10^9.");

            Console.WriteLine("\n--- Режим с контролем переполнения (checked) ---");
            try
            {
                checked
                {
                    int a = (int)num1 % 1000000;
                    int b = (int)num2 % 1000000;
                    int sum = checked(a + b);
                    Console.WriteLine($"Сумма (int-версия): {a} + {b} = {sum}");
                }
            }
            catch (OverflowException ex)
            {
                Console.WriteLine($"⚠️ Переполнение в режиме checked: {ex.Message}");
            }

            Console.WriteLine("\n--- Режим без контроля переполнения (unchecked) ---");
            try
            {
                unchecked
                {
                    int a = (int)num1 % 1000000;
                    int b = (int)num2 % 1000000;
                    int sum = a + b; 
                    Console.WriteLine($"Сумма (int-версия): {a} + {b} = {sum} (переполнение игнорируется)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в unchecked: {ex.Message}");
            }

            BigInteger result = num1 + num2;
            Console.WriteLine($"\n✅ Истинная сумма (BigInteger): {num1} + {num2} = {result}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"❌ Ошибка аргумента: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Неверный формат числа: {ex.Message}");
        }
        catch (OverflowException ex)
        {
            Console.WriteLine($"❌ Переполнение: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Неизвестная ошибка: {ex.Message}");
        }
    }

    static void TryFunctionCalculation()
    {
        Console.WriteLine("ЧАСТЬ 2: Вычисление функции f(x) = 7*sin²(x) - (1/(2x))*cos(x)");

        try
        {
            Console.Write("Введите начало отрезка a: ");
            string aStr = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(aStr))
                throw new ArgumentException("Начало отрезка не может быть пустым.");

            Console.Write("Введите конец отрезка b: ");
            string bStr = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(bStr))
                throw new ArgumentException("Конец отрезка не может быть пустым.");

            Console.Write("Введите шаг h: ");
            string hStr = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(hStr))
                throw new ArgumentException("Шаг не может быть пустым.");

            if (!double.TryParse(aStr, out double a) || !double.TryParse(bStr, out double b) || !double.TryParse(hStr, out double h))
                throw new FormatException("Все значения должны быть числами.");

            if (h <= 0)
                throw new ArgumentException("Шаг h должен быть положительным.");

            if (a > b)
                throw new ArgumentException("Начало отрезка a не может быть больше конца b.");

            if (a <= 0 && b >= 0)
                throw new ArgumentException("Отрезок [a,b] не может содержать x=0, так как функция не определена при x=0.");

            Console.WriteLine("\nТаблица значений функции f(x) = 7*sin²(x) - (1/(2x))*cos(x):");
            Console.WriteLine($"{"x",12} | {"f(x)",18}");
            Console.WriteLine(new string('-', 35));

            // Генерация исключения если шаг слишком мал
            if (h < 1e-10)
                throw new ArgumentException("Шаг слишком мал — вычисления нестабильны.");

            for (double x = a; x <= b; x += h)
            {
                try
                {
                    // Проверка деления на ноль
                    if (Math.Abs(x) < 1e-15)
                        throw new DivideByZeroException($"Значение x = {x} слишком близко к нулю.");

                    double sinX = Math.Sin(x);
                    double cosX = Math.Cos(x);
                    double term1 = 7 * sinX * sinX; // 7 * sin²(x)
                    double term2 = (1.0 / (2.0 * x)) * cosX; // (1/(2x)) * cos(x)
                    double result = term1 - term2;

                    // Проверка на Infinity
                    if (double.IsNaN(result))
                        throw new ArithmeticException($"Функция вернула NaN при x = {x}");
                    if (double.IsInfinity(result))
                        throw new ArithmeticException($"Функция вернула бесконечность при x = {x}");

                    Console.WriteLine($"{x,12:F6} | {result,18:F8}");
                }
                catch (DivideByZeroException ex)
                {
                    Console.WriteLine($"{x,12:F6} | ОШИБКА: {ex.Message}");
                }
                catch (ArithmeticException ex)
                {
                    Console.WriteLine($"{x,12:F6} | ОШИБКА: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{x,12:F6} | НЕИЗВЕСТНАЯ ОШИБКА: {ex.Message}");
                }
            }

            if (a == b)
                Console.WriteLine("(Только одна точка)");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"❌ Ошибка ввода: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Неверный формат числа: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Неожиданная ошибка: {ex.Message}");
        }
    }
}
