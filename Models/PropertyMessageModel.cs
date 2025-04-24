using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class PropertyMessageViewModel
    {
        // Unique ID for each message (from DB)
        public int Id { get; set; }
        public int InquiryId { get; set; }

        // Guest name (user's name who sent the message)
        public string Name { get; set; }

        // Property ID (from the property that received the inquiry)
        public string PropertyId { get; set; }

        // Guest's email
        public string Email { get; set; }

        // Guest's contact number
        public string Contact { get; set; }

        // Message content
        public string Message { get; set; }

        // Title of the property
        public string PropertyTitle { get; set; }

        // URL of the first image (if any)
        public string ImageUrl { get; set; }

        // Pagination Details
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalMessages { get; set; }
        public int MessagesPerPage { get; set; } = 5;

        // Search functionality
        public string SearchTerm { get; set; }

        // Filter used to categorize the messages (e.g., "all", "read", "unread")
        public string Filter { get; set; }

        // Indexes to display in pagination
        public int FirstItemIndex => (CurrentPage - 1) * MessagesPerPage + 1;
        public int LastItemIndex => Math.Min(CurrentPage * MessagesPerPage, TotalMessages);

        // List of messages for the current page
        public List<PropertyMessageViewModel> PagedMessages { get; set; }

        // Pagination logic: calculates the next page and previous page based on current page
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        // Constructor to ensure the default values are initialized
        public PropertyMessageViewModel()
        {
            PagedMessages = new List<PropertyMessageViewModel>();
        }

        // Optional: helper method to calculate pagination values
        public void SetPagination(int totalMessages, int currentPage, int messagesPerPage = 5)
        {
            TotalMessages = totalMessages;
            CurrentPage = currentPage;
            MessagesPerPage = messagesPerPage;
            TotalPages = (int)Math.Ceiling((double)TotalMessages / MessagesPerPage);
        }
    }
}
