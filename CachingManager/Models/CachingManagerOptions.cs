namespace CachingManager.Models
{
	public class CachingManagerOptions
	{
		public string ConnectionString { get; set; }
		public TimeSpan? ExpiresIn { get; set; }
	}
}