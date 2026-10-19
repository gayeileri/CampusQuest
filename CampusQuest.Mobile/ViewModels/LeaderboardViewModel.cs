using System.Collections.ObjectModel;
using CampusQuest.Mobile.Services;

namespace CampusQuest.Mobile.ViewModels;

public class LeaderboardViewModel
{
    public LeaderboardUser? FirstPlace { get; set; }
    public LeaderboardUser? SecondPlace { get; set; }
    public LeaderboardUser? ThirdPlace { get; set; }
    public ObservableCollection<LeaderboardUser> RestOfLeaderboard { get; set; } = new();
    
    // For the "You" card at the bottom. In a real app we'd find the logged-in user's record.
    public LeaderboardUser? CurrentUser { get; set; }

    public LeaderboardViewModel(List<LeaderboardUser> users, string currentUsername)
    {
        if (users.Count > 0) FirstPlace = users[0];
        if (users.Count > 1) SecondPlace = users[1];
        if (users.Count > 2) ThirdPlace = users[2];

        for (int i = 3; i < users.Count; i++)
        {
            RestOfLeaderboard.Add(users[i]);
        }

        CurrentUser = users.FirstOrDefault(u => u.Username == currentUsername);
    }
}
