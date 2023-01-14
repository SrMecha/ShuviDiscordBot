using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Shuvi.Services;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Interactions;
using Shuvi.Classes.Map;
using Shuvi.Interfaces.User;
using Shuvi.Classes.Enemy;
using Shuvi.Classes.Characteristics;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Status;
using Shuvi.Classes.Status;
using Shuvi.Classes.Player;
using MongoDB.Driver;
using Shuvi.Classes.User;
using Shuvi.StaticServices.UserCheck;
using Shuvi.Enums;

namespace Shuvi.Modules.SlashCommands
{
    public class HuntCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;
        private readonly WorldMap _map;
        private bool _isAfk = false;

        private const int _huntEnergyCost = 1;
        private const int _maxHod = 100;

        public HuntCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
            _map = _database.Info.Map;
        }

        [SlashCommand("hunt", "Охота на монстров.")]
        public async Task HuntCommandAsync()
        {
            await DeferAsync();
            var param = new InteractionParameters(await GetOriginalResponseAsync(), null);
            var dbUser = await _database.Users.GetUser(Context.User.Id);
            if (!dbUser.Energy.HaveEnergy(_huntEnergyCost))
            {
                await ModifyOriginalResponseAsync(
                    msg => { msg.Embed = ErrorEmbedBuilder.Simple("У вас не хватает энергии."); }
                    );
                return;
            }
            if (UserCommandsCheck.IsUseCommands(Context.User.Id, ActiveCommands.Hunt))
            {
                await ModifyOriginalResponseAsync(
                    msg => { msg.Embed = UserCommandsCheck.GetErrorEmbed(Context.User.Id, ActiveCommands.Hunt); }
                    );
                return;
            }
            if (_map.GetRegion(dbUser.Location.MapRegion).GetLocation(dbUser.Location.MapLocation).Enemies.Count < 1)
            {
                var embed = new UserEmbedBuilder(Context.User)
                    .WithDescription("Кажеться, в этой локации не на кого поохотиться. Попробуйте поискать врагов в другой локации.")
                    .Build();
                await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; });
                return;
            }
            UserCommandsCheck.Add(Context.User.Id, ActiveCommands.Hunt);
            await FightPrepareAsync(param, dbUser);
            UserCommandsCheck.Remove(Context.User.Id, ActiveCommands.Hunt);
        }
        public async Task FightPrepareAsync(InteractionParameters param, IDatabaseUser dbUser)
        {
            var enemies = _map.GetRegion(dbUser.Location.MapRegion).GetLocation(dbUser.Location.MapLocation).EnemiesWithChance;
            IEnemy? enemy;
            while (true)
            {
                dbUser.Energy.ReduceEnergy(_huntEnergyCost);
                await _database.Users.UpdateUser(dbUser.Id, new UpdateDefinitionBuilder<UserData>().Set("EnergyRegenTime", dbUser.Energy.RegenTime));
                enemy = EnemyFactory.GetEnemy(enemies.GetRandom(), "hunt");
                var embed = new FightEmbedBuilder(Context.User.GetAvatarUrl(), Context.User.Username, enemy.Name)
                    .WithAuthor("Охота")
                    .WithDescription($"Вы встретили {enemy.Name}!\n" +
                    $"**Энергии осталось:** {dbUser.Energy.GetCurrentEnergy()}/{dbUser.Energy.Max}")
                    .AddField(enemy.Name,
                    ((UserCharacteristics)enemy.Characteristics).ToRusString(enemy.EffectBonuses) +
                    $"\n{enemy.Mana.ToString()}\n{enemy.Health.ToString()}",
                    true)
                    .AddField("Дополнительно:",
                    $"**Заклинание:** {enemy.Spell.Info.Name}",
                    true)
                    .Build();
                var components = new ComponentBuilder()
                    .WithButton("Напасть", "fight", ButtonStyle.Danger, row: 0)
                    .WithButton("Сбежать", "leave", ButtonStyle.Success, row: 0)
                    .WithButton("Поискать другого противника", "retry", ButtonStyle.Secondary, 
                    disabled: !dbUser.Energy.HaveEnergy(_huntEnergyCost), row: 1)
                    .Build();
                await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
                if (param.Interaction != null)
                    await param.Interaction.DeferAsync();
                param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
                if (param.Interaction == null)
                {
                    await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = new ComponentBuilder().Build(); });
                    return;
                }
                switch (param.Interaction.Data.CustomId)
                {
                    case "fight":
                        await MainFightAsync(param, dbUser, new PlayerBase(Context.User.Username, dbUser), enemy);
                        return;
                    case "leave":
                        embed = new UserEmbedBuilder(Context.User)
                            .WithAuthor("Охота")
                            .WithDescription("Вы сбежали.")
                            .Build();
                        await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = new ComponentBuilder().Build(); });
                        return;
                    case "retry":
                        if (!dbUser.Energy.HaveEnergy(_huntEnergyCost))
                        {
                            await param.Interaction.RespondAsync(embed: ErrorEmbedBuilder.Simple("У вас не хватает энергии."), ephemeral: true);
                            await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = new ComponentBuilder().Build(); });
                            return;
                        }
                        break;
                    default:
                        return;
                }
            }
        }
        public async Task MainFightAsync(InteractionParameters param, IDatabaseUser dbUser, IPlayer player, IEnemy enemy)
        {
            var status = new FightStatus();
            status.AddDescription("Сражение началось!");
            while (true)
            {
                if (status.Hod > _maxHod)
                {
                    await FightDrawAsync(param, dbUser, player, enemy, status);
                    return;
                }
                if (_isAfk)
                    PlayerAutoHod(param, player, enemy, status);
                else
                    await PlayerHodAsync(param, player, enemy, status);
                if (enemy.IsDead)
                {
                    await FightWinAsync(param, dbUser, player, enemy, status);
                    return;
                }
                EnemyHod(player, enemy, status);
                if (player.IsDead)
                {
                    await FightLooseAsync(param, dbUser, player, enemy, status);
                    return;
                }
                player.Update();
                if (player.IsDead)
                {
                    await FightLooseAsync(param, dbUser, player, enemy, status);
                    return;
                }
                enemy.Update();
                if (enemy.IsDead)
                {
                    await FightWinAsync(param, dbUser, player, enemy, status);
                    return;
                }
                status.Hod++;
            }
        }
        public async Task PlayerHodAsync(InteractionParameters param, IPlayer player, IEnemy enemy, IFightStatus status)
        {
            var embed = FightEmbed(player, enemy, status);
            var components = new ComponentBuilder()
                    .WithButton("Лёгкая атака", "lightAttack", ButtonStyle.Danger, row: 0)
                    .WithButton("Тяжёлая атака", "heavyAttack", ButtonStyle.Danger, row: 0)
                    .WithButton("Уворот", "dodge", ButtonStyle.Success, row: 1)
                    .WithButton("Защита", "defense", ButtonStyle.Success, row: 1)
                    .WithButton(player.Spell.Info.Name, "spell", ButtonStyle.Primary, disabled: !player.Spell.CanCast(player),row: 2)
                    .WithButton(player.Skill.Info.Name, "skill", ButtonStyle.Primary, disabled: !player.Skill.CanUse, row: 2)
                    .Build();
            await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
            await param.Interaction!.DeferAsync();
            param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
            status.ClearDescriptions();
            if (param.Interaction == null)
            {
                _isAfk = true;
                PlayerAutoHod(param, player, enemy, status);
                return;
            }
            switch (param.Interaction.Data.CustomId)
            {
                case "lightAttack":
                    status.AddDescription(player.DealLightDamage(enemy).Description);
                    break;
                case "heavyAttack":
                    status.AddDescription(player.DealHeavyDamage(enemy).Description);
                    break;
                case "dodge":
                    status.AddDescription(player.PreparingForDodge(enemy).Description);
                    break;
                case "defense":
                    status.AddDescription(player.PreparingForDefense(enemy).Description);
                    break;
                case "spell":
                    status.AddDescription(player.CastSpell(enemy).Description);
                    break;
                case "skill":
                    status.AddDescription(player.UseSkill(enemy).Description);
                    break;
                default:
                    break;
            }
        }
        public void PlayerAutoHod(InteractionParameters param, IPlayer player, IEnemy enemy, IFightStatus status)
        {
            status.ClearDescriptions();
            status.AddDescription(player.RandomAction(enemy).Description);
        }
        public void EnemyHod(IPlayer player, IEnemy enemy, IFightStatus status)
        {
            status.AddDescription(enemy.RandomAction(player).Description);
        }
        public async Task FightWinAsync(InteractionParameters param, IDatabaseUser dbUser, IPlayer player, IEnemy enemy, IFightStatus status)
        {
            var drop = enemy.Drop.GetRandom(dbUser.Inventory);
            dbUser.Inventory.AddItems(drop);
            dbUser.Health.ReduceHealth(dbUser.Health.GetCurrentHealth() - player.Health.Now);
            dbUser.Mana.ReduceMana(dbUser.Mana.GetCurrentMana() - player.Mana.Now);
            dbUser.Statistics.AddEnemyKilled(1);
            status.AddDescription(dbUser.Rating.AddPoints(enemy.RatingGet, enemy.Rank).Description);
            dbUser.Statistics.UpdateMaxRating(dbUser.Rating.Points);
            status.AddDescription("\n__Победа!__\n**Вы получили:**");
            status.AddDescription(drop.GetDropInfo());
            await _database.Users.UpdateUser(
                dbUser.Id, 
                new UpdateDefinitionBuilder<UserData>()
                .Set("Inventory", dbUser.Inventory.GetInvetoryCache())
                .Set("HealthRegenTime", dbUser.Health.RegenTime)
                .Set("ManaRegenTime", dbUser.Mana.RegenTime)
                .Set("Rating", dbUser.Rating.Points)
                .Set("MaxRating", dbUser.Statistics.MaxRating)
                .Inc("EnemyKilled", 1));
            var embed = FightEmbed(player, enemy, status);
            embed = embed.ToEmbedBuilder()
                .WithAuthor($"{embed.Author} | Победа")
                .WithColor(Color.Green)
                .Build();
            await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = new ComponentBuilder().Build(); });
        }
        public async Task FightLooseAsync(InteractionParameters param, IDatabaseUser dbUser, IPlayer player, IEnemy enemy, IFightStatus status)
        {
            var embed = FightEmbed(player, enemy, status);
            embed = embed.ToEmbedBuilder()
                .WithAuthor($"{embed.Author} | Поражение")
                .WithColor(Color.Red)
                .Build();
            await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = new ComponentBuilder().Build(); });
            await _database.Users.KillUser(dbUser.Id);
            await param.Message.Channel.SendMessageAsync(embed: UserDeadEmbedBuilder.Simple(enemy.Name));
        }
        public async Task FightDrawAsync(InteractionParameters param, IDatabaseUser dbUser, IPlayer player, IEnemy enemy, IFightStatus status)
        {
            dbUser.Health.ReduceHealth(dbUser.Health.GetCurrentHealth() - player.Health.Now);
            dbUser.Mana.ReduceMana(dbUser.Mana.GetCurrentMana() - player.Mana.Now);
            status.AddDescription("\nВремя на сражение истекло. Ничья.");
            await _database.Users.UpdateUser(
                dbUser.Id,
                new UpdateDefinitionBuilder<UserData>()
                .Set("Inventory", dbUser.Inventory.GetInvetoryCache())
                .Set("HealthRegenTime", dbUser.Health.RegenTime)
                .Set("ManaRegenTime", dbUser.Mana.RegenTime));
            var embed = FightEmbed(player, enemy, status);
            embed = embed.ToEmbedBuilder()
                .WithAuthor($"{embed.Author} | Ничья")
                .WithColor(Color.LightGrey)
                .Build();
            await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = new ComponentBuilder().Build(); });
        }
        public Embed FightEmbed(IPlayer player, IEnemy enemy, IFightStatus status)
        {
            return new FightEmbedBuilder(Context.User.GetAvatarUrl(), Context.User.Username, enemy.Name)
                .WithAuthor($"Охота | Ход №{status.Hod}")
                .WithDescription(status.GetDescriptions())
                .AddField(player.Name, $"{((UserCharacteristics)player.Characteristics).ToRusString(player.EffectBonuses)}\n" +
                $"{player.Mana.ToString()}\n{player.Health.ToString()}", true)
                .AddField("Дополнительно:", $"**Заклинание:** {player.Spell.Info.Name}\n{player.Skill.GetStatus(player)}", true)
                .AddField("** - - - - - - - - - - - - - - - - - - - - - - - - - - - - -**", "** **", false)
                .AddField(enemy.Name, $"{((UserCharacteristics)enemy.Characteristics).ToRusString(enemy.EffectBonuses)}\n" +
                $"{enemy.Mana.ToString()}\n{enemy.Health.ToString()}", true)
                .AddField("Дополнительно:", $"**Заклинание:** {player.Spell.Info.Name}", true)
                .Build();
        }
    }
}
