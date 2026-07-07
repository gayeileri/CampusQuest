using CampusQuest.Mobile.Models;

namespace CampusQuest.Mobile.Services
{
    public class MissionService
    {
       
        public async Task<List<Mission>> GetActiveMissionsAsync()
        {
            await Task.Delay(500); 

            return new List<Mission>
            {
                new Mission
                {
                    Id = 1,
                    Title = "Library Exploration",
                    Description = "Go to the central library, find the QR code near the main desk, and scan it to earn 50 points!",
                    Points = 50,
                    LocationName = "Central Library",
                    IsCompleted = false
                }
            };
        }
    }
}