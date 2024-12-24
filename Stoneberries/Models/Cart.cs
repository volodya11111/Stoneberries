using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoneberries
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        public User User { get; set; }
    }
}
