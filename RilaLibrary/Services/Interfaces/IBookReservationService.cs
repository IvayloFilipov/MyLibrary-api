using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;

namespace Services.Interfaces
{
    public interface IBookReservationService
    {
        Task<BookReservationEntity> AddBookReservationAsync(BookReservationDto input);

        Task<(List<BookReservationOutput>, int)> GetBooksReservationsAsync(PaginatorInputDto input);

        Task<BookConfirmReservationOutput> GetBookReservationByIdAsync(Guid bookReservationId);

        Task RejectBookReservationByIdAsync(BookReservationMessageDto input);

        Task ApproveBookReservation(BookReservationMessageDto input);

        Task CheckUserOnAddAsync(BookReservationDto input);

        Task CheckExistingBookOnAddAsync(BookReservationDto input);

        Task<BookReservationOutput> GenerateBookReservationOutputAsync(BookReservationEntity bookReservationEntity);

        void CheckExistingReservationRequest(BookReservationEntity? existingRequest);

        Task<BookConfirmReservationOutput> GenerateConfirmBookReservationOutputAsync(BookReservationEntity bookReservationEntity);

        Task CheckExistingLibrarianAsync(BookReservationMessageDto input);

        Task<string> GetEmailFromRequestAsync(BookReservationEntity bookReservationEntity);

        void CheckBook(BookEntity? book, bool isApproval);

        void ModifyBookReservationOnApproval(BookReservationEntity bookReservationEntity, BookReservationMessageDto input);

        BookEntity ModifyBookQuantityOnApproval(BookEntity book);
    }
}
