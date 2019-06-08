using Gadget.Widgets;
using System.Collections.Generic;

namespace Gadget.Extensions
{
    public static class ListExtensions
    {
        public static void SortByPosition(this List<IWidget> list)
        {
            list.Sort((item1, item2) => item1.Position.CompareTo(item2.Position));
        }
    }
}
