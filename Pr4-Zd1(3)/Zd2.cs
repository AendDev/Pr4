using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Вычисление ln(x) с помощью ряда Тейлора ===\n");

        try
        {
            // Ввод данных с обработкой исключений
            double xStart, xEnd, dx, epsilon;
            Console.Write("Введите начальное значение x (xнач): ");
            if (!double.TryParse(Console.ReadLine(), out xStart) || xStart <= 0)
                throw new ArgumentException("xнач должен быть положительным числом.");

            Console.Write("Введите конечное значение x (xкон): ");
            if (!double.TryParse(Console.ReadLine(), out xEnd) || xEnd <= 0)
                throw new ArgumentException("xкон должен быть положительным числом.");

            Console.Write("Введите шаг dx: ");
            if (!double.TryParse(Console.ReadLine(), out dx) || dx <= 0)
                throw new ArgumentException("Шаг dx должен быть положительным числом.");

            Console.Write("Введите точность вычисления ε (например, 1e-8): ");
            if (!double.TryParse(Console.ReadLine(), out epsilon) || epsilon <= 0)
                throw new ArgumentException("Точность ε должна быть положительной.");

            if (xStart > xEnd)
                throw new ArgumentException("xнач не может быть больше xкон.");

            // Проверка: ряд Тейлора сходится только при x > 0, и хорошо — при x близком к 1
            // Для x очень больших или очень малых — сходимость медленная, но математически корректна
            Console.WriteLine($"\nРасчёт ln(x) для x ∈ [{xStart}, {xEnd}] с шагом {dx} и точностью {epsilon:E8}\n");

            // Вывод таблицы
            PrintTableHeader();

            // Вычисление в режиме checked и unchecked
            Console.WriteLine("Режим: checked (контроль переполнения)");
            Console.WriteLine(new string('=', 60));
            CalculateAndPrintTable(xStart, xEnd, dx, epsilon, true);

            Console.WriteLine("\nРежим: unchecked (без контроля переполнения)");
            Console.WriteLine(new string('=', 60));
            CalculateAndPrintTable(xStart, xEnd, dx, epsilon, false);

            Console.WriteLine("\n✅ Программа завершена успешно.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"\n❌ Ошибка ввода: {ex.Message}");
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"\n❌ Деление на ноль: {ex.Message}");
        }
        catch (OverflowException ex)
        {
            Console.WriteLine($"\n❌ Переполнение арифметической операции: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"\n❌ Ошибка вычислений: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Неожиданная ошибка: {ex.Message}");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    static void PrintTableHeader()
    {
        Console.WriteLine($"{"x",12} | {"ln(x)",18} | {"n_членов",10}");
        Console.WriteLine(new string('-', 45));
    }

    static void CalculateAndPrintTable(double xStart, double xEnd, double dx, double epsilon, bool useChecked)
    {
        for (double x = xStart; x <= xEnd; x
+= dx)
        {
            try
            {
                // Проверка: x должен быть положительным
                if (x <= 0)
                    throw new ArgumentException($"ln(x) не определён при x = {x}.");

                double result;
                int termsCount;

                if (useChecked)
                {
                    // Режим checked — проверка переполнения в арифметике
                    try
                    {
                        checked
                        {
                            (result, termsCount) = CalculateLnTaylor(x, epsilon);
                        }
                    }
                    catch (OverflowException)
                    {
                        // Если переполнение — пробуем в unchecked
                        (result, termsCount) = CalculateLnTaylor(x, epsilon);
                        Console.WriteLine($"{x,12:F6} | {result,18:E8} | {termsCount,10} (⚠️ переполнение в checked, использован unchecked)");
                        continue;
                    }
                }
                else
                {
                    // Режим unchecked
                    (result, termsCount) = CalculateLnTaylor(x, epsilon);
                }

                Console.WriteLine($"{x,12:F6} | {result,18:E8} | {termsCount,10}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"{x,12:F6} | ОШИБКА: {ex.Message} | N/A");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"{x,12:F6} | ОШИБКА: {ex.Message} | N/A");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"{x,12:F6} | ОШИБКА: {ex.Message} | N/A");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{x,12:F6} | НЕИЗВЕСТНАЯ ОШИБКА: {ex.Message} | N/A");
            }
        }
    }

    static (double result, int termsCount) CalculateLnTaylor(double x, double epsilon)
    {
        // Проверка: x > 0
        if (x <= 0)
            throw new ArgumentException("Аргумент x должен быть положительным для ln(x).");

        // Проверка: ряд сходится при x > 0, но для стабильности лучше x ∈ [0.1, 10]
        // При x очень малом или очень большом — ряд сходится медленно, но корректен
        if (x > 1000 || x < 1e-6)
            throw new InvalidOperationException($"Слишком экстремальное значение x = {x}. Рекомендуется x ∈ [0.1, 10].");

        double u = (x - 1) / (x + 1); // u = (x-1)/(x+1), |u| < 1 при x > 0
        if (Math.Abs(u) >= 1)
            throw new InvalidOperationException($"Значение u = (x-1)/(x+1) = {u:F6} не удовлетворяет |u| < 1. Нарушение сходимости.");

        double sum = 0.0;
        double term;
        int n = 0;
        double power = u; // u^(2n+1) = u^1, u^3, u^5...

        // Используем рекуррентное соотношение для эффективности
        // term_n = 2 * u^(2n+1) / (2n+1)
        // term_{n} = term_{n-1} * u^2 * (2n-1)/(2n+1)
        // Но проще пересчитывать степень напрямую

        while (true)
        {
            // Вычисляем текущий член: 2 * u^(2n+1) / (2n+1)
            term = 2 * power / (2 * n + 1);

            // Проверка на NaN или Infinity
            if (double.IsNaN(term) || double.IsInfinity(term))
                throw new InvalidOperationException($"Член ряда стал NaN или Infinity при n = {n}, x = {x}");

            sum += term;

            // Проверка точности: |term| < epsilon
            if (Math.Abs(term) < epsilon)
                break;

            // Следующая степень: u^(2n+3) = u^(2n+1) * u^2
            power *= u * u;
            n++;

            // Защита от бесконечного цикла
            if (n > 10000)
                throw new InvalidOperationException($"Слишком много итераций ({n}) — ряд не сходится с заданной точностью ε = {epsilon:E8} при x = {x}");
        }

        return (sum, n + 1); // n+1 — количество просуммированных членов (n от 0 до n)
    }
}
