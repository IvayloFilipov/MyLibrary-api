namespace Common
{
    public static class GlobalConstants
    {
        // Roles constants
        public const string ADMIN_ROLE_NAME = "Admin";
        public const string LIBRARIAN_ROLE_NAME = "Librarian";
        public const string READER_ROLE_NAME = "Reader";

        // ENTITIES
        // AuthorEntity
        public const int AUTHORNAME_MIN_LENGTH = 1;
        public const int AUTHORNAME_MAX_LENGTH = 256;


        // BookEntity
        public const int BOOK_TITLE_MIN_LENGTH = 1;
        public const int BOOK_TITLE_MAX_LENGTH = 256;
        public const int BOOK_DESCRIPTION_MAX_LENGTH = 1028;
        public const int BOOK_RANGE_MIN_VALUE = 1;
        public const int BOOK_RANGE_MAX_VALUE = 1000;
        public const int BOOK_STANDARD_BORROW_PERIOD = 30;
        public const int BOOK_ADD_BORROW_MIN_PERIOD = 1;
        public const int BOOK_ADD_BORROW_MAX_PERIOD = 30;

        // Regex for adding/updating book, genre, author
        public const string BOOK_ATTRIBUTE_REGEX = @"^(?=.*[^ !'&quot;#$%&()*+,-.\/:;&lt;=&gt;?@[\]^_`{|}~\r\n])(.)+$";

        // GenreEntity
        public const int GENRE_NAME_MAX_LENGTH = 65;

        // UserEntity
        public const int USER_FIRST_NAME_MIN_LENGTH = 1;
        public const int USER_FIRST_NAME_MAX_LENGTH = 65;
        public const int USER_LAST_NAME_MIN_LENGTH = 1;
        public const int USER_LAST_NAME_MAX_LENGTH = 65;
        public const int USER_PASSWORD_MIN_LENGTH = 10;
        public const int USER_PASSWORD_MAX_LENGTH = 65;
        public const string USER_PASSWORD_REGEX = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\#^\(\)_\-;:+])[A-Za-z\d@$!%*?&\#^\(\)_\-;:+]{10,65}$";
        public const string USER_PHONE_NUMBER_REGEX = @"^(\+\([\d]\-[\d]{3}\)[\d]{1,61})|(\+\([\d]{2}[-][\d]{4}\)[\d]{1,59})|(\+\([\d]{1}\)[\d]{1,64})|(\+\([\d]{2}\)[\d]{1,63})|(\+\([\d]{3}\)[\d]{1,62})$";
        public const string USER_EMAIL_REGEX = @"^(([^<>()[\]\\.,;:\s@""]+(\.[^<>()[\]\\.,;:\s@""]+)*)|("".+""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";

        // CommentEntity
        public const int COMMENT_DESCRIPTION_MAX_LENGTH = 500;

        // AddressEntity
        public const int ADDRESS_COUNTRY_NAME_MIN_LENGTH = 3;
        public const int ADDRESS_COUNTRY_NAME_MAX_LENGTH = 56;
        public const int ADDRESS_CITY_NAME_MIN_LENGTH = 1;
        public const int ADDRESS_CITY_NAME_MAX_LENGTH = 128;
        public const int ADDRESS_STREET_NAME_MIN_LENGTH = 1;
        public const int ADDRESS_STREET_NAME_MAX_LENGTH = 128;
        public const int ADDRESS_STREET_NUMBER_MIN_LENGTH = 1;
        public const int ADDRESS_STREET_NUMBER_MAX_LENGTH = 65;
        public const int ADDRESS_BUILDING_NUMBER_MAX_LENGTH = 65;
        public const int ADDRESS_APARTMENT_NUMBER_MAX_LENGTH = 65;
        public const int ADDRESS_ADDITIONAL_INFO_MAX_LENGTH = 1028;

        // ProlongingRequestEntity
        public const int PROLONGING_DESCRIPTION_MAX_LENGTH = 2000;
        public const int PROLONGING_RANGE_MIN_VALUE = 1;
        public const int PROLONGING_RANGE_MAX_VALUE = 30;

        // NotificationEntity
        public const int NOTIFICATION_MESSAGE_MAX_LENGTH = 1500;

        // BookReservationMessage
        public const int MESSAGE_MAX_LENGTH = 1028;

        // MODELS
        // LoginUserDto - Error messages
        public const string EMAIL_REQUIRED_ERROR_MESSAGE = "Please enter your email!";
        public const string WRONG_EMAIL_ERROR_MESSAGE = "An invalid email address has been entered into the email field.";

        public const string PASSWORD_REQUIRED_ERROR_MESSAGE = "Please enter your password!";
        public const string CONFIRM_PASSWORD_REQUIRED_ERROR_MESSAGE = "Please confirm your password!";
        public const string COMPARE_PASSWORDS_ERROR_MESSAGE = "Password and Confirm password do not match!";
        public const string WRONG_PASSWORD_ERROR_MESSAGE = "Password must contain minimum of 10 and maximum of 65 characters." +
            " Password must contain at least one upper-case letter, one lower-case letter, one number and one symbol";

        // Address - Error messages
        public const string COUNTRY_REQUIRED_ERROR_MESSAGE = "Please enter your country!";
        public const string COUNTRY_LENGTH_NAME_ERROR_MESSAGE = "Country must contain minimum of 3 and maximum of 56 characters.";

        public const string CITY_REQUIRED_ERROR_MESSAGE = "Please enter your city!";
        public const string CITY_LENGTH_NAME_ERROR_MESSAGE = "City must contain minimum of 1 and maximum of 128 characters.";

        public const string STREET_REQUIRED_ERROR_MESSAGE = "Please enter your street!";
        public const string STREET_LENGTH_NAME_ERROR_MESSAGE = "Street name must contain minimum of 1 and maximum of 128 characters.";

        public const string STREET_NUMBER_REQUIRED_ERROR_MESSAGE = "Please enter your street number!";
        public const string STREET_NUMBER_LENGTH_ERROR_MESSAGE = "Streen number must contain minimum of 1 and maximum of 65 characters.";

        public const string BUILDING_NUMBER_LENGTH_ERROR_MESSAGE = "Building number cannot contain more than 65 characters.";

        public const string APARTMENT_NUMBER_LENGTH_ERROR_MESSAGE = "Apartment number cannot contain more than 65 characters.";

        public const string ADDITIONAL_INFO_LENGTH_ERROR_MESSAGE = "Additional info cannot contain more than 1028 characters.";

        // RegisterUserDto - error messages
        public const string FIRST_NAME_REQUIRED_ERROR_MESSAGE = "Please enter your first name!";
        public const string FIRST_NAME_LENGTH_ERROR_MESSAGE = "First name must contain minimum of 1 and maximum of 65 characters.";

        public const string LAST_NAME_REQUIRED_ERROR_MESSAGE = "Please enter your last name!";
        public const string LAST_NAME_LENGTH_ERROR_MESSAGE = "Last name must contain minimum of 1 and maximum of 65 characters.";

        public const string PHONE_NUMBER_REQUIRED_ERROR_MESSAGE = "Please enter your phone number!";
        public const string INVALID_PHONE_NUMBER_ERROR_MESSAGE = "Invalid phone number!";

        // Genre - Error messages
        public const string GENRE_NAME_REQUIRED_ERROR_MESSAGE = "Please enter a genre name.";
        public const string GENRE_NAME_LENGTH_ERROR_MESSAGE = "Genre name cannot contain more than 65 characters.";
        public const string GENRE_REQUIRED_ERROR_MESSAGE = "Please insert your genres!";
        public const string GENRE_NAME_REGEX_ERROR_MESSAGE = "Genre name must contain at least one letter or number";


        // AddBooksDto - error messages
        public const string BOOKTITLE_REQUIRED_ERROR_MESSAGE = "Please enter a book title!";
        public const string BOOKTITLE_LENGTH_ERROR_MESSAGE = "Book title must contain minimum of 1 and maximum of 256 characters.";
        public const string BOOKTITLE_REGEX_ERROR_MESSAGE = "Book title must contain at least one letter or number";

        public const string AUTHORNAME_REQUIRED_ERROR_MESSAGE = "Please enter an author name!";
        public const string AUTHORNAME_LENGHT_ERROR_MESSAGE = "Author name must contain minimum of 1 and maximum of 256 characters.";
        public const string AUTHOR_REQUIRED_ERROR_MESSAGE = "Please insert authors!";

        public const string DESCRIPTION_LENGTH_ERROR_MESSAGE = "Description must contain max length of 1028 characters.";

        public const string BOOKCOVER_REQUIRED_ERROR_MESSAGE = "Please insert your book cover!";

        // AuthorDto - error messages
        public const string AUTHORNAME_REGEX_ERROR_MESSAGE = "Author name must contain at least one letter or number";

        // BookReservations - error messages
        public const string MESSAGE_REQUIRED_ERROR_MESSAGE = "The message is required.";
        public const string MESSAGE_LENGTH_ERROR_MESSAGE = "The message must contain max length of 1028 characters.";

        // PaginatorInputDto - error messages
        public const string PAGE_NUMBER_REQUIRED_ERROR_MESSAGE = "Page number is required";
        public const string PAGE_NUMBER_ATLEAST_ONE_ERROR_MESSAGE = "Page number can't be less than one";
        public const string PAGE_SIZE_REQUIRED_ERROR_MESSAGE = "Page size is required";
        public const string PAGE_SIZE_ATLEAST_ONE_ERROR_MESSAGE = "Page size can't be less than one";

        // Upload Blob Files - error messages
        public const string BLOBFILE_CAN_NOT_BE_EMPTY = "Blob file can not be uploaded.";
        public const string BLOB_STORAGE_CONTAINER = "book-covers";
    }
}