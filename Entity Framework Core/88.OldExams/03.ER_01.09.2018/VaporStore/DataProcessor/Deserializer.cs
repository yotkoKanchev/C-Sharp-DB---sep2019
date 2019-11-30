namespace VaporStore.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    using Data;
    using ImportDtos;
    using Data.Models;
    using Data.Models.Enumerations;

    using Newtonsoft.Json;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var games = new List<Game>();
            var developers = new HashSet<Developer>();
            var genres = new HashSet<Genre>();
            var tags = new HashSet<Tag>();

            var gamesDto = JsonConvert.DeserializeObject<ImportGameDto[]>(jsonString);

            foreach (var gameDto in gamesDto)
            {
                bool gameDtoIsValid = gameDto.Price >= 0 &&
                    !String.IsNullOrWhiteSpace(gameDto.Name) &&
                    !String.IsNullOrWhiteSpace(gameDto.Developer) &&
                    !String.IsNullOrWhiteSpace(gameDto.Genre) &&
                    !String.IsNullOrWhiteSpace(gameDto.ReleaseDate) &&
                    gameDto.Tags.Any();

                if (gameDtoIsValid == false)
                {                 // create constant               
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var developer = developers.Any(d => d.Name == gameDto.Developer)
                        ? developers.First(d => d.Name == gameDto.Developer)
                        : new Developer { Name = gameDto.Developer, };

                var genre = genres.Any(g => g.Name == gameDto.Genre)
                        ? genres.First(g => g.Name == gameDto.Genre)
                        : new Genre { Name = gameDto.Genre, };

                developers.Add(developer);
                genres.Add(genre);

                var currentTags = new List<Tag>();

                foreach (var tagName in gameDto.Tags)
                {
                    var tag = tags.Any(t => t.Name == tagName)
                        ? tags.First(t => t.Name == tagName)
                        : new Tag { Name = tagName };

                    currentTags.Add(tag);
                    tags.Add(tag);
                }

                var game = new Game
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = DateTime.ParseExact(
                        gameDto.ReleaseDate, @"yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Developer = developer,
                    Genre = genre,
                    GameTags = currentTags.Select(gt => new GameTag { Tag = gt }).ToList()
                };

                games.Add(game);
                //create constant here !!!
                sb.AppendLine($"Added {gameDto.Name} ({gameDto.Genre}) with {gameDto.Tags.Length} tags");
            }

            context.Games.AddRange(games);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var users = new List<User>();

            var importUserDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(jsonString);

            foreach (var userDto in importUserDtos)
            {
                var cardDtosAreValid = userDto.Cards.All(c => IsValid(c) && Enum.IsDefined(typeof(CardType), c.Type));
                var userDtoIsValid = IsValid(userDto);

                if (!cardDtosAreValid || !userDtoIsValid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var userCards = userDto.Cards
                    .Select(c => new Card
                    {
                        Number = c.Number,
                        Cvc = c.Cvc,
                        Type = (CardType)Enum.Parse(typeof(CardType), c.Type),
                    }
                    ).ToList();

                var user = new User
                {
                    FullName = userDto.FullName,
                    Username = userDto.Username,
                    Age = userDto.Age,
                    Email = userDto.Email,
                    Cards = userCards,
                };

                sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ImportPurchaseDto[]), new XmlRootAttribute("Purchases"));
            var purchases = new List<Purchase>();
            var validCards = context.Cards.ToList();
            var validGames = context.Games.ToList();

            var validGameNames = validGames.Select(g => g.Name).ToList();
            var validCardNumbers = validCards.Select(c => c.Number).ToList();

            ImportPurchaseDto[] importPurchaseDtos;

            using (var reader = new StringReader(xmlString))
            {
                importPurchaseDtos = (ImportPurchaseDto[])serializer.Deserialize(reader);
            }

            foreach (var purchaseDto in importPurchaseDtos)
            {
                var purchaseIsValid = Enum.IsDefined(typeof(PurchaseType), purchaseDto.Type) &&
                    validCardNumbers.Contains(purchaseDto.Card) &&
                    validGameNames.Contains(purchaseDto.Title) &&
                    IsValid(purchaseDto);

                if (!purchaseIsValid)
                {
                    sb.AppendLine("ImportPurchaseDto");
                    continue;
                }

                var card = validCards.First(c => c.Number == purchaseDto.Card);
                var game = validGames.Find(g => g.Name == purchaseDto.Title);

                var purchase = new Purchase
                {
                    Type = (PurchaseType)Enum.Parse(typeof(PurchaseType), purchaseDto.Type),
                    Date = DateTime.ParseExact(purchaseDto.Date, @"dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    ProductKey = purchaseDto.Key,
                    Card = card,
                    Game = game,
                };

                purchases.Add(purchase);

                var username = card.User.Username;

                sb.AppendLine($"Imported {game.Name} for {username}");
            }

            context.Purchases.AddRange(purchases);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}