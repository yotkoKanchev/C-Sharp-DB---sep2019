namespace MusicHub.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    //using System.Xml;
    using System.Xml.Serialization;
    using AutoMapper;
    using Data;
    using MusicHub.DataProcessor.ExportDtos;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums
                .Where(a => a.ProducerId == producerId)
                .OrderByDescending(a => a.Price)
                .ToList();

            var exportAlbumDtos = Mapper.Map<ExportAlbumDto[]>(albums)
                .ToArray();

            return JsonConvert.SerializeObject(exportAlbumDtos,
                                               Formatting.Indented,
                                               new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs
                .Where(s => s.Duration.TotalSeconds > duration)
                .ToArray();

            var songDtos = Mapper.Map<ExportSongAboveDurationDto[]>(songs)
                .OrderBy(sdt => sdt.SongName)
                .ThenBy(sdt => sdt.Writer)
                .ThenBy(sdt => sdt.Performer).ToArray();

            var serializer = new XmlSerializer(typeof(ExportSongAboveDurationDto[]), new XmlRootAttribute("Songs"));
            var ns = new XmlSerializerNamespaces(new[] { System.Xml.XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, songDtos, ns);
            }

            return sb.ToString().TrimEnd();
        }
    }
}