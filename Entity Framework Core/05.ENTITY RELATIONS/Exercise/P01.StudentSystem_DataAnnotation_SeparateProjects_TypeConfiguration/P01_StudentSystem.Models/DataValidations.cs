namespace P01_StudentSystem.Data
{
    public static class DataValidations
    {
        public static class Student
        {
            public const int NameMaxLength = 100;
            public const int PhoneFixedLength = 10;
        }

        public static class Course
        {
            public const int NameMaxLength = 80;
            public const int DescriptionMaxLength = 2000;
        }

        public static class Resource
        {
            public const int NameMaxLength = 50;
            public const int UrlMaxLength = 2000;
        }

        public static class Homework
        {
            public const int ContentMaxLength = 2000;
        }
    }
}
