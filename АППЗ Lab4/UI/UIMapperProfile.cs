using AutoMapper;
using ContentLibrary.BLL.Models;
using ContentLibrary.UI.ViewModels;
using System;

namespace ContentLibrary.UI.Mapping
{
    // Профіль для мапінгу DTO -> ViewModel
    public class DtoToViewModelMappingProfile : Profile
    {
        public DtoToViewModelMappingProfile()
        {
            // Мапінг для типу контенту і формату
            CreateMap<ContentTypeDto, string>().ConvertUsing(src => src.ToString());
            CreateMap<ContentFormatDto, string>().ConvertUsing(src => src.ToString());
            CreateMap<StorageTypeDto, string>().ConvertUsing(src => src.ToString());

            // Мапінг для контентних моделей
            CreateMap<ContentItemDto, ContentItemViewModel>()
                .Include<BookDto, BookViewModel>()
                .Include<DocumentDto, DocumentViewModel>()
                .Include<VideoDto, VideoViewModel>()
                .Include<AudioDto, AudioViewModel>();

            CreateMap<BookDto, BookViewModel>();
            CreateMap<DocumentDto, DocumentViewModel>();
            CreateMap<VideoDto, VideoViewModel>();
            CreateMap<AudioDto, AudioViewModel>();

            // Мапінг для сховища
            CreateMap<StorageDto, StorageViewModel>();

            // Мапінг для ContentStorage
            CreateMap<ContentStorageDto, ContentStorageViewModel>()
                .ForMember(dest => dest.ContentTitle, opt => opt.Ignore())
                .ForMember(dest => dest.StorageName, opt => opt.Ignore());
        }
    }

    // Профіль для мапінгу ViewModel -> DTO
    public class ViewModelToDtoMappingProfile : Profile
    {
        public ViewModelToDtoMappingProfile()
        {
            // Виправлений мапінг для рядкового представлення в енуми (без CS8198)
            CreateMap<string, ContentTypeDto>().ConvertUsing<ContentTypeDtoConverter>();
            CreateMap<string, ContentFormatDto>().ConvertUsing<ContentFormatDtoConverter>();
            CreateMap<string, StorageTypeDto>().ConvertUsing<StorageTypeDtoConverter>();

            // Мапінг для контентних моделей
            CreateMap<BookViewModel, BookDto>();
            CreateMap<DocumentViewModel, DocumentDto>();
            CreateMap<VideoViewModel, VideoDto>();
            CreateMap<AudioViewModel, AudioDto>();

            // Мапінг для сховища
            CreateMap<StorageViewModel, StorageDto>();

            // Мапінг для ContentStorage
            CreateMap<ContentStorageViewModel, ContentStorageDto>();
        }
    }

    // Окремі класи конвертерів для уникнення помилки CS8198
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