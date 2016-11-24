using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace FrozenPizza
{
	public class Collection
	{
		List<Melee> _melee;

		public Collection()
		{
			
		}

		public bool Load(ContentManager content)
		{
			LoadMelee(content);
			return (true);
		}

		public bool LoadMelee(ContentManager content)
		{
			_melee = content.Load<List<Melee>>("items/melee");
			return (true);
		}
	}
}
