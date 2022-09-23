using NUnit.Framework;
using System.Collections.Generic;
using System.Data.SqlClient;
using TDS_Coordinator_Application.TaskCoordinator.DB;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;
using TDS_Coordinator_Application.TaskCoordinator.DB.Enums;
using TDS_Coordinator_Application.TaskCoordinator.DB.RepoFactory;

namespace TDS_Test
{
    public class TestTaskRepo
    {
        private IDatabaseManager<SqlDataReader> _databaseManager;
        private dynamic _dao;

        [SetUp]
        public void Setup()
        {
            _databaseManager = new SqlDatabaseManager();
            _dao = RepoFactory.GetRepository(ReopsitoryType.TASK, _databaseManager);
        }

        [Test]
        public void Insert_Task_Succeeds()
        {
            var task = new TaskData() //Arrange
            {
                ClientId = 3,
                TaskPath = "C:\\P-101 Assignments\\python_list_dict_pass_by_reference.py"
            };

            _dao.Add(task); //Act

            Assert.Pass(); //Assert
        }

        [Test]
        public void Get_Task_By_Id_Succeeds()
        {
            TaskData task = _dao.GetTaskById(3); //Act

            Assert.AreEqual("C:\\P-101 Assignments\\python_basic_oop_problems.py", task.TaskPath); //Assert
        }

        [Test]
        public void Get_Task_By_Client_Id_Succeeds()
        {
            List<TaskData> _ = _dao.GetTasksByClientId(3); //Act

            Assert.Pass(); //Assert
        }

        [Test]
        public void Set_Task_Status_Succeeds()
        {
            _dao.SetTaskStatus(3, TaskStatus.EXECUTING); //Act

            Assert.Pass(); //Assert
        }

        [Test]
        public void Set_Task_Result_Succeeds()
        {
            _dao.SetTaskResult(3, "Hello World\n"); //Act

            Assert.Pass(); //Assert
        }

        [Test]
        public void Get_Tasks_By_Status_Succeeds()
        {
            List<TaskData> _ = _dao.GetTasksByStatus(TaskStatus.PENDING); //Act

            Assert.Pass(); //Assert
        }

        [Test]
        public void Delete_Task_By_Id_Succeeds()
        {
            _dao.DeleteById(12); //Act

            Assert.Pass(); //Assert
        }
    }
}