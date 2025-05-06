// Converters.cs
using AutoMapper;
using ContentLibrary.BLL.Models;
using System;

namespace ContentLibrary.UI.Mapping
{
    public class ContentTypeDtoConverter : ITypeConverter<string, ContentTypeDto>
    {
        public ContentTypeDto Convert(string source, ContentTypeDto destination, ResolutionContext context)
        {
            return Enum.TryParse(source, out ContentTypeDto result) ? result : ContentTypeDto.Book;
        }
    }

    public class ContentFormatDtoConverter : ITypeConverter<string, ContentFormatDto>
    {
        public ContentFormatDto Convert(string source, ContentFormatDto destination, ResolutionContext context)
        {
            return Enum.TryParse(source, out ContentFormatDto result) ? result : ContentFormatDto.PDF;
        }
    }

    public class StorageTypeDtoConverter : ITypeConverter<string, StorageTypeDto>
    {
        public StorageTypeDto Convert(string source, StorageTypeDto destination, ResolutionContext context)
        {
            return Enum.TryParse(source, out StorageTypeDto result) ? result : StorageTypeDto.LocalDisk;
        }
    }
}