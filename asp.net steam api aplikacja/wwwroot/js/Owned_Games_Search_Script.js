function test() {
    var gameInfoDivHeader = document.getElementById("GameInformationHeader");

    gameInfoDivHeader.innerHTML = "";

    var gameInfoDivMain = document.getElementById("GameInformationMain");

    gameInfoDivMain.innerHTML = "";

    var input = document.getElementById("GameInput").value.toLowerCase();
    var found = false;

    for (var i = 0; i < games.length; i++) {
        var game = games[i];
        var appid = game["appid"].toString();
        var name = game["name"].toString().toLowerCase();
        var img_icon_url = game["img_icon_url"].toString();
        var lasttimeplayed = game["rtime_last_played"]
        var minutesplayed = game["playtime_forever"];
        var hoursplayed = Math.round(minutesplayed / 60.0, 1);
        var Lastplayed_Date = new Date(lasttimeplayed * 1000);

        var data = Lastplayed_Date.toLocaleDateString("pl-PL");

        

        if (name === input || appid === input) {
            found = true;

            document.getElementById("GAME").style = " font-size:25px; border: 2px solid black; border-radius: 15px;"
            
            // show game info header - icon of the game, name and steam id
            gameInfoDivHeader.innerHTML = "<div id='ChosenGameInfoHeader'><h3>" +
                '<img src="http://media.steampowered.com/steamcommunity/public/images/apps/' + appid + '/' + img_icon_url + '.jpg">' +
                "  " + game["name"] + "  (" + game["appid"] + ")</h3></div>";

            // show game info main - hours spent in game, last 2 weeks hours, last time played 
            // played before but not in last 2 weeks
            if (game["playtime_2weeks"] != null && minutesplayed > 0)
            {

                var played2weeks = Math.round(game["playtime_2weeks"] / 60.0 ,1);

                gameInfoDivMain.innerHTML = "Time Played: " + hoursplayed + " Hours" +
                    "<br><br>" +
                    "Time played in the last 2 weeks:  " + played2weeks + " Hours" +
                    "<br><br>" +
                    "Last time played: " + data
                    ;
            }
            // played before and in last 2 weeks
            else if (game["playtime_2weeks"] == null && minutesplayed > 0)
            {
                gameInfoDivMain.innerHTML = "Time Played: " + hoursplayed + " Hours" +
                    "<br><br>" +
                    "Last time played: " + data
                    ;
            }
            // didnt play before or error
            else
            {
                gameInfoDivMain.innerHTML = "This user never played this game before";
            }



            console.log(game);
        }
        if (!found) {
            document.getElementById("GameInformationHeader").innerHTML = "Game not found or error";
        }

    }

}