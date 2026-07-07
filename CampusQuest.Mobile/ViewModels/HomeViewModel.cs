using System.ComponentModel;
using System.Runtime.CompilerServices;
using CampusQuest.Mobile.Services;
using CampusQuest.Mobile.Models;

namespace CampusQuest.Mobile.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly MissionService _missionService;
        private string _username = "Gaye";
        private int _xp = 250;
        private int _level = 3;
        private string _currentMissionTitle = "Loading...";
        private string _currentMissionDescription = "Please wait while fetching today's quest.";
        private int _nearbyMissionsCount = 0;

        
        public HomeViewModel(MissionService missionService)
        {
            _missionService = missionService;

           
            _ = LoadTodayMissionAsync();
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public int Xp
        {
            get => _xp;
            set { _xp = value; OnPropertyChanged(); }
        }

        public int Level
        {
            get => _level;
            set { _level = value; OnPropertyChanged(); }
        }

        public string CurrentMissionTitle
        {
            get => _currentMissionTitle;
            set { _currentMissionTitle = value; OnPropertyChanged(); }
        }

        public string CurrentMissionDescription
        {
            get => _currentMissionDescription;
            set { _currentMissionDescription = value; OnPropertyChanged(); }
        }

        public int NearbyMissionsCount
        {
            get => _nearbyMissionsCount;
            set { _nearbyMissionsCount = value; OnPropertyChanged(); }
        }

        private async Task LoadTodayMissionAsync()
        {
            var missions = await _missionService.GetActiveMissionsAsync();
            var todayMission = missions.FirstOrDefault();

            if (todayMission != null)
            {
                CurrentMissionTitle = todayMission.Title;
                CurrentMissionDescription = todayMission.Description;
                NearbyMissionsCount = missions.Count; 
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}