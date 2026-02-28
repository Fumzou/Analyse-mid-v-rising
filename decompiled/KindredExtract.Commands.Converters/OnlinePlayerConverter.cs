using VampireCommandFramework;

namespace KindredExtract.Commands.Converters;

internal class OnlinePlayerConverter : CommandArgumentConverter<OnlinePlayer>
{
	public override OnlinePlayer Parse(ICommandContext ctx, string input)
	{
		return new OnlinePlayer(FoundPlayerConverter.HandleFindPlayerData(ctx, input, requireOnline: false));
	}
}
