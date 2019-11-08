namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //DbInitializer.ResetDatabase(db);

                //var result = GetBooksByAgeRestriction(db, Console.ReadLine());
                //var result = GetGoldenBooks(db);
                //var result = GetBooksByPrice(db);
                //var result = GetBooksNotReleasedIn(db, int.Parse(Console.ReadLine()));
                //var result = GetBooksByCategory(db, Console.ReadLine());
                //var result = GetBooksReleasedBefore(db, Console.ReadLine());
                //var result = GetAuthorNamesEndingIn(db, Console.ReadLine());
                //var result = GetBookTitlesContaining(db, Console.ReadLine());
                //var result = GetBooksByAuthor(db, Console.ReadLine());
                //var result = CountBooks(db, int.Parse(Console.ReadLine()));
                //var result = CountCopiesByAuthor(db);
                //var result = GetTotalProfitByCategory(db);
                var result = GetMostRecentBooks(db);
                //IncreasePrices(db);
                //var result = RemoveBooks(db);

                Console.WriteLine(result);
            }
        }

        //1. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var bookTitles = context.Books
                                    .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                                    .OrderBy(b => b.Title)
                                    .Select(t => t.Title)
                                    .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //2. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            var bookTitles = context.Books
                                    .Where(b => b.EditionType.ToString() == "Gold" && b.Copies < 5000)
                                    .OrderBy(b => b.BookId)
                                    .Select(b => b.Title)
                                    .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //3. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                               .Where(b => b.Price > 40)
                               .OrderByDescending(b => b.Price)
                               .Select(b => new
                               {
                                   b.Title,
                                   b.Price,
                               })
                               .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //4. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var bookTitles = context.Books
                               .Where(b => b.ReleaseDate.Value.Year != year)
                               .OrderBy(b => b.BookId)
                               .Select(b => b.Title)
                               .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //5. Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.ToLower()
                                  .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var bookTitles = context.Books
                                    .Where(b => b.BookCategories
                                                 .Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                                    .Select(b => b.Title)
                                    .OrderBy(b => b)
                                    .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //6. Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var dateFormat = "dd-MM-yyyy";
            var dateAsDateTime = DateTime.ParseExact(date, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
            var books = context.Books
                               .Where(b => b.ReleaseDate < dateAsDateTime)
                               .OrderByDescending(b => b.ReleaseDate)
                               .Select(b => new
                               {
                                   Title = b.Title,
                                   EditionType = b.EditionType.ToString(),
                                   Price = b.Price,
                               })
                               .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //7. Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authorNames = context.Authors
                                     .Where(b => b.FirstName.EndsWith(input))
                                     .Select(b => new
                                     {
                                         FullName = b.FirstName + " " + b.LastName
                                     })
                                     .OrderBy(a => a.FullName)
                                     .ToList();


            var sb = new StringBuilder();

            authorNames.ForEach(n => sb.AppendLine(n.FullName));
            return sb.ToString().TrimEnd();
        }

        //8. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var bookTitles = context.Books
                                    .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                                    .OrderBy(b => b.Title)
                                    .Select(b => b.Title)
                                    .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //9. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                                    .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                                    .OrderBy(b => b.BookId)
                                    .Select(b => new
                                    {
                                        Title = b.Title,
                                        Author = b.Author.FirstName + ' ' + b.Author.LastName,
                                    })
                                    .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.Author})");
            }

            return sb.ToString().TrimEnd();
        }

        //10. Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksCount = context.Books
                                    .Where(b => b.Title.Length > lengthCheck)
                                    .Count();

            return booksCount;
        }

        //11. Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                                 .Select(a => new
                                 {
                                     Author = a.FirstName + " " + a.LastName,
                                     BooksCount = a.Books
                                                   .Select(b => b.Copies).Sum()
                                 })
                                 .OrderByDescending(a => a.BooksCount)
                                 .ToList();

            var sb = new StringBuilder();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.Author} - {author.BooksCount}");
            }

            return sb.ToString().TrimEnd();
        }

        //12. Profit by Category TODO 
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                                    .Select(c => new
                                    {
                                        Category = c.Name,
                                        TotalProfit = c.CategoryBooks
                                                       .Select(cb => cb.Book.Price * cb.Book.Copies)
                                                       .Sum()
                                    })
                                    .OrderByDescending(c => c.TotalProfit)
                                    .ThenBy(c => c.Category)
                                    .ToList();

            var sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"{category.Category} ${category.TotalProfit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //13. Most Recent Books TODO 
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                                    .Select(c => new
                                    {
                                        Category = c.Name,
                                        Books = c.CategoryBooks
                                                 .OrderByDescending(b => b.Book.ReleaseDate)
                                                 .Take(3)
                                                 .Select(b => new
                                                 {
                                                     Title = b.Book.Title,
                                                     Year = b.Book.ReleaseDate.Value.Year,
                                                 })
                                                 .ToList()
                                    })
                                    .OrderBy(c => c.Category)
                                    .ToList();

            var sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.Category}");

                foreach (var book in category.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //14. Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
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
            var booksToRemove = context.Books
                                       .Where(b => b.Copies < 4200)
                                       .ToList();

            context.Books.RemoveRange(booksToRemove);
            context.SaveChanges();

            return booksToRemove.Count;
        }
    }
}

