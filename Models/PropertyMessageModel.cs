namespace WebApplication1.Models
{
    public class PropertyMessageViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PropertyId { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Message { get; set; }
        public List<PropertyMessageViewModel> PagedMessages { get; set; } = new List<PropertyMessageViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalMessages { get; set; }
        public int FirstItemIndex => (CurrentPage - 1) * MessagesPerPage + 1;
        public int LastItemIndex => Math.Min(CurrentPage * MessagesPerPage, TotalMessages);
        public int MessagesPerPage { get; set; } = 5; // Change as needed
    }
}
