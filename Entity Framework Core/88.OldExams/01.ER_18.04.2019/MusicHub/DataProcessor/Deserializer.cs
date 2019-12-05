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
    using AutoMapper;
    using Data;
    using MusicHub.Data.Models;
    using MusicHub.Data.Models.Enums;
    using MusicHub.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

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
            var importWriterDtos = JsonConvert.DeserializeObject<ImportWriterDto[]>(jsonString);

            var writers = new List<Writer>();

            foreach (var writerDto in importWriterDtos)
            {
                if (!IsValid(writerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                sb.AppendLine(String.Format(SuccessfullyImportedWriter, writerDto.Name));

                writers.Add(new Writer
                {
                    Name = writerDto.Name,
                    Pseudonym = writerDto.Pseudonym,
                });
            }

            context.Writers.AddRange(writers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProducersAlbums(MusicHubDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var importProducerDtos = JsonConvert.DeserializeObject<ImportProducerDto[]>(jsonString);

            var producers = new List<Producer>();

            foreach (var producerDto in importProducerDtos)
            {
                if (!IsValid(producerDto) || !producerDto.Albums.All(IsValid))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var producer = new Producer
                {
                    Name = producerDto.Name,
                    PhoneNumber = producerDto.PhoneNumber,
                    Pseudonym = producerDto.Pseudonym,
                    Albums = producerDto.Albums.Select(a => new Album
                    {
                        Name = a.Name,
                        ReleaseDate = DateTime.ParseExact(a.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                    }).ToArray()
                };

                if (producer.PhoneNumber == null)
                {
                    sb.AppendLine(String.Format(SuccessfullyImportedProducerWithNoPhone, producerDto.Name, producerDto.Albums.Length));
                }
                else
                {
                    sb.AppendLine(String.Format(SuccessfullyImportedProducerWithPhone, producerDto.Name, producerDto.PhoneNumber, producerDto.Albums.Length));
                }

                producers.Add(producer);
            }

            context.Producers.AddRange(producers);
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
            var sb = new StringBuilder();
            var validSongIds = context.Songs.Select(s => s.Id).ToList();

            var serializer = new XmlSerializer(typeof(ImportPerformerDto[]), new XmlRootAttribute("Performers"));

            ImportPerformerDto[] importPerformerDtos;

            using (var reader = new StringReader(xmlString))
            {
                importPerformerDtos = (ImportPerformerDto[])serializer.Deserialize(reader);
            }

            var performers = new List<Performer>();

            foreach (var performerDto in importPerformerDtos)
            {
                var firstNameIsValid = performerDto.FirstName.Length >= 3 && performerDto.FirstName.Length <= 30;
                var lasstNameIsValid = performerDto.LastName.Length >= 3 && performerDto.LastName.Length <= 30;
                var ageIsValid = performerDto.Age >= 17 && performerDto.Age <= 70;
                var netWorthIsValid = performerDto.NetWorth >= 0;
                var songIdsAreValid = performerDto.Songs.All(s => validSongIds.Contains(s.SongId));

                if (!firstNameIsValid || !lasstNameIsValid || !ageIsValid || !netWorthIsValid || !songIdsAreValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                sb.AppendLine(string.Format(SuccessfullyImportedPerformer, performerDto.FirstName, performerDto.Songs.Length));

                var performer = Mapper.Map<Performer>(performerDto);

                foreach (var songDto in performerDto.Songs)
                {

                    var songPerformer = new SongPerformer
                    {
                        Performer = performer,
                        SongId = songDto.SongId,
                    };
                    performer.PerformerSongs.Add(songPerformer);
                    //songPerformers.Add(songPerformer);
                }

                performers.Add(performer);
            }

            context.Performers.AddRange(performers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResults, true);
        }
    }
}