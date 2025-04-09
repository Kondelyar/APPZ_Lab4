namespace ContentLibrary.DAL.Models
{

    public enum ContentType
    {
        Book,
        Document,
        Video,
        Audio
    }

    // Enum для форматів контенту
    public enum ContentFormat
    {
        // Книги та документи
        PDF,
        EPUB,
        DOCX,
        TXT,

        // Відео
        MP4,
        AVI,
        MKV,

        // Аудіо
        MP3,
        WAV,
        FLAC
    }

    // Enum для типів сховищ
    public enum StorageType
    {
        LocalDisk,
        NetworkDrive,
        Cloud,
        ExternalDrive
    }
}