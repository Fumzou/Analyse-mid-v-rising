namespace KindredExtract.Models;

public class EcsSystemCounts
{
	public int Group;

	public int Base;

	public int Unmanaged;

	public int Unknown;

	public int NotUsed;

	public int SumUsed()
	{
		return Group + Base + Unmanaged + Unknown;
	}
}
