namespace Cinema.DataProcessor
{
    using System;
    using System.Linq;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    
    using Newtonsoft.Json;

    using ExportDto;
    using Data;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies
                .Where(m => m.Rating >= rating)
                .Where(m => m.Projections.Any(p => p.Tickets.Any()))
                .Take(10)
                .OrderByDescending(m => m.Rating)
                .ThenByDescending(m => m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
                .Select(m => new
                {
                    MovieName = m.Title,
                    Rating = m.Rating.ToString("F2"),
                    TotalIncomes = m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)).ToString("F2"),
                    Customers = m.Projections
                        .SelectMany(p => p.Tickets)
                        .Select(t => new
                        {
                            FirstName = t.Customer.FirstName,
                            LastName = t.Customer.LastName,
                            Balance = t.Customer.Balance.ToString("F2"),
                        })
                        .OrderByDescending(c => c.Balance)
                        .ThenBy(c => c.FirstName)
                        .ThenBy(c => c.LastName)
                        .ToArray()
                })
                .ToArray();

            return JsonConvert.SerializeObject(movies, Newtonsoft.Json.Formatting.Indented);
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var customers = context.Customers
                .Where(c => c.Age >= age)
                .OrderByDescending(c => c.Tickets.Sum(t => t.Price))
                .Take(10)
                .Select(c => new ExportCustomerDto
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    SpentMoney = c.Tickets.Sum(t => t.Price).ToString("F2"),
                    SpentTime = TimeSpan.FromMilliseconds(c.Tickets
                        .Sum(t => t.Projection.Movie.Duration.TotalMilliseconds))
                        .ToString("c", CultureInfo.InvariantCulture),
                })
                .ToArray();


            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ExportCustomerDto[]), new XmlRootAttribute("Customers"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, customers, ns);
            }

            return sb.ToString().TrimEnd();
        }
    }
}