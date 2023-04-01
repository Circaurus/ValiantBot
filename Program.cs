using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System.Text.Json;

namespace ValiantBot
{
    public class DConfig
    {
        public string? ClientToken { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {

            string fileName = Path.Combine(Environment.CurrentDirectory, "dconfig.json");
            using FileStream openStream = File.OpenRead(fileName);
            DConfig? dConfig =
                await JsonSerializer.DeserializeAsync<DConfig>(openStream);


            //Initialize Client
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = dConfig.ClientToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });

            //Enable Interactivity Module
            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });

            //Register Command Modules
            var slash = discord.UseSlashCommands();
            slash.RegisterCommands<ValCommandModule>(900451370761605220);

            DiscordActivity activity = new();
            activity.Name = "ball with Brimstone";
            activity.ActivityType = ActivityType.Streaming;
            discord.Ready += async (client, readyEventArgs) =>
                await discord.UpdateStatusAsync(activity);

            Dictionary<string, TaskCompletionSource<string>> _rankSetTasks = new Dictionary<string, TaskCompletionSource<string>>();
            Dictionary<string, TaskCompletionSource<string>> _mainSetTasks = new Dictionary<string, TaskCompletionSource<string>>();
            //Handle Button Presses
            discord.ComponentInteractionCreated += async (s, e) =>
            {
                string userId = e.User.Id.ToString();
                if (!_rankSetTasks.ContainsKey(userId))
                {
                    _rankSetTasks[userId] = new TaskCompletionSource<string>();
                }

                if (!_mainSetTasks.ContainsKey(userId))
                {
                    _mainSetTasks[userId] = new TaskCompletionSource<string>();
                }

                var pm = new ProfileManager(_rankSetTasks, _mainSetTasks);
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                switch (e.Id)
                {
                    case "CREATE_ONE":
                        pm.CreateProfile(e, e.User.Id.ToString());
                        break;
                }
                if (e.Values[0] != null & e.Values[0].Contains("rank"))
                {
                    //await pm.GetSetRanks(e, e.Values[0]);
                    _rankSetTasks[userId].SetResult(e.Values[0].Split('_').Last());
                }
                else if (e.Values[0] != null & e.Values[0].Contains("agent"))
                {
                    _mainSetTasks[userId].SetResult(e.Values[0].Split('_').Last());
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}