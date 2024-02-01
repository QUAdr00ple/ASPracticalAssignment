namespace WebApplication3.Model
{
	public class AuditLog
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string Action { get; set; }
		public DateTime Timestamp { get; set; }
	}

	public class AuditLogService
	{
		private readonly AuthDbContext _dbContext;

		public AuditLogService(AuthDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public void LogLogin(string userId)
		{
			LogAction(userId, "Login");
		}

		public void LogRegister(string userId)
		{
			LogAction(userId, "Register");
		}

		public void LogLogout(string userId)
		{
			LogAction(userId, "Logout");
		}

		public void LogLoginFailed(string userId)
		{
			LogAction(userId, "LoginFailed");
		}

		private void LogAction(string userId, string action)
		{
			var logEntry = new AuditLog
			{
				UserId = userId,
				Action = action,
				Timestamp = DateTime.UtcNow
			};

			_dbContext.AuditLogs.Add(logEntry);
			_dbContext.SaveChanges();
		}
	}

}
