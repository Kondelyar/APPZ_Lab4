using AutoMapper;
using ContentLibrary.BLL.Models;
using ContentLibrary.WebAPI.Models.Book;
using ContentLibrary.WebAPI.Models.Storage;
using ContentLibrary.WebApi.Models.Storage;

namespace ContentLibrary.WebAPI.Mapping
{
    public class ApiMappingProfile : Profile
    {
        public ApiMappingProfile()
        {
            // Book mappings
            CreateMap<BookCreateModel, BookDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => ContentTypeDto.Book))
                .ForMember(dest => dest.Format, opt => opt.MapFrom(src => Enum.Parse<ContentFormatDto>(src.Format)))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ContentStorages, opt => opt.Ignore());

            CreateMap<BookUpdateModel, BookDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => ContentTypeDto.Book))
                .ForMember(dest => dest.Format, opt => opt.MapFrom(src => Enum.Parse<ContentFormatDto>(src.Format)))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ContentStorages, opt => opt.Ignore());

            CreateMap<BookDto, BookViewModel>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Format, opt => opt.MapFrom(src => src.Format.ToString()));

            // Storage mappings
            CreateMap<StorageCreateModel, StorageDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<StorageTypeDto>(src.Type)))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ContentStorages, opt => opt.Ignore());

            CreateMap<StorageUpdateModel, StorageDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<StorageTypeDto>(src.Type)))
                .ForMember(dest => dest.ContentStorages, opt => opt.Ignore());

            CreateMap<StorageDto, StorageViewModel>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.AvailableSpace, opt => opt.MapFrom(src => src.Capacity - src.UsedSpace));

            // ContentStorage mappings
            CreateMap<ContentStorageDto, ContentStorageViewModel>();
        }
    }
}