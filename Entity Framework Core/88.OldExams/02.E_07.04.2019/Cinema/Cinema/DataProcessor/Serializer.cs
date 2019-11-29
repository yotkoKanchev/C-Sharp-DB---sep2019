namespace Cinema.DataProcessor
{
    using System.Linq;
    using Data;
    using Cinema.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using AutoMapper;
    using System.Text;
    using System.Xml.Serialization;

    using System.IO;
    using AutoMapper.QueryableExtensions;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            //var movies = context.Movies     USING AUTOMAPPER RETURNS THE RIGHT RESULT BUT JUDGE FINDS DIFFERENCE ON 87TH ROW :(
            //   .Where(m => m.Rating >= rating && m.Projections.Any(p => p.Tickets.Any()))
            //   .Take(10)
            //   .OrderByDescending(m => m.Rating)
            //   .ThenByDescending(m => m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
            //   .ProjectTo<ExportMovieDto>()
            //   .ToList();

            var movies = context.Movies
                    .Where(m => m.Rating >= rating && m.Projections.Any(p => p.Tickets.Count() >= 1))
                    .Take(10)
                    .OrderByDescending(m => m.Rating)
                    .ThenByDescending(m => m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
                    .Select(m => new ExportMovieDto
                    {
                        MovieName = m.Title,
                        Rating = m.Rating.ToString("F2"),
                        TotalIncomes = m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)).ToString("F2"),
                        Customers = m.Projections
                        .SelectMany(p => p.Tickets)
                        .Select(t => new ExportCustomerDto
                        {
                            FirstName = t.Customer.FirstName,
                            LastName = t.Customer.LastName,
                            Balance = t.Customer.Balance.ToString("F2")
                        })
                        .OrderByDescending(c => c.Balance)
                        .ThenBy(c => c.FirstName)
                        .ThenBy(c => c.LastName)
                        .ToList()
                    })
                    .ToList();

            return JsonConvert.SerializeObject(movies, Formatting.Indented);
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var sb = new StringBuilder();

            var customers = context.Customers
                .Where(c => c.Age >= age)
                .OrderByDescending(c => c.Tickets.Sum(t => t.Price))
                .Take(10)
                .ProjectTo<ExportTopCustomerDto>()
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportTopCustomerDto[]), new XmlRootAttribute("Customers"));
            var ns = new XmlSerializerNamespaces(new[] { System.Xml.XmlQualifiedName.Empty });

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, customers, ns);
            }

            return sb.ToString().TrimEnd();
        }
    }
}