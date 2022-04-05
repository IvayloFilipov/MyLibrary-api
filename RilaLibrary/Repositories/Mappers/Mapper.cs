using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;

namespace Repositories.Mappers
{
    public static class Mapper
    {
        public static UserEntity ToUserEntity(RegisterUserDto user)
        {
            return new UserEntity
            {
                UserName = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                PasswordHash = user.Password,
                AddressesEntity = new AddressEntity
                {
                    Country = user.Address.Country,
                    City = user.Address.City,
                    Street = user.Address.Street,
                    StreetNumber = user.Address.StreetNumber,
                    BuildingNumber = user.Address.BuildingNumber,
                    ApartmentNumber = user.Address.ApartmentNumber,
                    AdditionalInfo = user.Address.AdditionalInfo
                }
            };
        }

        public static LoginUserWithRolesDto ToLoginUserWithRoles(this UserEntity userEntity, IList<string> roles)
        {
            return new LoginUserWithRolesDto
            {
                Id = userEntity.Id,
                Email = userEntity.Email,
                Password = userEntity.PasswordHash,
                Roles = roles,
            };
        }

        public static AuthorEntity ToAuthorEntity(string author)
        {
            return new AuthorEntity
            {
                AuthorName = author,
            };
        }

        public static AuthorOutput ToAuthorOutput(AuthorEntity entity)
        {
            return new AuthorOutput()
            {
                Id = entity.Id,
                AuthorName = entity.AuthorName
            };
        }

        public static BookEntity ToBookEntity(AddBookDto book, List<AuthorEntity> authorsEntities, List<GenreEntity> genresEntities, string? bookCoverUrl)
        {
            var bookEntity = new BookEntity
            {
                Title = book.BookTitle,
                Description = book.Description,
                ImageAddress = bookCoverUrl,
                IsAvailable = book.Availability,
                Sku = Guid.NewGuid(),
                TotalQuantity = book.TotalQuantity,
                CurrentQuantity = book.TotalQuantity,
                AuthorsBooks = new List<AuthorsBooks>(),
                GenresBooks = new List<GenresBooks>(),
            };

            foreach (var author in authorsEntities)
            {
                bookEntity.AuthorsBooks.Add(new AuthorsBooks { AuthorEntityId = author.Id, AuthorsEntity = author });
            }
            foreach (var genre in genresEntities)
            {
                bookEntity.GenresBooks.Add(new GenresBooks { GenreEntityId = genre.Id, GenresEntity = genre });
            }
            return bookEntity;
        }

        public static BookOutput ToBookOutput(BookEntity bookEntity)
        {
            return new BookOutput
            {
                Title = bookEntity.Title,
                Id = bookEntity.Id,
                Description = bookEntity.Description,
                ImageAddress = bookEntity.ImageAddress,
                IsAvailable = bookEntity.IsAvailable,
                CurrentQuantity = bookEntity.CurrentQuantity,
                TotalQuantity = bookEntity.TotalQuantity,
            };
        }
        public static BookReservationResult ToBookReservationResult(BookReservationEntity bookReservationEntity)
        {
            return new BookReservationResult
            {
                Id = bookReservationEntity.Id,
                UserId = bookReservationEntity.UserEntityId,
                BookId = bookReservationEntity.BookEntityId,
                IsApproved = bookReservationEntity.IsApproved,
                CreatedOn = bookReservationEntity.CreatedOn,
                IsReviewed = bookReservationEntity.IsReviewed
            };
        }

        public static GenreEntity ToGenreEntity(Genre genre)
        {
            return new GenreEntity
            {
                Name = genre.Name,
            };
        }

        public static GenreOutput ToGenreOutput(GenreEntity genreEntity)
        {
            return new GenreOutput
            {
                Name = genreEntity.Name,
                Id = genreEntity.Id,
            };
        }

        public static LastBooksOutput ToLastBooksOutput(BookEntity bookEntity)
        {
            return new LastBooksOutput
            {
                Id = bookEntity.Id,
                ImageAddress = bookEntity.ImageAddress,
                Title = bookEntity.Title,
                Description = bookEntity.Description,
            };
        }
    }
}
