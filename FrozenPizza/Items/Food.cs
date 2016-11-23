using System;
using System.Collections.Generic;

namespace FrozenPizza
{
	public class Drink : Consumable
	{
		public Drink(String name, List<SlotType> slots, int weight, int size) : base(name, slots, weight, size)
		{
		}

		public override bool Load(String name)
		{
			return (true);
		}
	}

	public class Food : Consumable
	{
		public Food(String name, List<SlotType> slots, int weight, int size) : base(name, slots, weight, size)
		{
		}

		public override bool Load(String name)
		{
			return (true);
		}
	}
}
