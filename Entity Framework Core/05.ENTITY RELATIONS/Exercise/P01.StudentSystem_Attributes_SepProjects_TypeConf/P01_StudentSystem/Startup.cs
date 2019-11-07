namespace P01_StudentSystem
{
    using Microsoft.EntityFrameworkCore;
    using P01_StudentSystem.Data;

    public class Startup
    {
        public static void Main(string[] args)
        {
            using (var db = new StudentSystemContext())
            {
                db.Database.Migrate();

                db.SaveChanges();
            }
        }
    }
}
