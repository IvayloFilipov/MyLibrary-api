using AutoMapper;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;

namespace Repositories.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Info about method -> CreateMap<TSource, TDestination>(); <- map from TSource to TDestination, if some properties has different names, add ForMember() method

            // Register User
            CreateMap<RegisterUserDto, UserEntity>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.AddressesEntity, opt => opt.MapFrom(src => src.Address));

            CreateMap<Address, AddressEntity>();

            // Login User
            CreateMap<UserEntity, LoginUserWithRolesDto>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash));

            // Authors
            CreateMap<AuthorDto, AuthorEntity>();

            CreateMap<AuthorEntity, AuthorOutput>();

            // Books
            CreateMap<AddBookDto, BookEntity>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.BookTitle))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.Availability))
                .ForMember(dest => dest.CurrentQuantity, opt => opt.MapFrom(src => src.TotalQuantity));

            CreateMap<AuthorEntity, AuthorsBooks>()
                .ForMember(dest => dest.AuthorEntityId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AuthorsEntity, opt => opt.MapFrom(src => src));

            CreateMap<GenreEntity, GenresBooks>()
                .ForMember(dest => dest.GenreEntityId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.GenresEntity, opt => opt.MapFrom(src => src));

            CreateMap<BookEntity, BookOutput>();

            CreateMap<BookReservationEntity, BookReservationResult>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserEntityId))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookEntityId));

            CreateMap<BookEntity, LastBooksOutput>();

            // Genres
            CreateMap<Genre, GenreEntity>();

            CreateMap<GenreEntity, GenreOutput>();
        }
    }
}
