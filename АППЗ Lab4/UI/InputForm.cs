// Базовий абстрактний клас для всіх форм введення
using ContentLibrary.DAL.Models;

public abstract class InputForm
{
    public abstract object GetInputData();
    protected string GetStringInput(string prompt)
    {
        Console.Write($"{prompt}: ");
        return Console.ReadLine() ?? string.Empty;
    }

    protected int GetIntInput(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            if (int.TryParse(Console.ReadLine(), out int result))
            {
                return result;
            }
            Console.WriteLine("Неправильний формат числа. Спробуйте ще раз.");
        }
    }

    protected decimal GetDecimalInput(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal result))
            {
                return result;
            }
            Console.WriteLine("Неправильний формат числа. Спробуйте ще раз.");
        }
    }

    protected DateTime GetDateTimeInput(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime result))
            {
                return result;
            }
            Console.WriteLine("Неправильний формат дати. Спробуйте ще раз.");
        }
    }

    protected TimeSpan GetTimeSpanInput(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (hh:mm:ss): ");
            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan result))
            {
                return result;
            }
            Console.WriteLine("Неправильний формат часу. Спробуйте ще раз.");
        }
    }

    protected T GetEnumInput<T>(string prompt) where T : struct, Enum
    {
        while (true)
        {
            Console.WriteLine($"{prompt}:");
            var values = Enum.GetValues(typeof(T));

            for (int i = 0; i < values.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {values.GetValue(i)}");
            }

            Console.Write("Виберіть опцію: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= values.Length)
            {
                return (T)values.GetValue(choice - 1);
            }
            Console.WriteLine("Неправильний вибір. Спробуйте ще раз.");
        }
    }
}