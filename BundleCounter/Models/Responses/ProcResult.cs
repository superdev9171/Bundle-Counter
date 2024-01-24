namespace BundleCounter.Models.Responses
{
	public class ProcResult<T>
	{
		public bool Success { get; set; }
		public int ErrorCode { get; set; }
		public string? ErrorMessage { get; set; }
		public T? Data { get; set; }
	}
}
