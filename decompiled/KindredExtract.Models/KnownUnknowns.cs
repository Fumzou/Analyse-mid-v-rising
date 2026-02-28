using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

namespace KindredExtract.Models;

public class KnownUnknowns
{
	public ISet<SystemHandle> SystemNotFoundInWorld = new HashSet<SystemHandle>();

	public bool AreKnown()
	{
		return SystemNotFoundInWorld.Any();
	}
}
