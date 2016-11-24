using System;
using System.Collections.Generic;

namespace FrozenPizza
{
	public class Beverage : Consumable
	{
		public Beverage()
		{
		}

		public override bool Load(String name)
		{
			return (true);
		}
	}

	public class Food : Consumable
	{
		public Food()
		{
		}

		public override bool Load(String name)
		{
			return (true);
		}
	}
}
