namespace WebApplication1.Models
{
    public class PropertyMessageViewModel
    {
<<<<<<< HEAD
        public int Id { get; set; }              // Unique ID for each message (from DB)
        public string Name { get; set; }          // Guest name (user's name who sent the message)
        public string PropertyId { get; set; }    // Property ID (from the property that received the inquiry)
        public string Email { get; set; }         // Guest's email
        public string Contact { get; set; }       // Guest's contact number
        public string Message { get; set; }       // Message content
        public string PropertyTitle { get; set; } // Title of the property
        public string ImageUrl { get; set; }      // URL of the first image (if any)

        // Pagination Details
        public int CurrentPage { get; set; }      // Current page number
        public int TotalPages { get; set; }       // Total number of pages
        public int TotalMessages { get; set; }    // Total number of messages (from DB)
        public int MessagesPerPage { get; set; }  // Number of messages per page

        // Indexes to display in pagination
=======
        // Unique identifier for the message
        public int Id { get; set; }

        // Name of the person who sent the message
        public string Name { get; set; }

        // Associated property ID
        public string PropertyId { get; set; }

        // Email of the person who sent the message
        public string Email { get; set; }

        // Contact number of the person who sent the message
        public string Contact { get; set; }

        // The actual message content
        public string Message { get; set; }

        // Property title (optional)
        public string PropertyTitle { get; set; }

        // Image URL (optional)
        public string ImageUrl { get; set; }

        // List of paged messages (for pagination)
        public List<PropertyMessageViewModel> PagedMessages { get; set; } = new List<PropertyMessageViewModel>();

        // Current page number for pagination
        public int CurrentPage { get; set; }

        // Total number of pages based on the total messages and messages per page
        public int TotalPages { get; set; }

        // Total number of messages
        public int TotalMessages { get; set; }

        // Search term used for filtering messages
        public string SearchTerm { get; set; }

        // Filter used to categorize the messages (e.g., "all", "read", "unread")
        public string Filter { get; set; }

        // Calculate the indexes for the current page
>>>>>>> ef7b32b2840d9e0db7a201e1360e94cef2caa37c
        public int FirstItemIndex => (CurrentPage - 1) * MessagesPerPage + 1;

        // Last item index for pagination
        public int LastItemIndex => Math.Min(CurrentPage * MessagesPerPage, TotalMessages);

<<<<<<< HEAD
        // For search functionality (optional, can be used if you need to pass search term)
        public string SearchTerm { get; set; }

        // Constructor to ensure the default values are initialized
        public PropertyMessageViewModel()
        {
            PagedMessages = new List<PropertyMessageViewModel>();
        }

        // List of messages for the current page
        public List<PropertyMessageViewModel> PagedMessages { get; set; }

        // Pagination logic: calculates the next page and previous page based on current page
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        // Add any other additional properties if required for filtering or UI
=======
        // Number of messages per page (this can be configured)
        public int MessagesPerPage { get; set; } = 5;

        // Constructor for pagination settings
        public PropertyMessageViewModel()
        {
        }

        // Method to set pagination
        public void SetPagination(int totalMessages, int currentPage, int messagesPerPage = 5)
        {
            TotalMessages = totalMessages;
            CurrentPage = currentPage;
            MessagesPerPage = messagesPerPage;
            TotalPages = (int)Math.Ceiling((double)TotalMessages / MessagesPerPage);
        }
>>>>>>> ef7b32b2840d9e0db7a201e1360e94cef2caa37c
    }

}
