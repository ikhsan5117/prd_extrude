using System.Linq;
using System.Reflection;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Helpers
{
    public static class SpsMapper
    {
        public static void CopyProperties<TSource, TTarget>(TSource source, TTarget target)
        {
            var sourceProps = typeof(TSource).GetProperties().Where(x => x.CanRead).ToList();
            var targetProps = typeof(TTarget).GetProperties().Where(x => x.CanWrite).ToDictionary(x => x.Name);

            foreach (var sourceProp in sourceProps)
            {
                if (targetProps.TryGetValue(sourceProp.Name, out var targetProp))
                {
                    // Skip Id to avoid identity insert issues when not needed, but here we let it copy if we want.
                    // Let's explicitly allow copying if type matches.
                    if (targetProp.PropertyType == sourceProp.PropertyType)
                    {
                        var value = sourceProp.GetValue(source);
                        targetProp.SetValue(target, value);
                    }
                }
            }
        }

        public static void CopyPropertiesForUpdate<TSource, TTarget>(TSource source, TTarget target)
        {
            var sourceProps = typeof(TSource).GetProperties().Where(x => x.CanRead).ToList();
            var targetProps = typeof(TTarget).GetProperties().Where(x => x.CanWrite).ToDictionary(x => x.Name);

            foreach (var sourceProp in sourceProps)
            {
                // SKIP: DocumentNumber (PK), No, ItemLists (navigation property)
                if (sourceProp.Name == "DocumentNumber" || sourceProp.Name == "No" || sourceProp.Name == "ItemLists")
                    continue;

                if (targetProps.TryGetValue(sourceProp.Name, out var targetProp))
                {
                    if (targetProp.PropertyType == sourceProp.PropertyType)
                    {
                        var value = sourceProp.GetValue(source);
                        targetProp.SetValue(target, value);
                    }
                }
            }
        }

        public static SpsMaster ToSpsMaster(SpsNoDoc source, string itemList)
        {
            var target = new SpsMaster();
            CopyProperties(source, target);
            target.ItemList = itemList;
            return target;
        }

        public static SpsMaster ToSpsMaster(SpsNoDoc source)
        {
            var target = new SpsMaster();
            CopyProperties(source, target);
            return target;
        }

        public static SpsNoDoc ToSpsNoDoc(SpsMaster source)
        {
            var target = new SpsNoDoc();
            CopyProperties(source, target);
            // Don't copy Id because Create will generate new. If Edit, we handle Id separately.
            return target;
        }

        public static void UpdateSpsNoDoc(SpsNoDoc target, SpsMaster source)
        {
            CopyPropertiesForUpdate(source, target);
        }
    }
}
