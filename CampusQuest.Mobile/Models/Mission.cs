namespace CampusQuest.Mobile.Models
{
    public class Mission
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Points { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}