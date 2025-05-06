using System.Collections.Generic;
using System.Linq;
#if IL2CPP
using Il2CppScheduleOne.Product;
#else
using ScheduleOne.Product;
#endif

namespace JustEnoughDrugs.Models
{
    public class DrugSorter
    {

        public enum SortOrder { Asc, Desc }
        public enum SorterType { Newest, Addictiveness, Cost, Price, Profit }

        public static List<ProductEntry> SortDrugs(List<ProductEntry> drugs, SorterType sorterType, SortOrder sortOrder)
        {
            switch (sorterType)
            {
                case SorterType.Addictiveness:
                    return SortByAddictiveness(drugs, sortOrder);
                case SorterType.Cost:
                    return SortByCost(drugs, sortOrder);
                case SorterType.Price:
                    return SortByPrice(drugs, sortOrder);
                case SorterType.Profit:
                    return SortByProfit(drugs, sortOrder);
                case SorterType.Newest:
                    return SortByNewest(drugs, sortOrder);
                default:
                    return drugs;
            }
        }

        private static List<ProductEntry> SortByAddictiveness(List<ProductEntry> drugs, SortOrder sortOrder)
        {
            return sortOrder == SortOrder.Asc ?
                drugs.OrderBy(d => d.Definition.GetAddictiveness()).ToList() :
                drugs.OrderByDescending(d => d.Definition.GetAddictiveness()).ToList();
        }

        private static List<ProductEntry> SortByCost(List<ProductEntry> drugs, SortOrder sortOrder)
        {

            return sortOrder == SortOrder.Asc ?
        drugs.OrderBy(d =>
        {
            MainMod.ProductCosts.TryGetValue(d.Definition, out var cost);
            return cost;
        }).ToList() :
        drugs.OrderByDescending(d =>
        {
            MainMod.ProductCosts.TryGetValue(d.Definition, out var cost);
            return cost;
        }).ToList();
        }
        private static List<ProductEntry> SortByPrice(List<ProductEntry> drugs, SortOrder sortOrder)
        {
            return sortOrder == SortOrder.Asc ?
                drugs.OrderBy(d => d.Definition.Price).ToList() :
                drugs.OrderByDescending(d => d.Definition.Price).ToList();
        }
        private static List<ProductEntry> SortByProfit(List<ProductEntry> drugs, SortOrder sortOrder)
        {
            return sortOrder == SortOrder.Asc ?
                drugs.OrderBy(d =>
                {

                    MainMod.ProductCosts.TryGetValue(d.Definition, out var cost);
                    return d.Definition.Price - cost;
                }).ToList() :
                drugs.OrderByDescending(d =>
                {
                    MainMod.ProductCosts.TryGetValue(d.Definition, out var cost);
                    return d.Definition.Price - cost;
                }).ToList();
        }

        private static List<ProductEntry> SortByNewest(List<ProductEntry> drugs, SortOrder sortOrder)
        {
            var allProducts = ProductManager.Instance.AllProducts;
            return sortOrder == SortOrder.Asc
                ? drugs.OrderBy(d => allProducts.IndexOf(d.Definition)).ToList()
                : drugs.OrderByDescending(d => allProducts.IndexOf(d.Definition)).ToList();
        }
    }
}