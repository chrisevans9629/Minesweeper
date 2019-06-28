using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class ItemList : List<Item>, ICloneable
    {
        public ItemList(IList<Item> items) : base(items)
        {
            
        }
        public ItemList()
        {
            
        }
        public ItemList GetItemsBaseOnvalue(int containerSize)
        {
            var sorted = this.ToList().OrderByDescending(p => p.Value).ThenBy(p => p.Size).ToList();
            return GetItemsThatFit(containerSize, sorted);
        }

        ItemList GetItemsThatFit(int container, IList<Item> items)
        {
            var list = new ItemList();
            int size = 0;
            foreach (var item in items)
            {
                if (size + item.Size <= container)
                {
                    list.Add(item);
                    size += item.Size;
                }
            }

            return list;
        }

        public object Clone()
        {
            return new ItemList(this.Select(p=>p.Clone() as Item).ToList());
        }
    }
}