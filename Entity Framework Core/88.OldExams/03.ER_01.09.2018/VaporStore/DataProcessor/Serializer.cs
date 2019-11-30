namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.ExportDtos;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var exportGenreDtos = context.Genres
                .Where(g => genreNames.Contains(g.Name))
                .OrderByDescending(g => g.Games.Sum(gm => gm.Purchases.Count))
                .ThenBy(g => g.Id)
                .Select(g => new ExportGenreDto
                {
                    Id = g.Id,
                    Genre = g.Name,
                    Games = g.Games
                    .Where(x => x.Purchases.Any())
                    .Select(x => new ExportGameDto
                    {
                        Id = x.Id,
                        Developer = x.Developer.Name,
                        Title = x.Name,
                        Tags = string.Join(", ", x.GameTags.Select(z => z.Tag.Name).ToArray()),
                        Players = x.Purchases.Count

                    })
                    .OrderByDescending(y => y.Players)
                    .ThenBy(y => y.Id)
                    .ToArray(),
                    TotalPlayers = g.Games.Sum(z => z.Purchases.Count)
                });

            return JsonConvert.SerializeObject(exportGenreDtos, Formatting.Indented);
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            return null;
        }
    }
}