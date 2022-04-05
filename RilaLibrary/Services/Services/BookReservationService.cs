using Microsoft.AspNetCore.Identity;
using log4net;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;
using Repositories.Interfaces;
using Repositories.Mappers;
using Services.EmailSender;
using Services.Interfaces;
using AutoMapper;

using static Common.ExceptionMessages;

namespace Services.Services
{
    public class BookReservationService : IBookReservationService
    {
        private readonly IBookReservationRepository bookReservationRepository;
        private readonly IBookService bookService;
        private readonly IUserService userService;
        private readonly IMailSender mailSender;
        private readonly IMapper mapper;
        private readonly UserManager<UserEntity> userManager;
        private readonly ILog log = LogManager.GetLogger(typeof(BookReservationService));

        public BookReservationService(
            IBookReservationRepository bookReservationRepository,
            IBookService bookService,
            IUserService userService,
            UserManager<UserEntity> userManager,
            IMailSender mailSender,
            IMapper mapper
            )
        {
            this.bookReservationRepository = bookReservationRepository;
            this.bookService = bookService;
            this.userService = userService;
            this.userManager = userManager;
            this.mailSender = mailSender;
            this.mapper = mapper;
        }

        public async Task<BookReservationEntity> AddBookReservationAsync(BookReservationDto input)
        {
            await CheckUserOnAddAsync(input);
            await CheckExistingBookOnAddAsync(input);

            var bookReservationEntity = new BookReservationEntity
            {
                UserEntityId = input.UserId,
                BookEntityId = input.BookId,
            };

            await bookReservationRepository.InsertAsync(bookReservationEntity);
            await bookReservationRepository.SaveAsync();

            return bookReservationEntity;
        }

        public async Task<(List<BookReservationOutput>, int)> GetBooksReservationsAsync(PaginatorInputDto input)
        {
            var result = new List<BookReservationOutput>();
            var (entities, totalCount) = await bookReservationRepository.GetBookReservationPageAsync(input);

            if (totalCount == 0)
            {
                log.Error("There are no new bookReservations. Exception is thrown {ALL_BOOK_RESERVATIONS_ARE_REVIEWED}");
                throw new NullReferenceException(ALL_BOOK_RESERVATIONS_ARE_REVIEWED);
            }

            foreach (var entity in entities)
            {
                if (!entity.IsReviewed)
                {
                    var generatedEntity = await GenerateBookReservationOutputAsync(entity);
                    result.Add(generatedEntity);
                }
            }
            return (result, totalCount);
        }

        public async Task<BookConfirmReservationOutput> GetBookReservationByIdAsync(Guid bookReservationId)
        {
            var existingRequest = await bookReservationRepository.GetByIdAsync(bookReservationId);

            CheckExistingReservationRequest(existingRequest!);

            var result = await GenerateConfirmBookReservationOutputAsync(existingRequest!);

            return result;
        }

        public async Task RejectBookReservationByIdAsync(BookReservationMessageDto input)
        {
            var existingBookReservation = await bookReservationRepository.GetByIdAsync(input.bookReservationId);
            CheckExistingReservationRequest(existingBookReservation!);

            var book = await bookService.GetByBookIdRepoAsync(existingBookReservation!.BookEntityId);
            CheckBook(book);

            await CheckExistingLibrarianAsync(input);

            var user = await userService.GetByUserIdRepoAsync(existingBookReservation.UserEntityId.ToString());
            CheckUserOnRejection(user, input);

            var email = await GetEmailFromRequestAsync(existingBookReservation);

            existingBookReservation.LibrarianId = input.librarianId;
            existingBookReservation.IsReviewed = true;
            await bookReservationRepository.UpdateAsync(existingBookReservation);
            await bookReservationRepository.SaveAsync();

            await mailSender.SendEmailAsync(email, "Rejected book reservation request","", $"<p>{ input.Message }</p>");
        }

