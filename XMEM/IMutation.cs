using System;
using Newtonsoft.Json.Linq;

namespace Xemo
{
	public interface IMutation
	{
		JProperty Command();
	}
}

