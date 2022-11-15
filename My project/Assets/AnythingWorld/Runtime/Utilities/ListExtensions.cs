using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Utilities
{
    public static class ListExtensions
    {
        public static List<T> AddItem<T>(this List<T> self, T item)
        {
            self ??= new List<T>();
            self.Add(item);
            return self;
        }

        public static void DoAction<T>(this List<T> self, Action<T> func)
        {
            if (self == null) return;
            foreach (var variable in self)
            {
                func(variable);
            }
        }
    }
}