        public async Task ApproveBookReservation(BookReservationMessageDto input)
        {
            var existingBookReservation = await bookReservationRepository.GetByIdAsync(input.bookReservationId);
            CheckExistingReservationRequest(existingBookReservation);

            var book = await bookService.GetByBookIdRepoAsync(existingBookReservation!.BookEntityId);
            bool isApproval = true;
            CheckBook(book, isApproval);

            await CheckLibrarianOnApprovalAsync(input);

            var user = await userService.GetByUserIdRepoAsync(existingBookReservation.UserEntityId.ToString());
            CheckUserOnApproval(user, input);

            ModifyBookReservationOnApproval(existingBookReservation, input);

            book = ModifyBookQuantityOnApproval(book!);

            await bookService.UpdateRepoAsync(book);

            await bookReservationRepository.SaveAsync();

            await mailSender.SendEmailAsync(user!.Email, "Approved book reservation request", input.Message!, $"<p> {input.Message} <p>");
        }

        public async Task CheckUserOnAddAsync(BookReservationDto input)
        {
            var userIdParsedToString = input.UserId.ToString();

            var existingUserEntity = await userService.GetByUserIdRepoAsync(userIdParsedToString);

            if (existingUserEntity == null)
            {
                log.Error($"User is not found. Exception is thrown {USER_NOT_FOUND}");
                throw new ArgumentException(USER_NOT_FOUND);
            }
        }

        public async Task CheckExistingBookOnAddAsync(BookReservationDto input)
        {
            var existingBookEntity = await bookService.GetByBookIdRepoAsync(input.BookId);

            if (existingBookEntity == null)
            {
                log.Error($"Book is not found. Exception is thrown {BOOK_NOT_FOUND}");
                throw new ArgumentException(BOOK_NOT_FOUND);
            }

            if (!existingBookEntity.IsAvailable)
            {
                log.Error($"Exception is thrown {BOOK_IS_NOT_AVAILABLE}");
                throw new ArgumentException(BOOK_IS_NOT_AVAILABLE);
            }

            var currentQuantity = existingBookEntity.CurrentQuantity;

            if (currentQuantity <= 0)
            {
                log.Error($"Exception is thrown {BOOK_QUANTITY_IS_NULL}");
                throw new ArgumentException(BOOK_QUANTITY_IS_NULL);
            }
        }

        public async Task<BookReservationOutput> GenerateBookReservationOutputAsync(BookReservationEntity bookReservationEntity)
        {
            TimeZoneInfo sofiaTime = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

            //var mappedEntity = Mapper.ToBookReservationResult(bookReservationEntity); //was
            var mappedEntity = mapper.Map<BookReservationResult>(bookReservationEntity);
            var currentBook = await bookService.GetByBookIdRepoAsync(mappedEntity.BookId);
            var currentUser = await userService.GetByUserIdRepoAsync((mappedEntity.UserId).ToString());

            var createdOnLocal = TimeZoneInfo.ConvertTimeFromUtc(bookReservationEntity.CreatedOn, sofiaTime);

            var result = new BookReservationOutput
            {
                Id = mappedEntity.Id,
                BookTitle = currentBook!.Title,
                UserName = string.Concat(currentUser!.FirstName, " ", currentUser.LastName),
                Email = currentUser.Email,
                IsApproved = bookReservationEntity.IsApproved,
                CreatedOn = createdOnLocal.ToString("dd/MM/yyyy HH:mm:ss")
            };

            return result;
        }

        public void CheckExistingReservationRequest(BookReservationEntity? existingRequest)
        {
            if (existingRequest == null)
            {
                log.Error($"Book reservation is null. Exception is thrown {BOOKRESERVATION_NOT_FOUND}");
                throw new NullReferenceException(BOOKRESERVATION_NOT_FOUND);
            }

            if (existingRequest.IsReviewed)
            {
                log.Error($"Exception is thrown {BOOKRESERVATION_WAS_REVIEWED}");
                throw new NullReferenceException(BOOKRESERVATION_WAS_REVIEWED);
            }
        }

