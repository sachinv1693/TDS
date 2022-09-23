using NUnit.Framework;
using System.Collections.Generic;
using System.Data.SqlClient;
using TDS_Coordinator_Application.TaskCoordinator.DB;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;
using TDS_Coordinator_Application.TaskCoordinator.DB.Enums;
using TDS_Coordinator_Application.TaskCoordinator.DB.RepoFactory;

namespace TDS_Test
{
    public class TestNodeRepo
    {
        private IDatabaseManager<SqlDataReader> _databaseManager;
        private dynamic _dao;

        [SetUp]
        public void Setup()
        {
            _databaseManager = new SqlDatabaseManager();
            _dao = RepoFactory.GetRepository(ReopsitoryType.NODE, _databaseManager);
        }

        [Test]
        public void Insert_Node_Succeeds()
        {
            var node = new NodeData() //Arrange
            {
                NodeIpAddress = "191.158.1.36",
                NodePort = "8001",
                HostName = "Executor03"
            };

            _dao.Add(node); //Act
            
            Assert.Pass(); //Assert
        }

        [Test]
        public void Get_All_Available_Nodes_Succeeds()
        {
            List<NodeData> _ = _dao.GetAllAvailableNodes(); //Act

            Assert.Pass(); //Assert
        }

        [Test]
        public void Get_All_Nodes_Succeeds()
        {
            List<NodeData> _ = _dao.GetAllNodes(); //Act

            Assert.Pass(); //Assert
        }

        [Test]
        public void Delete_Node_By_Id_Succeeds()
        {
            _dao.DeleteById(2003); //Act

            Assert.Pass(); //Assert
        }
    }
}