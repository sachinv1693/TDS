namespace TDS_Coordinator_Application.TaskCoordinator.DB
{
	public interface IDatabaseManager<TDataReader>
	{
		void CloseConnection();
		TDataReader ExecuteSelectQuery(string query);
		void ExecuteNonQuery(string query);
	}
}
