namespace BookShop
{
    using Data;
    using System;
    using Initializer;
    using System.Linq;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //DbInitializer.ResetDatabase(db);

                //Console.WriteLine(GetBooksByAgeRestriction(db, Console.ReadLine()));
                //Console.WriteLine(GetGoldenBooks(db));
                //Console.WriteLine(GetBooksByPrice(db));
                //Console.WriteLine(GetBooksNotReleasedIn(db, int.Parse(Console.ReadLine())));
                //Console.WriteLine(GetBooksByCategory(db, Console.ReadLine()));
                //Console.WriteLine(GetBooksReleasedBefore(db, Console.ReadLine()));
                //Console.WriteLine(GetAuthorNamesEndingIn(db, Console.ReadLine()));
                //Console.WriteLine(GetBookTitlesContaining(db, Console.ReadLine()));
                //Console.WriteLine(GetBooksByAuthor(db, Console.ReadLine()));
                //Console.WriteLine(CountBooks(db, int.Parse(Console.ReadLine())));
                //Console.WriteLine(CountCopiesByAuthor(db));
                //Console.WriteLine(GetTotalProfitByCategory(db));
                //Console.WriteLine(GetMostRecentBooks(db));
                //IncreasePrices(db);
                //Console.WriteLine(RemoveBooks(db));
            }
        }

        //1. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var bookTitles = context
                .Books
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //2. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            var bookTitles = context
                .Books
                .Where(b => b.EditionType.ToString() == "Gold")
                .Where(b => b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //3. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new
                {
                    b.Title,
                    b.Price,
                })
                .ToList();

            return String.Join(Environment.NewLine, books.Select(b => $"{b.Title} - ${b.Price:f2}"));
        }

        //4. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var bookTitles = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //5. Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var bookTitles = context
                .Books
                .Where(b => b.BookCategories
                             .Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //6. Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var books = context
                .Books
                .Where(b => b.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price,
                })
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}"));
        }

        //7. Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context
                .Authors
                .Where(a => a.FirstName.EndsWith(input.ToLower()))
                .Select(a => a.FirstName + ' ' + a.LastName)
                .OrderBy(a => a)
                .ToList();

            return string.Join(Environment.NewLine, authors);
        }

        //8. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var bookTitles = context
                .Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //9. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context
                .Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(a => new
                {
                    Title = a.Title,
                    Author = a.Author.FirstName + ' ' + a.Author.LastName
                })
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => $"{b.Title} ({b.Author})"));
        }

        //10. Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksCount = context
                .Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();

            return booksCount;
        }

        //11. Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context
                  .Authors
                  .Select(a => new
                  {
                      Author = a.FirstName + " " + a.LastName,
                      Copies = a.Books
                                .Select(b => b.Copies)
                                .Sum()
                  })
                  .OrderByDescending(a => a.Copies)
                  .ToList();

            return string.Join(Environment.NewLine, authors.Select(a => $"{a.Author} - {a.Copies}"));
        }

        //12. Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context
                .Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Proffit = c.CategoryBooks
                                .Select(cb => cb.Book.Price * cb.Book.Copies)
                                .Sum()
                })
                .OrderByDescending(c => c.Proffit)
                .ThenBy(c => c.CategoryName)
                .ToList();

            return string.Join(Environment.NewLine, categories.Select(c => $"{c.CategoryName} ${c.Proffit:f2}"));
        }

        //13. Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context
                .Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Books = c.CategoryBooks
                             .OrderByDescending(cb => cb.Book.ReleaseDate)
                             .Take(3)
                             .Select(cb => new
                             {
                                 BookTitle = cb.Book.Title,
                                 BookYear = cb.Book.ReleaseDate.Value.Year
                             })
                             .ToList()
                })
                .OrderBy(c => c.CategoryName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var categorie in categories)
            {
                sb.AppendLine($"--{categorie.CategoryName}");

                categorie.Books.ForEach(b => sb.AppendLine($"{b.BookTitle} ({b.BookYear})"));
            }

            return sb.ToString().TrimEnd();
        }

        //14. Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //15. Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.Copies < 4200)
                .ToList();

            var booksCount = books.Count();

            context.Books.RemoveRange(books);
            context.SaveChanges();

            return booksCount;
        }
    }
}
