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

    // Оновлений профіль для мапінгу ViewModel -> DTO
    public class ViewModelToDtoMappingProfile : Profile
    {
        public ViewModelToDtoMappingProfile()
        {
            // Використання методів замість лямбда-виразів
            CreateMap<string, ContentTypeDto>().ConvertUsing(ConvertToContentTypeDto);
            CreateMap<string, ContentFormatDto>().ConvertUsing(ConvertToContentFormatDto);
            CreateMap<string, StorageTypeDto>().ConvertUsing(ConvertToStorageTypeDto);

            // Решта мапінгу залишається незмінною
            CreateMap<BookViewModel, BookDto>();
            CreateMap<DocumentViewModel, DocumentDto>();
            CreateMap<VideoViewModel, VideoDto>();
            CreateMap<AudioViewModel, AudioDto>();
            CreateMap<StorageViewModel, StorageDto>();
            CreateMap<ContentStorageViewModel, ContentStorageDto>();
        }

        // Методи конвертації
        private ContentTypeDto ConvertToContentTypeDto(string source, ContentTypeDto destination, ResolutionContext context)
        {
            ContentTypeDto result;
            return Enum.TryParse(source, out result) ? result : ContentTypeDto.Book;
        }

        private ContentFormatDto ConvertToContentFormatDto(string source, ContentFormatDto destination, ResolutionContext context)
        {
            ContentFormatDto result;
            return Enum.TryParse(source, out result) ? result : ContentFormatDto.PDF;
        }

        private StorageTypeDto ConvertToStorageTypeDto(string source, StorageTypeDto destination, ResolutionContext context)
        {
            StorageTypeDto result;
            return Enum.TryParse(source, out result) ? result : StorageTypeDto.LocalDisk;
        }
    }
}