namespace SoftJail.DataProcessor
{

    using Data;
    using System;

    public class Bonus
    {
        public static string ReleasePrisoner(SoftJailDbContext context, int prisonerId)
        {
            var prisoner = context.Prisoners
                .Find(prisonerId);
            var releaseDate = prisoner.ReleaseDate;
            prisoner.ReleaseDate = DateTime.UtcNow;
            prisoner.CellId = null;

            var result = $"Prisoner {prisoner.FullName} released";

            if (releaseDate == null)
            {
                result = $"Prisoner { prisoner.FullName} is sentenced to life";
            }

            context.SaveChanges();

            return result;
        }
    }
}
