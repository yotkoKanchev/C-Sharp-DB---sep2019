namespace Cinema
{
    using System;
    using System.Linq;
    using System.Globalization;
    using Data.Models;
    using DataProcessor.ImportDto;
    using Data.Models.Enums;
    using Cinema.DataProcessor.ExportDto;
    using AutoMapper;

    public class CinemaProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
        public CinemaProfile()
        {
            //deserializations
            this.CreateMap<ImportMovieDto, Movie>()
                .ForMember(x => x.Genre, y => y.MapFrom(x => Enum.Parse(typeof(Genre), x.Genre)))
                .ForMember(x => x.Duration, y => y.MapFrom(x => TimeSpan
                    .ParseExact(x.Duration, @"hh\:mm\:ss", CultureInfo.InvariantCulture)));

            this.CreateMap<ImportHallDto, Hall>();

            this.CreateMap<ImportProjectionDto, Projection>()
                .ForMember(x => x.DateTime, y => y.MapFrom(x => DateTime
                    .ParseExact(x.DateTime, @"yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)));

            this.CreateMap<ImportTicketDto, Ticket>();
            this.CreateMap<ImportCustomerTicketDto, Customer>();

            //serializations

            //USING AUTOMAPPER RETURNS THE RIGHT RESULT BUT JUDGE FINDS DIFFERENCE ON 87TH ROW :(
            //this.CreateMap<Customer, ExportCustomerDto>()
            //    .ForMember(x => x.Balance, y => y.MapFrom(x => x.Balance.ToString("F2")));

            //this.CreateMap<Movie, ExportMovieDto>()
            //    .ForMember(x => x.MovieName, y => y.MapFrom(x => x.Title))
            //    .ForMember(x => x.Rating, y => y.MapFrom(x => x.Rating.ToString("F2")))
            //    .ForMember(x => x.TotalIncomes, y => y.MapFrom(x => x.Projections
            //        .Sum(p => p.Tickets
            //            .Sum(t => t.Price))
            //            .ToString("F2")))
            //    .ForMember(x => x.Customers, y => y.MapFrom(x => x.Projections
            //        .SelectMany(p => p.Tickets)
            //        .Select(z => z.Customer)
            //            .OrderByDescending(c => c.Balance)
            //            .ThenBy(c => c.FirstName)
            //            .ThenBy(c => c.LastName)));

            this.CreateMap<Customer, ExportTopCustomerDto>()
                .ForMember(x => x.SpentMoney, y => y.MapFrom(x => x.Tickets.Sum(t => t.Price).ToString("F2")))
                .ForMember(x => x.SpentTime, y => y.MapFrom(x => TimeSpan.FromMilliseconds(x.Tickets
                    .Sum(t => t.Projection.Movie.Duration.TotalMilliseconds))
                    .ToString(@"hh\:mm\:ss")));
        }
    }
}
