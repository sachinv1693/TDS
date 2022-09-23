# TDS Application (Sachin Vaze)

It's a Console Application built using the best Object Oriented Programming practices

## Project Overview

* Quick picks 
    - Multiple client machines can send tasks (some programs or executable files) to the TDS server.
       - I have implemented C# (".cs" file), Python (".py" file) and simple ".exe" file execution
    - TDS Coordinator grabs the incoming tasks arriving onto TDS Server and queues them into queue.
    - It then distributes them to execute on available node machines. Node machines are pre-registered.
    - Client can query for the task result and status by using respective task GUID.
    - Request-Response model is in JSON format
* Version-01
* [Learn & Code Batch-1 (Trainers: Rakesh Sawan & Madhavi Latha)]

## How do I get set up?

* Run the TDS Coordinator and Node applications using respective binaries and try various command requests from client processes.
* Dependencies - .NET 3.1
* Database configuration
     - Currently, the app is using SQL database. Its connection string and DB provider details are stored in App.Config
     - Code is developed using **"open for extension, closed for modification"** principle.
     - So, one can easily change the DB if required in future.
* How to run tests - Try unit tests from TDS_Test project
* Deployment instructions - Using git version control (In Time Tec BitBucket account) 

## Contribution guidelines

* Writing tests
    - Basic unit tests for DB are completed.
    - Basic unit test for protocol object serialization-deserialization completed.
* Code review - Regular team meetings and getting Pull Requests reviewed from Rakesh or Madhavi
* Other guidelines - Following **clean coding techniques**

## Who do I talk to?

* L&C Batch-1 Leaders - Rakesh Sawan or Madhavi Latha
* Other community or team contact
    - Zoom groups
	    - My team : TDS Implementation
    	- All Leads : L&C Team Leads - New