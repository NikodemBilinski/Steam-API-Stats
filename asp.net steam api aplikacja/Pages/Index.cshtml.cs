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

        public string RawJson { get; set; }

        public string UserName { get; set; }

        public string UserAvatar { get; set; }

        public string UserStatus { get; set; }


        [BindProperty]
        public string apiKey { get; set; }

        public async Task OnPostAsync()
        {
            var url = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + apiKey + "&steamids=" + SteamID;

            // "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key="+apiKey+"&steamids="+SteamID;
            // https://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=NOWY_KLUCZ&steamid=STEAMID&include_appinfo=1 For debugging purposes

            Console.WriteLine(SteamID);
            Console.WriteLine(apiKey);
            using var http = new HttpClient();

            RawJson = await http.GetStringAsync(url);

            using var jsonDoc = JsonDocument.Parse(RawJson);

            var User = jsonDoc.RootElement.GetProperty("response").GetProperty("players")[0];

            UserName = User.GetProperty("personaname").GetString();

            UserAvatar = User.GetProperty("avatarfull").GetString();

            UserStatus = User.GetProperty("personastate").ToString();


            Console.WriteLine(UserName);
            Console.WriteLine(UserAvatar);
            Console.WriteLine(UserStatus);
        }
    }
}