        public async Task<BookConfirmReservationOutput> GenerateConfirmBookReservationOutputAsync(BookReservationEntity bookReservationEntity)
        {
            TimeZoneInfo sofiaTime = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

            //var mappedEntity = Mapper.ToBookReservationResult(bookReservationEntity); //was
            var mappedEntity = mapper.Map<BookReservationResult>(bookReservationEntity);
            var currentBook = await bookService.GetByBookIdRepoAsync(mappedEntity.BookId);
            var currentUser = await userService.GetByUserIdRepoAsync((mappedEntity.UserId).ToString());

            var createdOnLocal = TimeZoneInfo.ConvertTimeFromUtc(bookReservationEntity.CreatedOn, sofiaTime);

            var result = new BookConfirmReservationOutput
            {
                BookTitle = currentBook!.Title,
                UserName = string.Concat(currentUser!.FirstName, " ", currentUser.LastName),
                Quantity = currentBook.CurrentQuantity,
                IsAvailable = currentBook.IsAvailable,
                CreatedRequestDate = createdOnLocal.ToString("dd/MM/yyyy HH:mm:ss"),
                Message = string.Empty
            };

            return result;
        }

        public async Task CheckExistingLibrarianAsync(BookReservationMessageDto input)
        {
            var existingLibrarian = await userManager.FindByIdAsync(input.librarianId.ToString());

            if (existingLibrarian == null)
            {
                log.Error($"Librarian is not found. Exception is thrown {LIBRARIAN_NOT_FOUND}");
                throw new NullReferenceException(LIBRARIAN_NOT_FOUND);
            }
        }

        public async Task<string> GetEmailFromRequestAsync(BookReservationEntity bookReservationEntity)
        {
            var userToString = bookReservationEntity.UserEntityId.ToString();
            var user = await userService.GetByUserIdRepoAsync(userToString);
            var result = user!.Email;

            return result;
        }

        public void CheckIfBookReservationExists(BookReservationEntity? bookReservationEntity)
        {
            if (bookReservationEntity == null)
            {
                log.Error($"Book reservation is null. Exception is thrown {LIBRARIAN_NOT_FOUND}");
                throw new NullReferenceException(BOOK_RESERVATION_NOT_EXISTS);
            }
        }

        public void CheckBook(BookEntity? book, bool isApproval = false)
        {
            if (book == null)
            {
                log.Error($"Book is null. Exception is thrown {BOOK_NOT_FOUND}");
                throw new NullReferenceException(BOOK_NOT_FOUND);
            }

            if ((book.CurrentQuantity == 0 || !book.IsAvailable) & isApproval)
            {
                log.Error($"Book quantity is 0 or it is not available. Exception is thrown {BOOK_IS_NOT_AVAILABLE}");
                throw new NullReferenceException(BOOK_IS_NOT_AVAILABLE);
            }
        }

        public void CheckUserOnApproval(UserEntity? user, BookReservationMessageDto input)
        {
            if (user == null)
            {
                log.Error($"User is null. Exception is thrown {USER_NOT_FOUND}");
                throw new ArgumentNullException(USER_NOT_FOUND);
            }

            if (user!.Id == input.librarianId.ToString())
            {
                throw new InvalidOperationException(LIBRARIAN_SELFCONFIRMATION_APPROVALERROR);
            }
        }

        public void CheckUserOnRejection(UserEntity? user, BookReservationMessageDto input)
        {
            if (user == null)
            {
                throw new NullReferenceException(USER_NOT_FOUND);
            }

            if (user!.Id == input.librarianId.ToString())
            {
                log.Error($"Exception is thrown {LIBRARIAN_SELFCONFIRMATION_REJECTIONERROR}");
                throw new InvalidOperationException(LIBRARIAN_SELFCONFIRMATION_REJECTIONERROR);
            }
        }

        public async Task CheckLibrarianOnApprovalAsync(BookReservationMessageDto input)
        {
            var librarian = await userManager.FindByIdAsync(input.librarianId.ToString());

            if (librarian == null)
            {
                log.Error($"Librarian is null. Exception is thrown {LIBRARIAN_NOT_FOUND}");
                throw new NullReferenceException(LIBRARIAN_NOT_FOUND);
            }
        }

        public void ModifyBookReservationOnApproval(BookReservationEntity bookReservationEntity, BookReservationMessageDto input)
        {
            bookReservationEntity.IsApproved = true;
            bookReservationEntity.IsReviewed = true;
            bookReservationEntity.LibrarianId = input.librarianId;
        }

        public BookEntity ModifyBookQuantityOnApproval(BookEntity book)
        {
            book!.CurrentQuantity -= 1;

            if (book.CurrentQuantity == 0)
            {
                book.IsAvailable = false;
            }

            return book;
        }
    }
}
