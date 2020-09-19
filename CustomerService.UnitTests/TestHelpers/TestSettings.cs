namespace CustomerService.UnitTests
{
	/// <summary>
	/// Test configuration settings
	/// </summary>
	public static class TestSettings
	{
		public static string EventuateDB { get; set; } = "Data Source=db;Initial Catalog=eventuate;User Id=sa;Password=App@Passw0rd";
		public static string KafkaBootstrapServers { get; set; } =  "kafka:29092";
		public static string EventuateTramDbSchema { get; set; } = "eventuate";
	}

}