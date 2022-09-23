using NUnit.Framework;
using System.Collections.Generic;
using System.Data.SqlClient;
using TDS_Coordinator_Application.TaskCoordinator.DB;
using TDS_Coordinator_Application.TaskCoordinator.DB.Entities;
using TDS_Coordinator_Application.TaskCoordinator.DB.Enums;
using TDS_Coordinator_Application.TaskCoordinator.DB.RepoFactory;

namespace TDS_Test
{
    public class TestClientRepo
    {
        private IDatabaseManager<SqlDataReader> _databaseManager;
        private dynamic _dao;

        [SetUp]
        public void Setup()
        {
            _databaseManager = new SqlDatabaseManager();
            _dao = RepoFactory.GetRepository(ReopsitoryType.CLIENT, _databaseManager);
        }

        [Test]
        public void Insert_Client_Succeeds()
        {
            var client = new ClientData() //Arrange
            {
                ClientIpAddress = "193.154.2.30",
                ClientPort = "8085",
                HostName = "Test PC Id Insertion"
            };
            
            _dao.Add(client); //Act

            Assert.Pass(); //Assert
        }

        [Test]
        public void Duplicate__Client_Insertion_Fails()
        {
            var client = new ClientData() //Arrange
            {
                ClientIpAddress = "193.154.2.30",
                ClientPort = "8085",
                HostName = "Test PC Id Insertion"
            };

            int id = _dao.Add(client); //Act

            Assert.AreEqual(id, -1); //Assert
        }

        [Test]
        public void Delete_Client_By_Id_Succeeds()
        {
            _dao.DeleteById(10); //Act

            Assert.Pass(); //Assert
        }

        [Test]
        public void Get_Client_By_Id_Succeeds()
        {
            ClientData client = _dao.GetClientById(1); //Act
            
            Assert.AreEqual("Sachin's PC", client.HostName); //Assert
        }

        [Test]
        public void Get_All_Clients_Succeeds()
        {
            List<ClientData> _ = _dao.GetAllClients(); //Act

            Assert.Pass(); //Assert
        }
    }
}