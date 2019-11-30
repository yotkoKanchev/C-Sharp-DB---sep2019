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
            var sb = new StringBuilder();

            var exportUserDtos = context.Users
                .Select(u => new ExportUserDto
                {
                    Username = u.Username,
                    Purchases = u.Cards.SelectMany(c => c.Purchases)
                    .Where(c => c.Type.ToString() == storeType)
                    .Select(c => new ExportPurchaseDto
                    {
                        Cvc = c.Card.Cvc,
                        Date = c.Date.ToString(@"yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Card = c.Card.Number,
                        Game = new GameDto
                        {
                            Title = c.Game.Name,
                            Genre = c.Game.Genre.Name,
                            Price = c.Game.Price
                        }
                    })
                    .OrderBy(p => p.Date)
                    .ToArray(),
                    TotalSpent = u.Cards.SelectMany(c => c.Purchases)
                    .Where(p => p.Type.ToString() == storeType)
                    .Sum(p => p.Game.Price)
                })
                .Where(p => p.Purchases.Any())
                .OrderByDescending(u => u.TotalSpent)
                .ThenBy(u => u.Username)
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportUserDto[]), new XmlRootAttribute("Users"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, exportUserDtos, ns);
            }

            return sb.ToString().TrimEnd();
        }
    }
}