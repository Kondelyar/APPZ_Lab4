//using ContentLibrary.DAL.Models;
//using System;

//namespace ContentLibrary.DAL.Patterns
//{
//    // Фабрика для створення різних типів контенту
//    public class ContentFactory
//    {
//        public Book CreateBook(
//            string title,
//            string description,
//            decimal size,
//            ContentFormat format,
//            string author,
//            string publisher,
//            string isbn,
//            int pageCount)
//        {
//            return new Book
//            {
//                Title = title,
//                Description = description,
//                Size = size,
//                Format = format,
//                Author = author,
//                Publisher = publisher,
//                ISBN = isbn,
//                PageCount = pageCount
//            };
//        }

//        public Document CreateDocument(
//            string title,
//            string description,
//            decimal size,
//            ContentFormat format,
//            string author,
//            string documentType)
//        {
//            return new Document
//            {
//                Title = title,
//                Description = description,
//                Size = size,
//                Format = format,
//                Author = author,
//                DocumentType = documentType
//            };
//        }

//        public Video CreateVideo(
//            string title,
//            string description,
//            decimal size,
//            ContentFormat format,
//            string director,
//            TimeSpan duration,
//            string resolution)
//        {
//            return new Video
//            {
//                Title = title,
//                Description = description,
//                Size = size,
//                Format = format,
//                Director = director,
//                Duration = duration,
//                Resolution = resolution
//            };
//        }

//        public Audio CreateAudio(
//            string title,
//            string description,
//            decimal size,
//            ContentFormat format,
//            string artist,
//            TimeSpan duration)
//        {
//            return new Audio
//            {
//                Title = title,
//                Description = description,
//                Size = size,
//                Format = format,
//                Artist = artist,
//                Duration = duration
//            };
//        }
//    }
//}