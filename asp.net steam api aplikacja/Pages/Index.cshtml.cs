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
        
        public async Task OnPostAsync()
        {
            string apiKey = "133CF098DBC76C70555D8247AFD8A5A4";
            var url = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key="+apiKey+"&steamids="+SteamID;dawdwa

            Console.WriteLine(SteamID);

            Console.WriteLine(apiKey);
            using var http = new HttpClient();

            RawJson = await http.GetStringAsync(url);
        }
    }
}
