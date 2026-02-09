using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace asp.net_steam_api_aplikacja.Pages
{

    public class IndexModel : PageModel
    {
        [BindProperty]
        public string SteamID { get; set; }

        public string RawJsonSummaries { get; set; }
        public string RawJsonFriendList { get; set; }
        public string RawJsonLastPlayed { get; set; }

        public string RawJsonGames { get; set; }

        public JsonElement RecentGamesArray { get; set; }

        public JsonElement OwnedGamesArray { get; set; }

        public JsonElement FriendsListArray { get; set; }

        public List<JsonElement> FiveOldestFriends { get; set; }
        public List<JsonElement> FiveOldestFriendsInfo { get; set; }
        public List<JsonElement> FiveNewestFriends { get; set; }
        public List<JsonElement> FiveNewestFriendsInfo { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public string UserStatus { get; set; }

        public int OldestFriendTime = int.MaxValue;
        public string OldestFriendID;
        public int FriendCount;

        public int LastGamesCount;
        public int GameCount;
        public string steamgameid;
        public bool Profile_Visible;
        public bool Friendlist_Visible;
        public bool RecentlyPlayed_Visible;
        public bool Games_Visible;


        [BindProperty]
        public string apiKey { get; set; }

        public async Task OnPostAsync()
        {

            using var http = new HttpClient();

            await GetPlayerSummaries(http);
            await GetFriendList(http);
            await GetLastPlayedGames(http);
            await GetOwnedGames(http);



            //DEBUGGING 

            //Console.WriteLine(RawJsonFriendList);
            //Console.WriteLine(RawJsonLastPlayed);
            //Console.WriteLine("last games: "+ LastGamesCount);
            //Console.WriteLine(FriendCount);
            //Console.WriteLine(UserName);
            //Console.WriteLine(UserAvatar);
            //Console.WriteLine(UserStatus);
        }

        public async Task GetPlayerSummaries(HttpClient http)
        {
            // url 1 - Player Summaries
            var url1 = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + apiKey + "&steamids=" + SteamID;

            try
            {
                RawJsonSummaries = await http.GetStringAsync(url1);


                using var jsonDoc = JsonDocument.Parse(RawJsonSummaries);

                var response = jsonDoc.RootElement.GetProperty("response");

                if (response.TryGetProperty("players", out JsonElement User) && User.GetArrayLength() > 0)
                {
                    User = jsonDoc.RootElement.GetProperty("response").GetProperty("players")[0];

                    UserName = User.GetProperty("personaname").GetString();

                    UserAvatar = User.GetProperty("avatarfull").GetString();

                    UserStatus = User.GetProperty("personastate").ToString();

                    Console.WriteLine("Successfully pulled PlayerSummaries data.");

                }
                else
                {
                    Profile_Visible = false;
                    Console.WriteLine("Steam user's profile is private.");
                }
            }
            catch
            {
                Console.WriteLine("Error while pulling PlayerSummaries data.");
            }
        }

        public async Task GetFriendList(HttpClient http)
        {
            // url 2 - Friends List     DODAC TOP 5 NAJSTARSZYCH ZNAJOMYCH NA STEAM POD ILOSCIA ZNAJOMYCH NA NEXT

            var url2 = "http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key=" + apiKey + "&steamid=" + SteamID + "&relationship=friend";


            try
            {
                RawJsonFriendList = await http.GetStringAsync(url2);

                var JsonDoc2 = JsonDocument.Parse(RawJsonFriendList);

                //JsonDoc2.RootElement.TryGetProperty("friendslist", out JsonElement FriendList) && FriendList.GetProperty("friends").GetArrayLength() > 0

                if (JsonDoc2.RootElement.TryGetProperty("friendslist", out JsonElement FriendList) && FriendList.TryGetProperty("friends", out JsonElement Friends))
                {
                    // code for friend list

                    Friendlist_Visible = true;
                    FriendCount = FriendList.GetProperty("friends").GetArrayLength();
                    FriendsListArray = FriendList.GetProperty("friends");

                    var TempList = new List<JsonElement>();



                    for (int i = 0; i < FriendCount; i++)
                    {
                        TempList.Add(FriendsListArray[i]);
                    }


                    TempList.Sort((a, b) => a.GetProperty("friend_since").GetInt32().CompareTo(b.GetProperty("friend_since").GetInt32()));

                    // oldest friends 
                    FiveOldestFriends = TempList.GetRange(0, (Math.Min(5, TempList.Count())));


                    var ids = FiveOldestFriends.Select(x => x.GetProperty("steamid").GetString());

                    string joined_ids = string.Join(",", ids);


                    var url_Friends = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={joined_ids}";

                    var Friends_Response = await http.GetStringAsync(url_Friends);

                    using var OldestFriends_Parse = JsonDocument.Parse(Friends_Response);

                    FiveOldestFriendsInfo = OldestFriends_Parse.RootElement.GetProperty("response").GetProperty("players").EnumerateArray()
                        .Select(p => p.Clone()).ToList();


                    // newest friends
                    FiveNewestFriends = TempList.OrderByDescending(x => x.GetProperty("friend_since").GetInt32()).Take(5).ToList();

                    ids = FiveNewestFriends.Select(x => x.GetProperty("steamid").GetString());

                    joined_ids = string.Join(",", ids);

                    url_Friends = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={joined_ids}";


                    Friends_Response = await http.GetStringAsync(url_Friends);

                    using var NewestFriends_Parse = JsonDocument.Parse(Friends_Response);



                    FiveNewestFriendsInfo = NewestFriends_Parse.RootElement.GetProperty("response").GetProperty("players").EnumerateArray()
                        .Select(x => x.Clone()).ToList();



                    Console.WriteLine(FiveNewestFriendsInfo[1].GetProperty("avatar").ToString());

                    Console.WriteLine("Successfully pulled Friends data.");
                }
                else
                {
                    Friendlist_Visible = false;
                    Console.WriteLine("Friends data are private.");
                }
            }


            catch
            {
                Console.WriteLine("Error while pulling Friends data.");
            }
}

        public async Task GetLastPlayedGames(HttpClient http)
        {
            // url 3 - Last Played

            var url3 = "http://api.steampowered.com/IPlayerService/GetRecentlyPlayedGames/v0001/?key=" + apiKey + "&steamid=" + SteamID + "&format=json" + "&count=9";

            try
            {
                RawJsonLastPlayed = await http.GetStringAsync(url3);

                var JsonDoc3 = JsonDocument.Parse(RawJsonLastPlayed);

                //var LastPlayed = JsonDoc3.RootElement.GetProperty("response").GetProperty("games");

                if (JsonDoc3.RootElement.TryGetProperty("response", out JsonElement test) && test.TryGetProperty("games", out JsonElement RecentGames))
                {
                    RecentGamesArray = RecentGames;
                    LastGamesCount = RecentGamesArray.GetArrayLength();

                    RecentlyPlayed_Visible = true;
                    Console.WriteLine("Successfully pulled RecentlyPlayedGames data.");
                }
                else
                {
                    RecentlyPlayed_Visible = false;
                    Console.WriteLine("RecentGames data are private.");
                }
            }
            catch
            {
                Console.WriteLine("Error while pulling RecentlyPlayedGames data.");
                RecentlyPlayed_Visible = false;
            }
        }

        public async Task GetOwnedGames(HttpClient http)
        {
            // url 4 - Games   DODAC TOP NAJDLUZEJ GRANYCH GIER, ZERKNAC ROWNIEZ DO GetOwnedGames
            // I SPRAWDZIC CZY DA SIE COS WYKOMBINOWAC

            var url4 = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + apiKey + "&steamid=" + SteamID + "&format=json&include_appinfo=1";
            try
            {
                RawJsonGames = await http.GetStringAsync(url4);

                var JsonDoc4 = JsonDocument.Parse(RawJsonGames);

                if(JsonDoc4.RootElement.TryGetProperty("response",out JsonElement OwnedGamesResponse) && OwnedGamesResponse.TryGetProperty("games",out JsonElement OwnedGames))
                {
                    GameCount = OwnedGamesResponse.GetProperty("game_count").GetInt32();

                    OwnedGamesArray = OwnedGames;

                    //Console.WriteLine("Game count: " + GameCount);
                    //Console.WriteLine(OwnedGamesArray[54]);

                    Console.WriteLine("Successfully pulled OwnedGames data.");

                }
                else
                {
                    Console.WriteLine("OwnedGames data are private.");
                }

                //Console.WriteLine(JsonDoc4.RootElement.ToString());

            }
            catch
            {
                Console.WriteLine("Error while pulling OwnedGames data.");
            }
        }

    }
}
