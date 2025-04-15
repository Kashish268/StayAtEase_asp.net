namespace WebApplication1.Models
{
    public class PropertyMessageViewModel
    {
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
        public int FirstItemIndex => (CurrentPage - 1) * MessagesPerPage + 1;

        // Last item index for pagination
        public int LastItemIndex => Math.Min(CurrentPage * MessagesPerPage, TotalMessages);

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
    }
}
