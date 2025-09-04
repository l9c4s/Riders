using System;

namespace Domain.Utils.Mappers
{
    public static class MapperUtils
    {
        public static TDestination Map<TSource, TDestination>(TSource source)
            where TDestination : new()
        {
            if (source == null) return default;

            var destination = new TDestination();
            var sourceProps = typeof(TSource).GetProperties();
            var destProps = typeof(TDestination).GetProperties();

            foreach (var sourceProp in sourceProps)
            {
                var destProp = destProps.FirstOrDefault(p => p.Name == sourceProp.Name && p.PropertyType == sourceProp.PropertyType);
                if (destProp != null && destProp.CanWrite)
                {
                    destProp.SetValue(destination, sourceProp.GetValue(source));
                }
            }

            return destination;
        }
        
        public static List<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> sourceList)
            where TDestination : new()
        {
            if (sourceList == null) return null;

            var destinationList = new List<TDestination>();
            foreach (var source in sourceList)
            {
                destinationList.Add(Map<TSource, TDestination>(source));
            }
            return destinationList;
        }

    }

}

