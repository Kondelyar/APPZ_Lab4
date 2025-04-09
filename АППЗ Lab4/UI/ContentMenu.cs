// Клас для роботи з меню
public class ContentMenu
{
    private readonly string _title;
    private readonly IEnumerable<string> _options;

    public ContentMenu(string title, IEnumerable<string> options)
    {
        _title = title;
        _options = options;
    }

    public int Show()
    {
        Console.Clear();
        Console.WriteLine($"===== {_title} =====");

        int optionNumber = 1;
        foreach (var option in _options)
        {
            Console.WriteLine($"{optionNumber}. {option}");
            optionNumber++;
        }

        Console.Write("\nВиберіть опцію: ");

        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= _options.Count())
        {
            return choice;
        }

        return 0; // Неправильний вибір
    }
}