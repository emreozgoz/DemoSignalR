using System.Collections.Concurrent;

namespace API.HubServices
{
    public class HubService
    {
        //IEnumerable, sadece okumak için kullanılabilir, bu da bu koleksiyonun dışarıdan değiştirilmesini önler,
        //Readonly ile birleştirildiğinde, ChatGroups'un sabit kalmasını sağlar.
        public readonly IEnumerable<string> ChatGroups = ["Administrators", "Managers", "Users"];

        //Çoklu iş parçacıklarının aynı anda veri ekleyip güncelleyebilmesi için tasarlanmış bir koleksiyondur.
        //Bu, özellikle çoklu istemcilerin aynı anda bağlantı kurduğu durumlarda önemlidir.
        public readonly ConcurrentDictionary<string, string> ConnectionGroups = new();

        public List<string> GetMembers(string groupName)
        {
            //ConnectionGroups sözlüğündeki tüm anahtarları (yani bağlantı kimliklerini) alır.
            //Bu anahtarlar, grup üyelerini bulmak için kullanılacaktır.
            List<string> allConnectionIds = [.. ConnectionGroups.Keys];
            List<string> groupMembers = [];

            foreach (string connectionId in allConnectionIds)
            {
                var group = ConnectionGroups.FirstOrDefault(x => x.Key == connectionId).Value;
                if (Equals(groupName, group))
                    groupMembers.Add(connectionId);
            }
            return groupMembers;
        }

        public bool IsUserInGroup(string connectionId)
        {
            ConnectionGroups.TryGetValue(connectionId, out string? connectedGroupName);
            if (string.IsNullOrEmpty(connectedGroupName))
                return false;
            else
                return true;
        }

        public bool FindGroupName(string groupName)
        {
            var group = ChatGroups.FirstOrDefault(groupName);
            if (string.IsNullOrEmpty(group))
                return false;
            else
                return true;
        }


        //Bu iki metod, bir bağlantı kimliğine (connection ID) karşılık gelen grup adını almak ve mevcut grup isimlerini listelemek için kullanılıyor.
        public string GetUserGroupName(string connectionId) =>
            ConnectionGroups[connectionId];

        public IEnumerable<string> GetAvailableGroups() => ChatGroups;
    }
}
