namespace VaporStore.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using VaporStore.Data.Models.Enumerations;
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

            return JsonConvert.SerializeObject(exportGenreDtos, Newtonsoft.Json.Formatting.Indented);
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
                                        // DOESN'T WORK IN Judge !!!
            var sb = new StringBuilder();
            var purchaseType = Enum.Parse<PurchaseType>(storeType);

            var users = context
                .Users
                .Select(x => new UserExportDto
                {
                    Username = x.Username,
                    Purchases = x.Cards
                        .SelectMany(p => p.Purchases)
                        .Where(t => t.Type == purchaseType)
                        .Select(p => new PurchaseExportDto
                        {
                            Card = p.Card.Number,
                            Cvc = p.Card.Cvc,
                            Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                            Game = new GameExportDto
                            {
                                Genre = p.Game.Genre.Name,
                                Title = p.Game.Name,
                                Price = p.Game.Price,
                            }
                        })
                        .OrderBy(d => d.Date)
                        .ToArray(),
                    TotalSpent = x.Cards
                        .SelectMany(p => p.Purchases)
                        .Where(t => t.Type == purchaseType)
                        .Sum(p => p.Game.Price)
                })
                .Where(p => p.Purchases.Any())
                .OrderByDescending(t => t.TotalSpent)
                .ThenBy(u => u.Username)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(UserExportDto[]), new XmlRootAttribute("Users"));

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}