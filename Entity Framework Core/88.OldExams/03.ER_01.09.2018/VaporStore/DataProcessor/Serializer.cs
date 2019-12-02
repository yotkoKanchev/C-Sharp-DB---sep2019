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
        {                       // DOESN'T WORK IN Judge !!!
            var purchaseType = Enum.Parse<PurchaseType>(storeType);
            var sb = new StringBuilder();

            var purchases = context.Users
                .Where(u => u.Cards.SelectMany(c => c.Purchases).Any()) /* may not be the correct place ?!?*/
                .Select(u => new ExportUserDto
                {
                    Username = u.Username,
                    Purchases = u.Cards
                        .SelectMany(c => c.Purchases)
                        .Where(p => p.Type == purchaseType)
                        .Select(p => new ExportPurchaseDto
                        {
                            Card = p.Card.Number,
                            Cvc = p.Card.Cvc,
                            Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                            Game = new GameDto
                            {
                                Title = p.Game.Name,
                                Genre = p.Game.Genre.Name,
                                Price = p.Game.Price,
                            }
                        })
                        .OrderBy(p => p.Date)
                        .ToArray(),
                    TotalSpent = u.Cards
                        .SelectMany(c => c.Purchases)
                        .Where(p => p.Type == purchaseType)
                        .Sum(p => p.Game.Price)
                })
                //.Where(u => u.Purchases.Any())
                .OrderByDescending(u => u.TotalSpent)
                .ThenBy(u => u.Username)
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportUserDto[]), new XmlRootAttribute("Users"));

            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, purchases, ns);
            }

           return sb.ToString().TrimEnd();
        }
    }
}