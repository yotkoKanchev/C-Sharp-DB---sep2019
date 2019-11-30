namespace VaporStore.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Data;
    using ImportDtos;
    using Data.Models;

    using Newtonsoft.Json;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
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
                {
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

                sb.AppendLine($"Added {gameDto.Name} ({gameDto.Genre}) with {gameDto.Tags.Length} tags");
            }

            context.Games.AddRange(games);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            return null;
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            return null;
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}