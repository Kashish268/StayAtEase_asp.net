namespace WebApplication1.Models
{
    public class PropertyMessageViewModel
    {
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
        public int FirstItemIndex => (CurrentPage - 1) * MessagesPerPage + 1;
        public int LastItemIndex => Math.Min(CurrentPage * MessagesPerPage, TotalMessages);

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
    }

}
