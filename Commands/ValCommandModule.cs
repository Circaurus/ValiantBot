using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System.Text.Json;
public class PlayerProfile
{
    public string? ValName { get; set; }
    public string? ValRank { get; set; }
    public string? ValMain { get; set; }
    public string? FavClip { get; set; }
}

public class ProfileManager : CommonFunctions
{
    private Dictionary<string, TaskCompletionSource<string>> _rankSetTask;
    private Dictionary<string, TaskCompletionSource<string>> _mainSetTask;


    public ProfileManager(Dictionary<string, TaskCompletionSource<string>> rankSetTask = null, Dictionary<string, TaskCompletionSource<string>> mainSetTask = null)
    {
        _rankSetTask = rankSetTask;
        _mainSetTask = mainSetTask;
    }
    public async void CreateProfile(ComponentInteractionCreateEventArgs com, string playerID)
    {

        var options = new JsonSerializerOptions { WriteIndented = true };

        var playerProfile = new PlayerProfile
        {
            ValName = "Username not set",
            ValRank = "Rank not set",
            ValMain = "Main not set",
            FavClip = "link not set"
        };

        string fileName = "Data/UserIDs/" + playerID + ".json";
        using FileStream createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, playerProfile, options);
        await createStream.DisposeAsync();

        await SetUsername(com.Interaction);
        await SetRank(com.Interaction);
        await SetMain(com.Interaction);
        await SetClip(com.Interaction);
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
            return m.Content.ToLower() != null;
        });
        if (!result.TimedOut) await com.Channel.SendMessageAsync(BuildSimpleEmbed("Username Set!"));

        string writeFileName = "Data/UserIDs/" + playerID + ".json";
        using FileStream WriteStream = File.Create(writeFileName);
        await JsonSerializer.SerializeAsync(WriteStream, playerProfile, options);
        await WriteStream.DisposeAsync();
    }

    public async Task SetRank(DiscordInteraction com)
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

        await com.Channel.SendMessageAsync(BuildDropMessage("Please select your rank.", dropdown));

        playerProfile.ValRank = _rankSetTask[playerID].Task.Result;

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

    public async Task SetMain(DiscordInteraction com)
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

        // Make the dropdown
        var dropdown = new DiscordSelectComponent("dropdown", null, MainOptions, false, 1, 1);

        await com.Channel.SendMessageAsync(BuildDropMessage("Please select your main.", dropdown));

        playerProfile.ValMain = _mainSetTask[playerID].Task.Result;


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

    public async Task SetClip(DiscordInteraction com)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string playerID = com.User.Id.ToString();

        string fileName = "Data/UserIDs/" + playerID + ".json";
        using FileStream openStream = File.OpenRead(fileName);
        PlayerProfile? playerProfile =
            await JsonSerializer.DeserializeAsync<PlayerProfile>(openStream);
        await openStream.DisposeAsync();

        await com.Channel.SendMessageAsync(BuildSimpleEmbed("Please paste a link to your favorite clip of yours.\n   (I do not support uploaded files, only links.)"));
        var result = await com.Channel.GetNextMessageAsync(m =>
        {
            playerProfile.FavClip = m.Content;
            return m.Content.ToLower() != null;
        });
        if (!result.TimedOut) await com.Channel.SendMessageAsync(BuildSimpleEmbed("Favorite Clip Set!"));

        string writeFileName = "Data/UserIDs/" + playerID + ".json";
        using FileStream WriteStream = File.Create(writeFileName);
        await JsonSerializer.SerializeAsync(WriteStream, playerProfile, options);
        await WriteStream.DisposeAsync();
    }


    public async Task BuildProfile(DiscordInteraction com, InteractionContext intcom = null, ContextMenuContext contcom = null)
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

        var ranks = new Dictionary<string, string>()
        {
            {"iron", "https://media.discordapp.net/attachments/954563370747588660/1086274849242087484/Iron_3_Rank.png"},
            {"bronze", "https://media.discordapp.net/attachments/954563370747588660/1086274848218697778/Bronze_3_Rank.png"},
            {"silver", "https://media.discordapp.net/attachments/954563370747588660/1085974827850027121/Silver_3_Rank.png"},
            {"gold", "https://media.discordapp.net/attachments/954563370747588660/1086274848738791474/Gold_3_Rank.png"},
            {"platinum", "https://media.discordapp.net/attachments/954563370747588660/1086274849464406109/Platinum_3_Rank.png"},
            {"diamond", "https://media.discordapp.net/attachments/954563370747588660/1086274848516485150/Diamond_3_Rank.png"},
            {"ascendent", "https://media.discordapp.net/attachments/954563370747588660/1086274847946047508/Ascendant_3_Rank.png"},
            {"immortal", "https://media.discordapp.net/attachments/954563370747588660/1086274848977858640/Immortal_3_Rank.png"},
            {"radiant", "https://media.discordapp.net/attachments/954563370747588660/1086274849657335878/Radiant_Rank.png"}
        };

        var agents = new Dictionary<string, string>()
        {
            {"jett", "https://media.discordapp.net/attachments/954563370747588660/1088255578217914511/jett.png?width=900&height=506"},
            {"phoenix", "https://media.discordapp.net/attachments/954563370747588660/1088255579996307496/phoenix.jpg?width=1015&height=571"},
            {"raze", "https://media.discordapp.net/attachments/954563370747588660/1088255623814197319/raze.jpg?width=1015&height=571"},
            {"neon", "https://media.discordapp.net/attachments/954563370747588660/1088255579404910692/neon.jpg?width=1015&height=571"},
            {"reyna", "https://media.discordapp.net/attachments/954563370747588660/1088255624195866794/reyna.jpg?width=1015&height=571"},
            {"yoru", "https://media.discordapp.net/attachments/954563370747588660/1088255876797841448/yoru.jpg?width=1015&height=571"},
            {"sova", "https://media.discordapp.net/attachments/954563370747588660/1088255625512878230/sova.jpg?width=1015&height=571"},
            {"gekko", "https://media.discordapp.net/attachments/954563370747588660/1088256819580907520/gekko.jpg?width=895&height=571"},
            {"fade", "https://media.discordapp.net/attachments/954563370747588660/1088257058152919040/fade.jpg?width=1015&height=571"},
            {"kayo", "https://media.discordapp.net/attachments/954563370747588660/1088255578591199332/kayo.jpg?width=1015&height=571"},
            {"skye", "https://media.discordapp.net/attachments/954563370747588660/1088255624913105023/skye.jpg?width=1015&height=571"},
            {"breach", "https://media.discordapp.net/attachments/954563370747588660/1088257836229853184/breach.jpg?width=1015&height=571"},
            {"brimstone", "https://media.discordapp.net/attachments/954563370747588660/1088257874851008633/valorant-agent-brimstone.jpg?width=1142&height=571"},
            {"viper", "https://media.discordapp.net/attachments/954563370747588660/1088256147963773029/viper.jpg?width=1015&height=571"},
            {"astra", "https://media.discordapp.net/attachments/954563370747588660/1088255754269642822/astra.jpg?width=1015&height=571"},
            {"harbor", "https://media.discordapp.net/attachments/954563370747588660/1088256148248993894/Valorant-character-harbor.jpg?width=1015&height=571"},
            {"omen", "https://media.discordapp.net/attachments/954563370747588660/1088255579685912606/omen.jpg?width=1015&height=571"},
            {"sage", "https://media.discordapp.net/attachments/954563370747588660/1088255624518832168/sage.png?width=987&height=555"},
            {"chamber", "https://media.discordapp.net/attachments/954563370747588660/1088255754504507472/chamber.jpg?width=1015&height=571"},
            {"killjoy", "https://media.discordapp.net/attachments/954563370747588660/1088255578981277758/killjoy.jpg?width=1015&height=571"},
            {"cypher", "https://media.discordapp.net/attachments/954563370747588660/1088255754735198270/cypher.jpg?width=1015&height=571"},
        };

        var colors = new Dictionary<string, string>()
        {
            {"jett", "bababa"},
            {"phoenix", "e25822"},
            {"raze", "ff9245"},
            {"neon", "45d7ff"},
            {"reyna", "8700b8"},
            {"yoru", "3679ff"},
            {"sova", "296dff"},
            {"gekko", "59ff5f"},
            {"fade", "ff0000"},
            {"kayo", "b057ff"},
            {"skye", "33bd2b"},
            {"breach", "ff7b00"},
            {"brimstone", "ff7b00"},
            {"viper", "08e800"},
            {"astra", "b300ff"},
            {"harbor", "59a9ff"},
            {"omen", "6753b5"},
            {"sage", "40ffec"},
            {"chamber", "ffdd00"},
            {"killjoy", "eeff00"},
            {"cypher", "ffffff"},
        };

        DiscordColor color = new DiscordColor(colors[playerProfile.ValMain]);

        var embuilder = new DiscordEmbedBuilder()
            .WithTitle(playerUsername + "'s Valorant Profile")
            .WithFooter(playerProfile.ValName)
            .WithThumbnail(ranks[playerProfile.ValRank])
            .WithImageUrl(agents[playerProfile.ValMain])
            .WithColor(color);

        var CreateClip = new DiscordMessageBuilder()
            .WithContent(playerProfile.FavClip);


        if (intcom == null & contcom == null)
        {
            await com.Channel.SendMessageAsync(embuilder);
            await com.Channel.SendMessageAsync(CreateClip);
        }
        else if (intcom != null & contcom == null)
        {
            await intcom.CreateResponseAsync(embuilder);
            await intcom.Channel.SendMessageAsync(CreateClip);
        }
        else
        {
            await contcom.CreateResponseAsync(embuilder);
            await com.Channel.SendMessageAsync(playerProfile.FavClip);
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

    [ContextMenu(ApplicationCommandType.UserContextMenu, "View Valorant Profile")]
    public async Task UserMenu(ContextMenuContext com)
    {
        var pm = new ProfileManager();

        await pm.BuildProfile(com.Interaction, contcom: com);
    }
}