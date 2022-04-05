using Common;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using Repositories.Interfaces;
using Services.EmailSender;
using Services.Interfaces;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Common.ExceptionMessages;

namespace Tests.ServicesTests
{
    [TestFixture]
    public class BookReservationServiceTests
    {
        private Mock<IBookReservationRepository>? mockBookReservationRepository;
        private readonly Mock<IBookService> mockBookService = new Mock<IBookService>();
        private readonly Mock<IUserService> mockUserService = new Mock<IUserService>();
        private Mock<UserManager<UserEntity>>? mockUserManager;
        private Mock<IMailSender>? mockEmailSender;
        private BookReservationService? bookReservationService;

        private BookReservationEntity? bookResEntity;

        [SetUp]
        public void SetUp()
        {
            var mockUserEntity = new UserEntity
            {
                Email = "ivanov@gmail.com",
            };

            mockBookReservationRepository = new Mock<IBookReservationRepository>();
            mockEmailSender = new Mock<IMailSender>();
            mockUserManager = MockUserManager(mockUserEntity);

            bookReservationService = new BookReservationService(
                                              mockBookReservationRepository.Object,
                                              mockBookService.Object,
                                              mockUserService.Object,
                                              mockUserManager.Object,
                                              mockEmailSender.Object,
                                              null!);

            var userEntity = new UserEntity
            {
                Id = "e8f545bb-355d-423a-b115-40c11c7905d2",
                FirstName = "Zornitsa"
            };

            var bookEntity = new BookEntity
            {
                Id = Guid.NewGuid(),
                CurrentQuantity = 5,
                IsAvailable = true,
            };

            bookResEntity = new BookReservationEntity
            {
                BookEntityId = Guid.NewGuid(),
            };

            var bookReservations = new List<BookReservationResult>();
            bookReservations.Add(new BookReservationResult {  BookTitle = "Alice" });

            mockBookService.Setup(x => x.GetByBookIdRepoAsync(It.IsAny<Guid>())).ReturnsAsync(bookEntity);
            mockUserService.Setup(x => x.GetByUserIdRepoAsync(It.IsAny<string>())).ReturnsAsync(userEntity);
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity());
            mockBookReservationRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new BookReservationEntity());
        }

        [Test]
        public async Task Should_ReturnOk_When_AddingValidBookReservation()
        {
            var result = await bookReservationService!.AddBookReservationAsync(new BookReservationDto());

            Assert.IsNotNull(result);
        }

        [Test]
        public void Should_ThrowArgumentException_When_BookIdIsNotFound()
        {
            mockBookService!.Setup(x => x.GetByBookIdRepoAsync(It.IsAny<Guid>())).ReturnsAsync(default(BookEntity));

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await bookReservationService!.AddBookReservationAsync(new BookReservationDto()));
            Assert.AreEqual(BOOK_NOT_FOUND, result!.Message);
        }

        [Test]
        public void Should_ThrowArgumentException_When_UserIdIsNotFound()
        {
            mockUserService!.Setup(x => x.GetByUserIdRepoAsync(It.IsAny<string>())).ReturnsAsync(default(UserEntity));

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await bookReservationService!.AddBookReservationAsync(new BookReservationDto()));
            Assert.AreEqual(USER_NOT_FOUND, result!.Message);
        }

        [Test]
        public void Should_ThrowArgumentException_When_BookCurrentQuantity_Is_Zero_Or_Less_Than_Zero()
        {
            mockBookService!.Setup(x => x.GetByBookIdRepoAsync(It.IsAny<Guid>())).ReturnsAsync(new BookEntity { Id = Guid.NewGuid(),IsAvailable=true,CurrentQuantity = -1});

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await bookReservationService!.AddBookReservationAsync(new BookReservationDto()));
            Assert.AreEqual(BOOK_QUANTITY_IS_NULL, result!.Message);
        }

        [Test]
        public async Task Should_ReturnOk_When_GettingUnConfirmedBookReservations()
        {
            var bookReservation = new List<BookReservationEntity>() { new BookReservationEntity {Id = Guid.NewGuid(), IsReviewed = false }};
            mockBookReservationRepository!.Setup(x => x.GetBookReservationPageAsync(It.IsAny<PaginatorInputDto>())).ReturnsAsync((bookReservation, 25));

            var result = await bookReservationService!.GetBooksReservationsAsync(new PaginatorInputDto());

            Assert.IsNotNull(result);
        }

        [Test]
        public void Should_ThrowNullException_When_GettingNoUnConfirmedBookReservations()
        {
            mockBookReservationRepository!.Setup(x => x.GetEntityPageAsync(It.IsAny<PaginatorInputDto>()))!.ReturnsAsync((default(List<BookReservationEntity>), 0));

            var result = Assert.ThrowsAsync<NullReferenceException>(async () => await bookReservationService!.GetBooksReservationsAsync(new PaginatorInputDto()));
            Assert.AreEqual(ALL_BOOK_RESERVATIONS_ARE_REVIEWED, result!.Message);
        }

        [Test]
        public async Task Should_ReturnOk_When_GettingBookReservationRequest()
        {
            var result = await bookReservationService!.GetBookReservationByIdAsync(Guid.NewGuid());

            Assert.IsNotNull(result);
        }

        [Test]
        public void Should_ThrowNullReferenceException_When_GettingNoBookReservationRequest()
        {
            mockBookReservationRepository!.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(default(BookReservationEntity));

            var result = Assert.ThrowsAsync<NullReferenceException>(async () => await bookReservationService!.GetBookReservationByIdAsync(Guid.Empty));
            Assert.AreEqual(BOOKRESERVATION_NOT_FOUND, result!.Message);
        }

        [Test]
        public void Should_ThrowNullReferenceException_When_GettingBookRequestWasReviewed()
        {
            var bookReseravtionEntity = new BookReservationEntity
            {
                Id = Guid.NewGuid(),
                IsReviewed = true
            };
            mockBookReservationRepository!.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(bookReseravtionEntity);

            var result = Assert.ThrowsAsync<NullReferenceException>(async () => await bookReservationService!.GetBookReservationByIdAsync(bookReseravtionEntity.Id));
            Assert.AreEqual(BOOKRESERVATION_WAS_REVIEWED, result!.Message);
        }

        [Test]
        public async Task Should_ReturnOk_When_RejectedMessageWasSuccessfullySent()
        {
            await bookReservationService!.RejectBookReservationByIdAsync(new BookReservationMessageDto() {librarianId=Guid.NewGuid()});

            mockEmailSender!.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Should_ThrowInvalidOperationException_When_InRejectProcess_LibrarianAndBookRequestUserAreTheSamePerson()
        {
            var bookReservationEntity = new BookReservationEntity
            {
                LibrarianId = Guid.Parse("C1648CAE-B74A-4904-C0CC-08DA0CACA92A"),
                UserEntityId = Guid.Parse("C1648CAE-B74A-4904-C0CC-08DA0CACA92A"),
            };

            mockBookReservationRepository!.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(bookReservationEntity);
            mockUserService.Setup(x => x.GetByUserIdRepoAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity { Id = bookReservationEntity.UserEntityId.ToString() });

            var result = Assert.ThrowsAsync<InvalidOperationException>(async () => await bookReservationService!
                               .RejectBookReservationByIdAsync
                               (new BookReservationMessageDto() {bookReservationId= bookReservationEntity.UserEntityId,librarianId= bookReservationEntity.LibrarianId}));

            Assert.AreEqual(LIBRARIAN_SELFCONFIRMATION_REJECTIONERROR, result!.Message);
        }

        [Test]
        public void Should_ThrowInvalidOperationException_When_LibrarianIsNotFound()
        {
            var bookReservationEntity = new BookReservationEntity
            {
                IsReviewed = false,
            };

            mockBookReservationRepository!.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(bookReservationEntity);
            mockUserManager!.Setup(x => x.FindByIdAsync(It.IsAny<string>()))!.ReturnsAsync(default(UserEntity));

            var result = Assert.ThrowsAsync<NullReferenceException>(async () => await bookReservationService!
                               .RejectBookReservationByIdAsync
                               (new BookReservationMessageDto()));

            Assert.AreEqual(LIBRARIAN_NOT_FOUND, result!.Message);
        }

        public static Mock<UserManager<T>> MockUserManager<T>(T input)
           where T : class
        {
            var store = new Mock<IUserStore<T>>();
            var mgr = new Mock<UserManager<T>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<T>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<T>());

            return mgr;
        }

        [Test]
        public async Task Should_ApproveReservation_With_Correct_Input_Data()
        {
            mockBookReservationRepository!.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(bookResEntity);

            await bookReservationService!.ApproveBookReservation(new BookReservationMessageDto());

            mockEmailSender!.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Should_ThrowException_When_Approve_GetNotExistingBookReservationId()
        {
            mockBookReservationRepository!.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(default(BookReservationEntity));

            Assert.ThrowsAsync<NullReferenceException>(async Task () => await bookReservationService!
                                    .ApproveBookReservation(new BookReservationMessageDto()), message: "Book reservation not found.");
        }

        [Test]
        public void Should_ThrowException_When_Approve_CannotFindTheBook()
        {
            mockBookService.Setup(x => x.GetByBookIdRepoAsync(It.IsAny<Guid>())).ReturnsAsync(default(BookEntity));

            Assert.ThrowsAsync<NullReferenceException>(async Task () => await bookReservationService!
                                    .ApproveBookReservation(new BookReservationMessageDto()), message: "Book does not exist in the database.");
        }

        [Test]
        public void Should_ThrowException_When_BookIsNotAvailable_ByApprove()
        {
            var bookEntity = new BookEntity
            {
                Id = Guid.NewGuid(),
                CurrentQuantity = 5,
                IsAvailable = false,
            };

            mockBookService.Setup(x => x.GetByBookIdRepoAsync(It.IsAny<Guid>())).ReturnsAsync(bookEntity);

            Assert.ThrowsAsync<NullReferenceException>(async Task () => await bookReservationService!
                                    .ApproveBookReservation(new BookReservationMessageDto()), message: "Book does not exist in the database.");
        }

        [Test]
        public void Should_ThrowException_When_Approve_CannotFindTheUser()
        {
            mockUserService.Setup(x => x.GetByUserIdRepoAsync(It.IsAny<string>())).ReturnsAsync(default(UserEntity));

            Assert.ThrowsAsync<ArgumentNullException>(async Task () => await bookReservationService!
                                    .ApproveBookReservation(new BookReservationMessageDto()), message: "User does not exist in the database.");
        }

        [Test]
        public void Should_ThrowException_When_LibrarianApproveOwnRequest()
        {
            mockBookReservationRepository!.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).
                ReturnsAsync(new BookReservationEntity());

            Assert.ThrowsAsync<InvalidOperationException>(async Task () => await bookReservationService!
                    .ApproveBookReservation(new BookReservationMessageDto() {librarianId = Guid.Parse("e8f545bb-355d-423a-b115-40c11c7905d2") }), 
                message: "Librarian cannot approve its own book reservation requests.");
        }
    }
}
