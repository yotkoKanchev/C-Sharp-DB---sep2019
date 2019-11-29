namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using ImportDto;
    using Data;
    using Data.Models;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using AutoMapper;
    using System.Linq;
    using Cinema.Data.Models.Enums;
    using System.Xml.Serialization;
    using System.IO;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var importMovieDtos = JsonConvert.DeserializeObject<ImportMovieDto[]>(jsonString);
            var movies = new List<Movie>();

            foreach (var movieDto in importMovieDtos)
            {
                var genreIsValid = Enum.IsDefined(typeof(Genre), movieDto.Genre);
                var titleIsNotValid = movies.Any(m => m.Title == movieDto.Title);

                if (!genreIsValid || titleIsNotValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var movie = Mapper.Map<Movie>(movieDto);

                var movieIsValid = IsValid(movie);

                if (!movieIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                sb.AppendLine(String.Format(SuccessfulImportMovie, movie.Title, movie.Genre, movie.Rating.ToString("f2")));
                movies.Add(movie);
            }

            context.Movies.AddRange(movies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var importHallDtos = JsonConvert.DeserializeObject<ImportHallDto[]>(jsonString);
            var halls = new List<Hall>();

            foreach (var hallDto in importHallDtos)
            {
                var seatCountIsValid = hallDto.SeatsCount > 0;

                if (!seatCountIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var hall = Mapper.Map<Hall>(hallDto);

                var hallIsValid = IsValid(hall);

                if (!hallIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var projectionType = "Normal";

                if (hall.Is3D == true && hall.Is4Dx == true)
                {
                    projectionType = "4Dx/3D";
                }
                else if (hall.Is3D == true && hall.Is4Dx == false)
                {
                    projectionType = "3D";
                }
                else if (hall.Is3D == false && hall.Is4Dx == true)
                {
                    projectionType = "4Dx";
                }

                sb.AppendLine(string.Format(SuccessfulImportHallSeat, hall.Name, projectionType, hallDto.SeatsCount));

                for (int i = 0; i < hallDto.SeatsCount; i++)
                {
                    hall.Seats.Add(new Seat
                    {
                        Hall = hall
                    });

                }

                halls.Add(hall);
            }

            context.Halls.AddRange(halls);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var validHallIds = context.Halls.Select(h => h.Id).ToList();
            var validMovieIds = context.Movies.Select(h => h.Id).ToList();

            var serializer = new XmlSerializer(typeof(ImportProjectionDto[]), new XmlRootAttribute("Projections"));

            ImportProjectionDto[] importProjectionDtos;

            using (var reader = new StringReader(xmlString))
            {
                importProjectionDtos = (ImportProjectionDto[])serializer.Deserialize(reader);
            }

            var projections = new List<Projection>();

            foreach (var projectionDto in importProjectionDtos)
            {
                var movieIdIsValid = validMovieIds.Contains(projectionDto.MovieId);
                var hallIdIsValid = validHallIds.Contains(projectionDto.HallId);

                if (!movieIdIsValid || !hallIdIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var projection = Mapper.Map<Projection>(projectionDto);

                var movieTitle = context.Movies.Find(projection.MovieId).Title;
                var projectionTime = projection.DateTime.ToString("MM/dd/yyyy");

                sb.AppendLine(string.Format(SuccessfulImportProjection, movieTitle, projectionTime));
                projections.Add(projection);
            }

            context.Projections.AddRange(projections);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ImportCustomerTicketDto[]), new XmlRootAttribute("Customers"));

            ImportCustomerTicketDto[] importCustomerTicketDtos;

            using (var reader = new StringReader(xmlString))
            {
                importCustomerTicketDtos = (ImportCustomerTicketDto[])serializer.Deserialize(reader);
            }

            var customers = new List<Customer>();

            foreach (var customerDto in importCustomerTicketDtos)
            {
                var customer = Mapper.Map<Customer>(customerDto);

                if (!IsValid(customer))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                sb.AppendLine(string.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName, customer.Tickets.Count));
                customers.Add(customer);
            }

            context.Customers.AddRange(customers);
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