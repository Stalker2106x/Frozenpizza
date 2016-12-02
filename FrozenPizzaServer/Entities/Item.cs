using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizzaServer
{

    public class Item
    {
        public int Id { get; set; }
        public Item(int id)
        {
            Id = id;
        }
    }
}
