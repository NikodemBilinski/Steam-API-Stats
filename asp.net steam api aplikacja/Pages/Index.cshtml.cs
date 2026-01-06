using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;

namespace asp.net_steam_api_aplikacja.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string SteamID { get; set; }

        public string RawJsonSummaries { get; set; }

        public string RawJsonFriendList { get; set; }

        public string UserName { get; set; }

        public string UserAvatar { get; set; }

        public string UserStatus { get; set; }

        public int FriendCount { get; set; }


        [BindProperty]
        public string apiKey { get; set; }

        public async Task OnPostAsync()
        {
            var url1 = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + apiKey + "&steamids=" + SteamID;

            // "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key="+apiKey+"&steamids="+SteamID;
            // https://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=NOWY_KLUCZ&steamid=STEAMID&include_appinfo=1 For debugging purposes

            Console.WriteLine(SteamID);
            Console.WriteLine(apiKey);
            using var http = new HttpClient();

            RawJsonSummaries = await http.GetStringAsync(url1);

            using var jsonDoc = JsonDocument.Parse(RawJsonSummaries);

            var User = jsonDoc.RootElement.GetProperty("response").GetProperty("players")[0];

            UserName = User.GetProperty("personaname").GetString();

            UserAvatar = User.GetProperty("avatarfull").GetString();

            UserStatus = User.GetProperty("personastate").ToString();

            var url2 = "http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key=" + apiKey + "&steamid=" + SteamID+"&relationship=friend";

            using var http2 = new HttpClient();

            RawJsonFriendList = await http2.GetStringAsync(url2);

            using var JsonDoc2 = JsonDocument.Parse(RawJsonFriendList);

            var FriendList = JsonDoc2.RootElement.GetProperty("friendslist").GetProperty("friends");

            FriendCount = FriendList.GetArrayLength();



            Console.WriteLine(FriendCount);
           /* Console.WriteLine(UserName);
            Console.WriteLine(UserAvatar);
            Console.WriteLine(UserStatus);*/
        }
    }
}
