using System;
using System.Collections.Generic;

namespace ProjectDTS
{
    public static class BreadcrumbManager
    {
        private static readonly List<string> _crumbs = new List<string>();

        public static void Push(string crumb)
        {
            _crumbs.Add(crumb);
        }

        public static void Pop()
        {
            if (_crumbs.Count > 0)
            {
                _crumbs.RemoveAt(_crumbs.Count - 1);
            }
        }

    public static void Render()
    {
        if (_crumbs.Count == 0) return;

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("📍 ");
        
        for (int i = 0; i < _crumbs.Count; i++)
        {
            // De laatste kruimel (waar we nu zijn) geven we een fellere kleur
            if (i == _crumbs.Count - 1)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(_crumbs[i]);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(_crumbs[i]);
                
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" / ");
            }
        }
        
        Console.ResetColor();
        Console.WriteLine("\n" + new string('-', Console.WindowWidth - 1));
    }
}
}