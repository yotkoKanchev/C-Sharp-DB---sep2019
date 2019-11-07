namespace P03_FootballBetting.Data
{
    public static class DataValidations
    {

        public static class Color
        {
            public const int NameMaxLength = 20;
        }

        public static class Country
        {
            public const int NameMaxLength = 30;
        }

        public static class Player
        {
            public const int NameMaxLength = 100;
        }

        public static class Position
        {
            public const int NameMaxLength = 20;
        }

        public static class Team
        {
            public const int NameMaxLength = 50;
            public const int LogoUrlMaxLength = 1000;
            public const int InitialsUrlMaxLength = 5;
        }

        public static class Town
        {
            public const int NameMaxLength = 50;
        }

        public static class User
        {
            public const int NameMaxLength = 100;
            public const int EmailMaxLength = 100;
            public const int PasswordMaxLength = 30;
            public const int UsernameMaxLength = 30;
        }
    }
}
