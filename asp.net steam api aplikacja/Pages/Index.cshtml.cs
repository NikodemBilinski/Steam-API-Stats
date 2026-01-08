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
        public string RawJsonLastPlayed { get; set; }

        public JsonElement RecentGamesArray { get; set; }

        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public string UserStatus { get; set; }

        public int FriendCount;
        public int LastGamesCount;
        public bool Profile_Visible;
        public bool Friendlist_Visible;
        public bool RecentlyPlayed_Visible;
        public bool Games_Visible;


        [BindProperty]
        public string apiKey { get; set; }

        public async Task OnPostAsync()
        {
            // url 1 - Player Summaries
            var url1 = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + apiKey + "&steamids=" + SteamID;

            using var http = new HttpClient();

            
            RawJsonSummaries = await http.GetStringAsync(url1);


            using var jsonDoc = JsonDocument.Parse(RawJsonSummaries);

            var response = jsonDoc.RootElement.GetProperty("response");

            if (response.TryGetProperty("players", out JsonElement User) && User.GetArrayLength() > 0)
            {
                User = jsonDoc.RootElement.GetProperty("response").GetProperty("players")[0];

                UserName = User.GetProperty("personaname").GetString();

                UserAvatar = User.GetProperty("avatarfull").GetString();

                UserStatus = User.GetProperty("personastate").ToString();

            }
            else
            {
                Profile_Visible = false;
                Console.WriteLine("Profil prywatny");
            }
               
            // url 2 - Friends List

            var url2 = "http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key=" + apiKey + "&steamid=" + SteamID+"&relationship=friend";

            
            try
            {
                RawJsonFriendList = await http.GetStringAsync(url2);

                using var JsonDoc2 = JsonDocument.Parse(RawJsonFriendList);

                if (JsonDoc2.RootElement.TryGetProperty("friendslist", out JsonElement FriendList) && FriendList.GetProperty("friends").GetArrayLength() > 0)
                {
                    FriendCount = FriendList.GetProperty("friends").GetArrayLength();
                    Console.WriteLine("list jest");
                    Friendlist_Visible = true;
                }
                else
                {
                    Friendlist_Visible = false;
                    Console.WriteLine("Friends Private");
                }
            }
            catch
            {
                Console.WriteLine("Error");
            }
            

            // url 3 - Last Played

            var url3 = "http://api.steampowered.com/IPlayerService/GetRecentlyPlayedGames/v0001/?key=" + apiKey + "&steamid=" + SteamID + "&format=json" + "&count=9";


            RawJsonLastPlayed = await http.GetStringAsync(url3);

            var JsonDoc3 = JsonDocument.Parse(RawJsonLastPlayed);

            //var LastPlayed = JsonDoc3.RootElement.GetProperty("response").GetProperty("games");

            if(JsonDoc3.RootElement.TryGetProperty("response",out JsonElement test) && test.TryGetProperty("games",out JsonElement RecentGames))
            {
                RecentGamesArray = RecentGames;
                LastGamesCount = RecentGames.GetArrayLength();
                RecentlyPlayed_Visible = true;
            }
            else
            {
                RecentlyPlayed_Visible = false;
                Console.WriteLine("recent games private");
            }


                // url 4 - Games

                //DEBUGGING 

            //Console.WriteLine(RawJsonFriendList);
            //Console.WriteLine(RawJsonLastPlayed);
            //Console.WriteLine("last games: "+ LastGamesCount);
            //Console.WriteLine(FriendCount);
            //Console.WriteLine(UserName);
            //Console.WriteLine(UserAvatar);
            //Console.WriteLine(UserStatus);
        }
    }
}
