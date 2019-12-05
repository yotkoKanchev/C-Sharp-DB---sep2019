namespace MusicHub.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using ImportDtos;

    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data";

        private const string SuccessfullyImportedWriter
            = "Imported {0}";
        private const string SuccessfullyImportedProducerWithPhone
            = "Imported {0} with phone: {1} produces {2} albums";
        private const string SuccessfullyImportedProducerWithNoPhone
            = "Imported {0} with no phone number produces {1} albums";
        private const string SuccessfullyImportedSong
            = "Imported {0} ({1} genre) with duration {2}";
        private const string SuccessfullyImportedPerformer
            = "Imported {0} ({1} songs)";

        public static string ImportWriters(MusicHubDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var dtos = JsonConvert.DeserializeObject<ImportWriterDto[]>(jsonString);

            var results = new List<Writer>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                sb.AppendLine(String.Format(SuccessfullyImportedWriter, dto.Name));

                results.Add(new Writer
                {
                    Name = dto.Name,
                    Pseudonym = dto.Pseudonym,
                });
            }

            context.Writers.AddRange(results);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProducersAlbums(MusicHubDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var dtos = JsonConvert.DeserializeObject<ImportProducerDto[]>(jsonString);

            var resultList = new List<Producer>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto) || !dto.Albums.All(IsValid))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var result = new Producer
                {
                    Name = dto.Name,
                    PhoneNumber = dto.PhoneNumber,
                    Pseudonym = dto.Pseudonym,
                    Albums = dto.Albums.Select(a => new Album
                    {
                        Name = a.Name,
                        ReleaseDate = DateTime.ParseExact(a.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                    }).ToArray()
                };

                if (result.PhoneNumber == null)
                {
                    sb.AppendLine(String.Format(SuccessfullyImportedProducerWithNoPhone, dto.Name, dto.Albums.Length));
                }
                else
                {
                    sb.AppendLine(String.Format(SuccessfullyImportedProducerWithPhone, dto.Name, dto.PhoneNumber, dto.Albums.Length));
                }

                resultList.Add(result);
            }

            context.Producers.AddRange(resultList);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSongs(MusicHubDbContext context, string xmlString)
        {
            var albumIds = context.Albums.Select(a => a.Id).ToList();
            var writerIds = context.Writers.Select(w => w.Id).ToList();

            var songs = new List<Song>();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ImportSongDto[]), new XmlRootAttribute("Songs"));

            ImportSongDto[] importSongDtos;

            using (var reader = new StringReader(xmlString))
            {
                importSongDtos = (ImportSongDto[])serializer.Deserialize(reader);
            }

            foreach (var dto in importSongDtos)
            {
                var albumIdIsValid = albumIds.Contains(dto.AlbumId.Value) || dto.AlbumId == null;
                var writerIdIsValid = writerIds.Contains(dto.WriterId);
                var genreIsValid = Enum.IsDefined(typeof(Genre), dto.Genre);

                if (!IsValid(dto) || !albumIdIsValid || !writerIdIsValid || !genreIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var song = new Song
                {
                    Name = dto.Name,
                    WriterId = dto.WriterId,
                    AlbumId = dto.AlbumId,
                    Genre = Enum.Parse<Genre>(dto.Genre),
                    CreatedOn = DateTime.ParseExact(dto.CreatedOn, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Duration = TimeSpan.ParseExact(dto.Duration, "c", CultureInfo.InvariantCulture),
                    Price = dto.Price
                };

                sb.AppendLine(String.Format(SuccessfullyImportedSong, dto.Name, dto.Genre, dto.Duration));
                songs.Add(song);
            }

            context.Songs.AddRange(songs);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSongPerformers(MusicHubDbContext context, string xmlString)
        {
            var performers = new List<Performer>();
            var songs = context.Songs.ToList();
            var songIds = songs.Select(s => s.Id).ToList();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ImportPerformerDto[]), new XmlRootAttribute("Performers"));

            ImportPerformerDto[] importPerformerDtos;

            using (var reader = new StringReader(xmlString))
            {
                importPerformerDtos = (ImportPerformerDto[])serializer.Deserialize(reader);
            }

            foreach (var dto in importPerformerDtos)
            {
                var songsAreValid = dto.PerformersSongs.All(i => songIds.Contains(i.Id)) && 
                                    dto.PerformersSongs.All(IsValid) ;

                if (!IsValid(dto) || !songsAreValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var performer = new Performer
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Age = dto.Age,
                    NetWorth = dto.NetWorth,
                    PerformerSongs = dto.PerformersSongs
                        .Select(ps => new SongPerformer
                        {
                            SongId = ps.Id
                        })
                        .ToArray()
                };

                sb.AppendLine(string.Format(SuccessfullyImportedPerformer, performer.FirstName, performer.PerformerSongs.Count));
                performers.Add(performer);
            }

            context.Performers.AddRange(performers);
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