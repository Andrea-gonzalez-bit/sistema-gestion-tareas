namespace ApiGenerico.Domain.Constants
{
    public static class DefaultStates
    {
        public const int PendingId = 1;
        public const int InProgressId = 2;
        public const int CompletedId = 3;

        public static readonly DateTime SeedDateUtc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
