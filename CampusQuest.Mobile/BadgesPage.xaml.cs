using CampusQuest.Mobile.Services;

namespace CampusQuest.Mobile;

public partial class BadgesPage : ContentPage
{
    public BadgesPage(List<BadgeItem> badges)
    {
        InitializeComponent();
        
        // Eğer boş liste gelirse veya daha fazla göstermek istersek varsayılan bir liste oluştur
        if (badges == null || badges.Count == 0)
        {
            badges = new List<BadgeItem>
            {
                new BadgeItem { Icon = "🎒", Name = "Çaylak", Description = "Uygulamaya ilk giriş" },
                new BadgeItem { Icon = "🗺️", Name = "Kaşif", Description = "5 farklı yer keşfedildi" },
                new BadgeItem { Icon = "🔥", Name = "Ateşli", Description = "3 gün üst üste giriş" },
                new BadgeItem { Icon = "🎯", Name = "Hedef Odaklı", Description = "İlk görev tamamlandı" },
                new BadgeItem { Icon = "🏃", Name = "Maratoncu", Description = "10 km yürüdün" },
                new BadgeItem { Icon = "⭐", Name = "Süper Star", Description = "Liderlik tablosunda ilk 3'e gir" }
            };
        }
        else if (badges.Count < 4)
        {
            // Olan rozetlere ek olarak birkaç kilitli/diğer rozet gösterebiliriz
            badges.Add(new BadgeItem { Icon = "🎯", Name = "Hedef Odaklı", Description = "İlk görev tamamlandı" });
            badges.Add(new BadgeItem { Icon = "🏃", Name = "Maratoncu", Description = "10 km yürüdün" });
            badges.Add(new BadgeItem { Icon = "⭐", Name = "Süper Star", Description = "Liderlik tablosunda ilk 3'e gir" });
        }

        BadgesCollection.ItemsSource = badges;
    }
}
