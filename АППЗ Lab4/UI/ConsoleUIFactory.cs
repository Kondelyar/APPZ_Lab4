using System;
using System.Collections.Generic;

namespace ContentLibrary.UI
{
    // Фабрика для створення елементів консольного інтерфейсу
    public class ConsoleUIFactory
    {
        public ContentMenu CreateContentMenu(string title, IEnumerable<string> options)
        {
            return new ContentMenu(title, options);
        }

        public BookInputForm CreateBookInputForm()
        {
            return new BookInputForm();
        }

        public DocumentInputForm CreateDocumentInputForm()
        {
            return new DocumentInputForm();
        }

        public VideoInputForm CreateVideoInputForm()
        {
            return new VideoInputForm();
        }

        public AudioInputForm CreateAudioInputForm()
        {
            return new AudioInputForm();
        }

        public StorageInputForm CreateStorageInputForm()
        {
            return new StorageInputForm();
        }
    }
}