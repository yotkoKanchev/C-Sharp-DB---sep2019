namespace MusicHub
{
    using AutoMapper;
    using MusicHub.Data.Models;
    using MusicHub.Data.Models.Enums;
    using MusicHub.DataProcessor.ExportDtos;
    using MusicHub.DataProcessor.ImportDtos;
    using System;
    using System.Globalization;
    using System.Linq;

    public class MusicHubProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
        public MusicHubProfile()
        {
            //imports 
            this.CreateMap<ImportWriterDto, Writer>();

            this.CreateMap<ImportProducerDto, Producer>();
            this.CreateMap<ImportAlbumDto, Album>()
                .ForMember(x => x.ReleaseDate, y => y.MapFrom(
                    x => DateTime.ParseExact(x.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));

            this.CreateMap<ImportSongDto, Song>()
                .ForMember(x => x.Duration, y => y.MapFrom(
                    x => TimeSpan.ParseExact(x.Duration, @"hh\:mm\:ss", CultureInfo.InvariantCulture)))
                .ForMember(x => x.CreatedOn, y => y.MapFrom(
                    x => DateTime.ParseExact(x.CreatedOn, @"dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(x => x.Genre, y => y.MapFrom(
                    x => Enum.Parse(typeof(Genre), x.Genre)));

            this.CreateMap<ImportSongIdDto, Song>();
            this.CreateMap<ImportSongPerformerDto, SongPerformer>();
            this.CreateMap<ImportPerformerDto, Performer>();

            //exports
            this.CreateMap<Song, ExportSongDto>()
                .ForMember(x => x.SongName, y => y.MapFrom(x => x.Name))
                .ForMember(x => x.Price, y => y.MapFrom(x => $"{x.Price:f2}"))
                .ForMember(x => x.Writer, y => y.MapFrom(x => x.Writer.Name));
            this.CreateMap<Album, ExportAlbumDto>()
                .ForMember(x => x.AlbumName, y => y.MapFrom(x => x.Name))
                .ForMember(x => x.ReleaseDate, y => y.MapFrom(x => x.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(x => x.ProducerName, y => y.MapFrom(x => x.Producer.Name))
                .ForMember(x => x.Songs, y => y.MapFrom(x => x.Songs.OrderByDescending(s => s.Name).ThenBy(s => s.Writer.Name)))
                .ForMember(x => x.AlbumPrice, y => y.MapFrom(x => Math.Round(x.Songs.Sum(s => s.Price), 2)));

            this.CreateMap<Song, ExportSongAboveDurationDto>()
                .ForMember(x => x.SongName, y => y.MapFrom(x => x.Name))
                .ForMember(x => x.Writer, y => y.MapFrom(x => x.Writer.Name))
                .ForMember(x => x.Performer, y => y.MapFrom(x => x.SongPerformers
                                                                .Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                                                                .FirstOrDefault()))
                .ForMember(x => x.AlbumProducer, y => y.MapFrom(x => x.Album.Producer.Name))
                .ForMember(x => x.Duration, y => y.MapFrom(x => x.Duration.ToString(@"hh\:mm\:ss")));
        }
    }
}
