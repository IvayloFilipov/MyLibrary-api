namespace Common
{
    public static class ExceptionMessages
    {
        public const string NULL_OBJECT = "The input object is null";
        public const string GENRE_EXISTS = "Genre already exists";
        public const string GENRE_NOT_FOUND = "Genre not found";
        public const string NO_GENRES_FOUND = "No genres found.";
        public const string GENRE_HAS_BOOKS = "Genre is assigned to book(s)";
        public const string BOOK_NOT_FOUND = "Book does not exist in the database.";
        public const string BOOKRESERVATION_NOT_FOUND = "The choosen book reservation request does not exist.";
        public const string BOOKRESERVATION_WAS_REVIEWED = "The choosen book reservation request has been reviewed.";
        public const string NO_BOOKS_FOUND = "Books are not found.";
        public const string BOOK_IS_NOT_AVAILABLE = "Book is not available.";
        public const string ALL_BOOK_RESERVATIONS_ARE_REVIEWED = "All book reservations have been reviewed.";
        public const string BOOKTITLE_FOUND = "The booktitle already exist in the database.";
        public const string NO_BOOK_RESERVATIONS_FOUND = "No book reservations found.";
        public const string NO_AUTHORS_FOUND = "No authors found.";
        public const string DUPLICATE_AUTHOR_FOUND = "Duplicate author entered.";
        public const string BOOK_EXISTS = "Book already exists";
        public const string BOOK_QUANTITY_IS_INVALID = "Book quantity can not be zero or less than zero";
        public const string BOOK_QUANTITY_IS_LESS_THAN_ZERO = "Book quantity can not be less than zero";
        public const string BOOK_QUANTITY_IS_LESS_THAN_BORROWEDBOOKS = "Inserted book quantity can not be less than the number of the borrowed books. {0} book/books has/have already been taken.";
        public const string BOOK_QUANTITY_IS_NULL = "Book quantity is zero or less than zero.";
        public const string AUTHOR_EXISTS = "Author already exists";
        public const string AUTHOR_NOT_FOUND = "Author not found";
        public const string AUTHOR_HAS_BOOKS = "Author is assigned to book(s)";
        public const string USER_NOT_FOUND = "User does not exist in the database.";
        public const string LIBRARIAN_NOT_FOUND = "Librarian does not exist.";
        public const string LIBRARIAN_SELFCONFIRMATION_APPROVALERROR = "Librarian cannot approve its own book reservation requests.";
        public const string LIBRARIAN_SELFCONFIRMATION_REJECTIONERROR = "Librarian cannot reject its own book reservation requests.";
        public const string BOOK_RESERVATION_NOT_EXISTS = "Book reservation not found.";
        public const string BOOK_HAS_APPROVED_RESERVATIONS = "Book is reserved by readers";

        public const string SMTP_EXCEPTION = "Email sending failed.";
        public const string RESET_PASSWORD_FAILED = "Could not reset the password.";

        public const string BLOBFILE_NOT_EXIST = "Blob file does not exist.";
        public const string BLOBSTORAGE_IS_EMPTY = "Blob storage is empty.";
        public const string FILE_NOT_CORRECT_FORMAT = "The chosen file has no correct extension.";
        public const string FILE_UPLOAD_FAILED = "Problem occures by uploading the file. The file can not be uploaded";
        public const string FILE_OVER_SIZE = "File must not be larger than 512 KB";
    }
}
