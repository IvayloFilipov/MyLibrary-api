using Common.Models.InputDTOs;
using DataAccess;
using DataAccess.Entities;
using NUnit.Framework;
using Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class BookReservationRepositoryTests
    {
        LibraryDbContext? inMemoryContext;
        BookReservationRepository? bookReservationRepository;

        [SetUp]
        public void SetUp()
        {
            inMemoryContext = InMemoryDbContext.GetInMemoryDbContext("InMemoryDb");
            inMemoryContext.Database.EnsureDeleted();
            inMemoryContext.Database.EnsureCreated();
            bookReservationRepository = new BookReservationRepository(inMemoryContext);

            List<BookReservationEntity> bookReservations = new List<BookReservationEntity>()
            {
                new BookReservationEntity()
                {
                    Id = Guid.Parse("8616a313-af7a-4c40-b866-6e951bfc00c3"),
                    BookEntityId = Guid.Parse("f542a4ae-57d1-432d-889f-ed50f672b4a3"),
                    UserEntityId = Guid.Parse("e8f545bb-355d-423a-b115-40c11c7905d2"),
                    IsApproved = false,
                    IsReviewed = false,
                },
                new BookReservationEntity()
                {
                    Id = Guid.Parse("a906ee9f-a389-4aa8-9835-779bdaef32a7"),
                    BookEntityId = Guid.Parse("83c5f48d-eca1-4250-b5ae-a85bd3159730"),
                    UserEntityId = Guid.Parse("f59b7f3c-791a-40a5-915e-0043e5c1dae0"),
                    IsApproved = true,
                    IsReviewed = true,
                },
                new BookReservationEntity()
                {
                    Id = Guid.Parse("8616a313-af7a-4c90-b866-6e951bfc00c3"),
                    BookEntityId = Guid.Parse("f59b7f3c-791a-40a5-915e-0043e5c1dae0"),
                    UserEntityId = Guid.Parse("e8f545bb-355d-423a-b115-40c11c7905d2"),
                    IsApproved = false,
                    IsReviewed = false,
                },
                new BookReservationEntity()
                {
                    Id = Guid.Parse("503a560b-df3c-4ef6-855f-4daf6d3003ad"),
                    BookEntityId = Guid.Parse("85c572c1-6f39-4ca5-9b03-ff1174de6cae"),
                    UserEntityId = Guid.Parse("c59a208a-af02-4df2-8d76-4c47eed5b0e5"),
                    IsApproved = false,
                    IsReviewed = false,
                }
            };

            inMemoryContext.BookReservations.AddRange(bookReservations);
            inMemoryContext.SaveChanges();
        }

        [Test]
        public async Task GetBookReservationPage_Return_CorrectAnswer()
        {
            int expectedCount = 3;

            var result = await bookReservationRepository!.GetBookReservationPageAsync(new PaginatorInputDto() { Page = 1, PageSize = 5 });
            
            Assert.AreEqual(expectedCount, result.Item2);
        }

        [Test]
        public async Task GetById_Resturns_TheCorrect_BookReservation()
        {
            var expected = new BookReservationEntity()
            {
                Id = Guid.Parse("8616a313-af7a-4c90-b866-6e951bfc00c3"),
                BookEntityId = Guid.Parse("f59b7f3c-791a-40a5-915e-0043e5c1dae0"),
                UserEntityId = Guid.Parse("e8f545bb-355d-423a-b115-40c11c7905d2"),
                IsApproved = false,
                IsReviewed = false,
            };

            var result = await bookReservationRepository!.GetByIdAsync(Guid.Parse("8616a313-af7a-4c90-b866-6e951bfc00c3"));

            Assert.IsTrue(expected.Id == result!.Id);
        }

        [Test]
        public async Task Insert_AddTheNewRequest()
        {
            var newReserVationRequest = new BookReservationEntity()
            {
                Id = Guid.Parse("8616a313-af7a-4c90-b356-6e951bfc00c3"),
                BookEntityId = Guid.Parse("f59b7f3c-221a-40a5-915e-0043e5c1dae0"),
                UserEntityId = Guid.Parse("e8f775bb-355d-423a-b115-40c11c7905d2"),
                IsApproved = false,
                IsReviewed = false,
            };

            var result = await bookReservationRepository!.InsertAsync(newReserVationRequest);

            Assert.AreEqual(newReserVationRequest, result);
        }
    }
}
