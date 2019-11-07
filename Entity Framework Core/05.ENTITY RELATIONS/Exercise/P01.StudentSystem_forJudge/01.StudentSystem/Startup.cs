namespace P01_StudentSystem
{
    using Microsoft.EntityFrameworkCore;
    using P01_StudentSystem.Data;
    using System;

    public class Startup
    {
        public static void Main()
        {
            using (var db = new StudentSystemContext())
            {
                db.Database.Migrate();

                db.SaveChanges();
            }
        }
    }
}
