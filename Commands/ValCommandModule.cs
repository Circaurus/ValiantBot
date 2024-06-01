using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using ValorantNET;
using ValorantNET.Models;


namespace ValiantBot
{


    public class ValorantApiFunctions
    {

        public async Task<DConfig> GetConfig()
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, "dconfig.json");
            using FileStream openStream = File.OpenRead(fileName);
            DConfig? dConfig =
                await JsonSerializer.DeserializeAsync<DConfig>(openStream);
            return dConfig;
        }
        //Initialize Valorant Client
        public static ValorantClient val = new ValorantClient("VPS Circaurus", "2721", Enums.Regions.NA);

        private static void ShowProp(object obj, InteractionContext com)
        {
            foreach (var p in obj.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any()))
            {
                Console.WriteLine(p.GetValue(obj, null));
                
            }
        }

        public async Task<PlayerInfo> GetPlayerData()
        {
            var result = await val.GetPlayerInfo("28d8d9a6-811e-5cec-99bc-abb58d0d191e", 30, GetConfig().Result.ValToken);
            var result2 = await val.GetPlayerMMR("28d8d9a6-811e-5cec-99bc-abb58d0d191e", GetConfig().Result.ValToken);

            if (result != null)
            {
                result.data[0].currenttier_patched = result2.data.current_data.currenttierpatched;
                Console.WriteLine(result.data[0].currenttier_patched);
            }
            return result;
        }

        //Returns the 3 most played agents for a given user
        public async Task<string[]> GetMostPlayedAgents(PlayerInfo playerInfo)
        {
            string[] commonAgents = new string[3];

            var result = playerInfo;
            if (result != null)
            {
                string[] playedAgents = new string[result.data.Length];

                for (int gameCount = 0; gameCount < result.data.Length; gameCount++)
                {
                    playedAgents[gameCount] = result.data[gameCount].stats.character.name;
                }

                var query = (from item in playedAgents
                             group item by item into g
                             orderby g.Count() descending
                             select new { Item = g.Key, Count = g.Count() });

                for (int i = 0; i < 3; i++)
                {
                    commonAgents[i] = query.ToArray()[i].Item.ToLower();
                    //Console.WriteLine($"{commonAgents[i]}");
                }
            }
            return commonAgents;
        }

        public async Task<string> GetPlayerRank(PlayerInfo playerInfo)
        {
            var result = playerInfo;

            if (result != null)
            {
                return result.data[0].currenttier_patched.Split(" ").First().ToLower();
            }
            return "No Value Returned";
        }

    }

    public class PlayerProfile
    {
        public string? ValName { get; set; }
        public string? ValRank { get; set; }
        public string[]? ValMains { get; set; }
        public string? FavClip { get; set; }
    }

    public class ProfileManager : CommonFunctions
    {
        private Dictionary<string, TaskCompletionSource<string>> _rankSetTask;
        private Dictionary<string, TaskCompletionSource<string>> _mainSetTask;


        public ProfileManager(Dictionary<string, TaskCompletionSource<string>>? rankSetTask = null, Dictionary<string, TaskCompletionSource<string>>? mainSetTask = null)
        {
            _rankSetTask = rankSetTask;
            _mainSetTask = mainSetTask;
        }

        

        public async void CreateProfile(ComponentInteractionCreateEventArgs com, string playerID)
        {

            var options = new JsonSerializerOptions { WriteIndented = true };

            var playerProfile = new PlayerProfile
            {
                ValName = "Username and tag not set",
                ValRank = "Rank not set",
                ValMains = new string[3] {"No Mains Set", "No second set", "no third set"},
                FavClip = "li7nk not set"
            };

            string fileName = "Data/UserIDs/" + playerID + ".json";
            using FileStream createStream = File.Create(fileName);
            await JsonSerializer.SerializeAsync(createStream, playerProfile, options);
            await createStream.DisposeAsync();


            




            await SetUsername(com.Interaction);

            //Set Region

            ValorantApiFunctions valAPI = new ValorantApiFunctions();

            PlayerInfo playerInfo = new PlayerInfo();

            playerInfo = await valAPI.GetPlayerData();



            await SetRank(com.Interaction, playerInfo, valAPI);
            await SetMain(com.Interaction, playerInfo, valAPI);
            await BuildProfile(com.Interaction);
        }

        public async Task SetUsername(DiscordInteraction com)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string playerID = com.User.Id.ToString();

            string fileName = "Data/UserIDs/" + playerID + ".json";
            using FileStream openStream = File.OpenRead(fileName);
            PlayerProfile? playerProfile =
                await JsonSerializer.DeserializeAsync<PlayerProfile>(openStream);
            await openStream.DisposeAsync();

            await com.Channel.SendMessageAsync(BuildSimpleEmbed("Please type out your VALORANT username and ID.\n   eg:Chad#6969"));
            var result = await com.Channel.GetNextMessageAsync(m =>
            {
                playerProfile.ValName = m.Content;
                if (m.Content.ToLower() != null && !m.Author.IsBot)
                {
                    return true;
                }
                return false;
            });
            if (!result.TimedOut) await com.Channel.SendMessageAsync(BuildSimpleEmbed("Username Set!"));

            string writeFileName = "Data/UserIDs/" + playerID + ".json";
            using FileStream WriteStream = File.Create(writeFileName);
            await JsonSerializer.SerializeAsync(WriteStream, playerProfile, options);
            await WriteStream.DisposeAsync();
        }

        public async Task SetRank(DiscordInteraction com, PlayerInfo playerInfo, ValorantApiFunctions valAPI)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            string playerID = com.User.Id.ToString();

            string fileName = "Data/UserIDs/" + playerID + ".json";
            using FileStream openStream = File.OpenRead(fileName);
            PlayerProfile? playerProfile =
                await JsonSerializer.DeserializeAsync<PlayerProfile>(openStream);
            await openStream.DisposeAsync();

            // Create the options for the user to pick
            var RankOptions = new List<DiscordSelectComponentOption>()
        {
            new DiscordSelectComponentOption(
                "Iron",
                "rank_iron"),

            new DiscordSelectComponentOption(
                "Bronze",
                "rank_bronze"),

            new DiscordSelectComponentOption(
                "Silver",
                "rank_silver"),

            new DiscordSelectComponentOption(
                "Gold",
                "rank_gold"),

            new DiscordSelectComponentOption(
                "Platinum",
                "rank_platinum"),

            new DiscordSelectComponentOption(
                "Diamond",
                "rank_diamond"),

            new DiscordSelectComponentOption(
                "Ascendent",
                "rank_ascendent"),

            new DiscordSelectComponentOption(
                "Immortal",
                "rank_immortal"),

            new DiscordSelectComponentOption(
                "Radiant",
                "rank_radiant")
        };

            if (!_rankSetTask.ContainsKey(playerID))
            {
                _rankSetTask[playerID] = new TaskCompletionSource<string>();
            }

            // Make the dropdown
            var dropdown = new DiscordSelectComponent("dropdown", null, RankOptions, false, 1, 1);

            //await com.Channel.SendMessageAsync(BuildDropMessage("Please select your rank.", dropdown));

            //playerProfile.ValRank = _rankSetTask[playerID].Task.Result;
            playerProfile.ValRank = await valAPI.GetPlayerRank(playerInfo);

            await com.Channel.SendMessageAsync(BuildSimpleEmbed("Rank Set!"));

            if (_rankSetTask.ContainsKey(playerID))
            {
                _rankSetTask.Remove(playerID);
            }

            string writeFileName = "Data/UserIDs/" + playerID + ".json";
            using FileStream WriteStream = File.Create(writeFileName);
            await JsonSerializer.SerializeAsync(WriteStream, playerProfile, options);
            await WriteStream.DisposeAsync();
        }

        public async Task SetMain(DiscordInteraction com, PlayerInfo playerInfo, ValorantApiFunctions valAPI)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            string playerID = com.User.Id.ToString();

            string fileName = "Data/UserIDs/" + playerID + ".json";
            using FileStream openStream = File.OpenRead(fileName);
            PlayerProfile? playerProfile =
                await JsonSerializer.DeserializeAsync<PlayerProfile>(openStream);
            await openStream.DisposeAsync();

            // Create the options for the user to pick
            var MainOptions = new List<DiscordSelectComponentOption>()
        {
            new DiscordSelectComponentOption(
                "Jett",
                "agent_jett"),

            new DiscordSelectComponentOption(
                "Phoenix",
                "agent_phoenix"),

            new DiscordSelectComponentOption(
                "Neon",
                "agent_neon"),

            new DiscordSelectComponentOption(
                "Raze",
                "agent_raze"),

            new DiscordSelectComponentOption(
                "Yoru",
                "agent_yoru"),

            new DiscordSelectComponentOption(
                "Reyna",
                "agent_reyna"),

            new DiscordSelectComponentOption(
                "Sova",
                "agent_sova"),

            new DiscordSelectComponentOption(
                "Breach",
                "agent_breach"),

            new DiscordSelectComponentOption(
                "Gekko",
                "agent_gekko"),

            new DiscordSelectComponentOption(
                "Fade",
                "agent_fade"),

            new DiscordSelectComponentOption(
                "KAYO",
                "agent_kayo"),

            new DiscordSelectComponentOption(
                "Skye",
                "agent_skye"),

            new DiscordSelectComponentOption(
                "Brimstone",
                "agent_brimstome"),

            new DiscordSelectComponentOption(
                "Viper",
                "agent_viper"),

            new DiscordSelectComponentOption(
                "Omen",
                "agent_omen"),

            new DiscordSelectComponentOption(
                "Astra",
                "agent_astra"),

            new DiscordSelectComponentOption(
                "Harbor",
                "agent_harbor"),

            new DiscordSelectComponentOption(
                "Sage",
                "agent_sage"),

            new DiscordSelectComponentOption(
                "Cypher",
                "agent_cypher"),

            new DiscordSelectComponentOption(
                "Chamber",
                "agent_chamber"),

            new DiscordSelectComponentOption(
                "Killjoy",
                "agent_killjoy")
        };

            if (!_mainSetTask.ContainsKey(playerID))
            {
                _mainSetTask[playerID] = new TaskCompletionSource<string>();
            }

            string[] agents = await valAPI.GetMostPlayedAgents(playerInfo);

            for (int i = 0; i < 3; i++)
            {
                playerProfile.ValMains[i] = agents[i].ToLower();
            }

            


            await com.Channel.SendMessageAsync(BuildSimpleEmbed("Main Set!"));

            if (_mainSetTask.ContainsKey(playerID))
            {
                _mainSetTask.Remove(playerID);
            }

            string writeFileName = "Data/UserIDs/" + playerID + ".json";
            using FileStream WriteStream = File.Create(writeFileName);
            await JsonSerializer.SerializeAsync(WriteStream, playerProfile, options);
            await WriteStream.DisposeAsync();
        }


        public async Task BuildProfile(DiscordInteraction com, InteractionContext? intcom = null, ContextMenuContext? contcom = null)
        {
            string playerID;
            string playerUsername;
            if (contcom != null)
            {
                playerID = contcom.TargetUser.Id.ToString();
                playerUsername = contcom.TargetUser.Username;

                if (!File.Exists("Data/UserIDs/" + playerID + ".json"))
                {
                    await contcom.CreateResponseAsync(BuildSimpleEmbed("This user does not have a profile set up."));

                    return;
                }
            }
            else
            {
                playerID = com.User.Id.ToString();
                playerUsername = com.User.Username;
            }




            string fileName = "Data/UserIDs/" + playerID + ".json";
            using FileStream openStream = File.OpenRead(fileName);
            PlayerProfile? playerProfile =
                await JsonSerializer.DeserializeAsync<PlayerProfile>(openStream);
            await openStream.DisposeAsync();

            var CreateClip = new DiscordMessageBuilder()
                .WithContent(playerProfile.FavClip);

            var fs = new FileStream(".\\Data\\AgentImages\\" + playerProfile.ValMains[0] + ".jpg", FileMode.Open, FileAccess.Read);

            var msg = new DiscordMessageBuilder()
                .AddFile(playerProfile.ValMains[0] + ".jpg", fs);

            if (intcom == null & contcom == null)
            {
                //await com.Channel.SendMessageAsync(embuilder);
                //await com.Channel.SendMessageAsync(CreateClip);
            }
            else if (intcom != null & contcom == null)
            {
                //await intcom.CreateResponseAsync(embuilder);
                //await intcom.Channel.SendMessageAsync(msg);
            }
            else
            {
                //await contcom.CreateResponseAsync(embuilder);
                //await com.Channel.SendMessageAsync(playerProfile.FavClip);
            }

        }
    }

    public class CommonFunctions
    {
        public DiscordEmbedBuilder BuildSimpleEmbed(string text)
        {
            var embuilder = new DiscordEmbedBuilder()
                .WithTitle(text);
            return embuilder;
        }

        public DiscordMessageBuilder BuildDropMessage(string text, DiscordSelectComponent dropdown)
        {
            var mbuilder = new DiscordMessageBuilder()
                .WithContent(text)
                .AddComponents(dropdown);
            return mbuilder;
        }
    }

    public class ValCommandModule : ApplicationCommandModule
    {
        CommonFunctions comfunc = new CommonFunctions();

        [SlashCommand("profile", "Checks/Sets up a Valorant Profile for the User")]
        public async Task ProfileCommand(InteractionContext com)
        {
            string playerID = com.Interaction.User.Id.ToString();

            if (!File.Exists("Data/UserIDs/" + playerID + ".json"))
            {

                var CreateProfileButton = new DiscordMessageBuilder()
                .WithContent(" ")
                .AddComponents(new DiscordComponent[]
                {
                new DiscordButtonComponent(ButtonStyle.Primary, "CREATE_ONE", "Create one!")
                });


                await com.Member.SendMessageAsync(comfunc.BuildSimpleEmbed("It seems that I don't have a profile for you. If you would like to create one, please have your VALORANT username and your own favorite clip ready!"));
                await com.Member.SendMessageAsync(CreateProfileButton);
                await com.CreateResponseAsync(comfunc.BuildSimpleEmbed("I've sent you a DM to finalize setup!"));
            }
            else
            {
                var pm = new ProfileManager();

                await pm.BuildProfile(com.Interaction, com);

            }
        }

        [SlashCommand("remove", "Removes a Valorant Profile for the User")]
        public async Task RemoveProfileCommand(InteractionContext com)
        {
            string playerID = com.Interaction.User.Id.ToString();

            if (File.Exists("Data/UserIDs/" + playerID + ".json"))
            {
                File.Delete("Data/UserIDs/" + playerID + ".json");
            }
            await com.CreateResponseAsync(comfunc.BuildSimpleEmbed("Profile Deleted!"));
        }


        [SlashCommand("test", "test command for WIP commands")]
        public async Task TestCommand(InteractionContext com)
        {
            string playerID = com.Interaction.User.Id.ToString();

            var fs = new FileStream(".\\Data\\AgentImages\\yoru.jpg", FileMode.Open, FileAccess.Read);

            var msg = new DiscordMessageBuilder()
                .AddFile("yoru.jpg", fs);

            var testembuilder = new DiscordEmbedBuilder()
                .WithImageUrl("attachment://yoru.jpg");

            ValorantApiFunctions val = new ValorantApiFunctions();

            val.GetPlayerData();

        }

        [ContextMenu(ApplicationCommandType.UserContextMenu, "View Valorant Profile")]
        public async Task UserMenu(ContextMenuContext com)
        {
            ProfileManager pm = new ProfileManager();

            await pm.BuildProfile(com.Interaction, contcom: com);
        }
    }

}
