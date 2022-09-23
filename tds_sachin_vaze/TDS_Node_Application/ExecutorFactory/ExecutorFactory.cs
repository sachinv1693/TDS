using TDS_Node_Application.TaskExecution;

namespace TDS_Node_Application.ExecutionFactory
{
    public class ExecutorFactory
    {
		public static ITaskExecutor GetTaskExector(string fileExtension)
		{
            return fileExtension switch
            {
                ".exe" => new SimpleExeTaskExecutor(),
                ".py" => new PythonTaskExecutor(),
                ".cs" => new CSharpTaskExecutor(),
                _ => null,
            };
        }
	}
}
