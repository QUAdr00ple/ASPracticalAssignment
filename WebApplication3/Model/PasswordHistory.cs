using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Model
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string HashedPassword { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PasswordHistoryService
    {
        private readonly AuthDbContext db;

        public PasswordHistoryService(AuthDbContext db)
        {
            this.db = db;
        }

        public async Task<List<string>> GetRecentPasswordsAsync(ApplicationUser user)
        {
            var userId = user.Id;
            var recentPasswords = await db.PasswordHistories
                .Where(ph => ph.UserId == userId)
                .OrderByDescending(ph => ph.CreatedAt)
                .Take(2)
                .Select(ph => ph.HashedPassword)
                .ToListAsync();

            return recentPasswords;
        }

        public async Task<DateTime?> GetLastPasswordChangeDateAsync(ApplicationUser user)
        {
            var userId = user.Id;
            var lastPasswordChange = await db.PasswordHistories
                .Where(ph => ph.UserId == userId)
                .OrderByDescending(ph => ph.CreatedAt)
                .Select(ph => ph.CreatedAt)
                .FirstOrDefaultAsync();

            return lastPasswordChange;
        }

    }
}
