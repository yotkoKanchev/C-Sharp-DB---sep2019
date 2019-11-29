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
            var writers = Mapper.Map<Writer[]>(importWriterDtos);
            var validWriters = new List<Writer>();

            foreach (var writer in writers)
            {
                if (!IsValid(writer) == true)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                validWriters.Add(writer);
                sb.AppendLine(string.Format(SuccessfullyImportedWriter, writer.Name));
            }

            context.Writers.AddRange(validWriters);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProducersAlbums(MusicHubDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var importProducersDtos = JsonConvert.DeserializeObject<ImportProducerDto[]>(jsonString);

            var producers = new List<Producer>();

            foreach (var producerDto in importProducersDtos)
            {
                var producer = Mapper.Map<Producer>(producerDto);

                var producerIsValid = IsValid(producer);
                var albumsAreValid = producerDto.ProducerAlbums.All(a => IsValid(a));

                if (!producerIsValid || !albumsAreValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (producerDto.PhoneNumber == null)
                {
                    sb.AppendLine(String.Format(SuccessfullyImportedProducerWithNoPhone,
                                                    producerDto.Name,
                                                    producerDto.ProducerAlbums.Count));
                }
                else
                {
                    sb.AppendLine(String.Format(SuccessfullyImportedProducerWithPhone,
                                                    producerDto.Name, producerDto.PhoneNumber,
                                                    producerDto.ProducerAlbums.Count));
                }

                var albums = Mapper.Map<List<Album>>(producerDto.ProducerAlbums);
                albums.ForEach(a => producer.Albums.Add(a));

                producers.Add(producer);
            }

            context.Producers.AddRange(producers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSongs(MusicHubDbContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var validWriterIds = context.Writers.Select(w => w.Id).ToList();
            var validAlbumIds = context.Albums.Select(a => a.Id).ToList();

            var serializer = new XmlSerializer(typeof(ImportSongDto[]), new XmlRootAttribute("Songs"));

            ImportSongDto[] importSongDtos;

            using (var reader = new StringReader(xmlString))
            {
                importSongDtos = (ImportSongDto[])serializer.Deserialize(reader);
            }

            var songs = new List<Song>();

            foreach (var songDto in importSongDtos)
            {
                var nameIsValid = songDto.Name.Length >= 3 && songDto.Name.Length <= 20;
                var priceIsValid = songDto.Price > 0;
                var genreIsValid = Enum.IsDefined(typeof(Genre), songDto.Genre);
                var writerIdIsValid = validWriterIds.Contains(songDto.WriterId);
                var albumIdIsValid = songDto.AlbumId != null ? validAlbumIds.Contains(songDto.AlbumId.Value) : true;

                if (!nameIsValid || !priceIsValid || !genreIsValid || !writerIdIsValid || !albumIdIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var song = Mapper.Map<Song>(songDto);

                songs.Add(song);
                sb.AppendLine(string.Format(SuccessfullyImportedSong, song.Name, song.Genre.ToString(), song.Duration));
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