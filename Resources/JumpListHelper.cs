using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization.Collation;
using Symphonia;

public static class JumpListHelper
{

    public static List<JumpListGroup<TSource>> ToGroups<TSource,TSort,TGroup>(
        this IEnumerable<TSource> source, Func<TSource, TSort> sortSelector,
            Func<TSource, TGroup> groupSelector, bool isSortDescending = false)
    {
        {
            var groups = new List<JumpListGroup<TSource>>();

            // Group and sort items based on values returned from the selectors
            var query = from item in source
                        orderby groupSelector(item), sortSelector(item)
                        group item by groupSelector(item) into g
                        select new { GroupName = g.Key, Items = g };

            // For each group generated from the query, create a JumpListGroup
            // and fill it with its items
            foreach (var g in query)
            {
                JumpListGroup<TSource> group = new JumpListGroup<TSource>();
                group.Key = g.GroupName;
                foreach (var item in g.Items)
                    group.Add(item);

                if (isSortDescending)
                    groups.Insert(0, group);
                else
                    groups.Add(group);
            }

            return groups;
        }
    }
    
    public static List<JumpListGroup<TSource>> ToAlphaGroups<TSource>(
            this IEnumerable<TSource> source, Func<TSource, string> selector)
    {
        var characterGroupings = new CharacterGroupings();
        var keys = characterGroupings.Where(x => x.Label.Count() >= 1)
                        .Select(x => x.Label)
                        .ToDictionary(x => x);
        keys["..."] = "\uD83C\uDF10";
        var groupDictionary = keys.Select(x => new JumpListGroup<TSource>() { Key = x.Value })
                       .ToDictionary(x => (string)x.Key);
        var query = from item in source
                    orderby selector(item)
                    select item;

        foreach (var item in query)
        {
            var sortValue = selector(item);
            groupDictionary[keys[characterGroupings.Lookup(sortValue)]].Add(item);
        }

        return groupDictionary.Select(x => x.Value).ToList();
    }
}

