using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;

namespace asp.net_steam_api_aplikacja.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string SteamID { get; set; }

        public string RawJson { get; set; }
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
        }
    }
}
