// Реалізація конкретних форм

using ContentLibrary.DAL.Models;

public class BookInputForm : InputForm
{
    public override object GetInputData()
    {
        Console.Clear();
        Console.WriteLine("===== Додавання нової книги =====");

        string title = GetStringInput("Назва");
        string description = GetStringInput("Опис");
        decimal size = GetDecimalInput("Розмір (МБ)");
        ContentFormat format = GetEnumInput<ContentFormat>("Формат");
        string author = GetStringInput("Автор");
        string publisher = GetStringInput("Видавець");
        string isbn = GetStringInput("ISBN");
        int pageCount = GetIntInput("Кількість сторінок");

        return new Book
        {
            Title = title,
            Description = description,
            Size = size,
            Format = format,
            Author = author,
            Publisher = publisher,
            ISBN = isbn,
            PageCount = pageCount
        };
    }
}

public class DocumentInputForm : InputForm
{
    public override object GetInputData()
    {
        Console.Clear();
        Console.WriteLine("===== Додавання нового документа =====");

        string title = GetStringInput("Назва");
        string description = GetStringInput("Опис");
        decimal size = GetDecimalInput("Розмір (МБ)");
        ContentFormat format = GetEnumInput<ContentFormat>("Формат");
        string author = GetStringInput("Автор");
        string documentType = GetStringInput("Тип документа");

        return new Document
        {
            Title = title,
            Description = description,
            Size = size,
            Format = format,
            Author = author,
            DocumentType = documentType
        };
    }
}

public class VideoInputForm : InputForm
{
    public override object GetInputData()
    {
        Console.Clear();
        Console.WriteLine("===== Додавання нового відео =====");

        string title = GetStringInput("Назва");
        string description = GetStringInput("Опис");
        decimal size = GetDecimalInput("Розмір (МБ)");
        ContentFormat format = GetEnumInput<ContentFormat>("Формат");
        string director = GetStringInput("Режисер");
        TimeSpan duration = GetTimeSpanInput("Тривалість");
        string resolution = GetStringInput("Роздільна здатність (наприклад, 1920x1080)");

        return new Video
        {
            Title = title,
            Description = description,
            Size = size,
            Format = format,
            Director = director,
            Duration = duration,
            Resolution = resolution
        };
    }
}

public class AudioInputForm : InputForm
{
    public override object GetInputData()
    {
        Console.Clear();
        Console.WriteLine("===== Додавання нового аудіо =====");

        string title = GetStringInput("Назва");
        string description = GetStringInput("Опис");
        decimal size = GetDecimalInput("Розмір (МБ)");
        ContentFormat format = GetEnumInput<ContentFormat>("Формат");
        string artist = GetStringInput("Виконавець");
        TimeSpan duration = GetTimeSpanInput("Тривалість");

        return new Audio
        {
            Title = title,
            Description = description,
            Size = size,
            Format = format,
            Artist = artist,
            Duration = duration
        };
    }
}

public class StorageInputForm : InputForm
{
    public override object GetInputData()
    {
        Console.Clear();
        Console.WriteLine("===== Додавання нового сховища =====");

        string name = GetStringInput("Назва");
        StorageType type = GetEnumInput<StorageType>("Тип сховища");
        string location = GetStringInput("Розташування");
        decimal capacity = GetDecimalInput("Ємність (ГБ)");

        return new Storage
        {
            Name = name,
            Type = type,
            Location = location,
            Capacity = capacity,
            UsedSpace = 0 // Початково сховище порожнє
        };
    }
}
