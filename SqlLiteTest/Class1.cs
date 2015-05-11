using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using NUnit.Framework;

namespace SqlLiteTest
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void Test()
        {
            SetUp();

            var connection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;foreign keys=true;");
            connection.Open();


            ExecuteNonQuery(connection, "begin transaction");

            ExecuteNonQuery(connection, "insert into Account (Name, PreferredName, Forenames, Surname, Email) values ('thomasd', 'Dan', 'Dan', 'Thomas', 'danthomas000@gmail.com')");
            ExecuteNonQuery(connection, "insert into Account (Name, PreferredName, Forenames, Surname, Email) values ('fawcetts', 'Ste', 'Steven', 'Fawcett', 'fawcetts@gmail.com')");

            ExecuteNonQuery(connection, "insert into Programme (Code, Name) values ('CROSS', 'CrossFit')");
            ExecuteNonQuery(connection, "insert into Programme (Code, Name) values ('COMPETE', 'CrossFit Compete')");

            ExecuteNonQuery(connection, "insert into AccountProgramme (AccountId, ProgrammeId) values (1, 1)");


            ExecuteNonQuery(connection, "commit transaction");

            connection.Close();

            connection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;foreign keys=true;");
            connection.Open();

            IDataReader reader = ExecuteReader(connection, @"
select *
from Programme");

            while (reader.Read())
            {
                var id = reader.GetByte(0);
                var code = reader.GetValue(1) == DBNull.Value ? null :  reader.GetString(1);
                var name = reader.GetValue(2) == DBNull.Value ? null : reader.GetString(2);
            }
        }

        private static void SetUp()
        {
            SQLiteConnection.CreateFile("MyDatabase.sqlite");
            var connection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            connection.Open();

            ExecuteNonQuery(connection, @"create table Account
(
  Id integer primary key autoincrement
, Name varchar(30) not null unique
, PreferredName varchar(30) not null
, Forenames varchar(30) not null
, Surname varchar(30) not null
, Email varchar(200) unique
)");

            ExecuteNonQuery(connection, @"create table Programme
(
  Id integer primary key autoincrement
, Code varchar(10) not null
, Name varchar(30) not null
)");

            ExecuteNonQuery(connection, @"create table AccountProgramme
(
  Id integer primary key autoincrement
, AccountId int not null
, ProgrammeId int not null
, foreign key (AccountId) references Account(Id)
, foreign key (ProgrammeId) references Programme(Id)
)");
        }

        private static void ExecuteNonQuery(SQLiteConnection connection, string sql)
        {
            SQLiteCommand command = new SQLiteCommand(sql, connection);

            command.ExecuteNonQuery();
        }

        private static IDataReader ExecuteReader(SQLiteConnection connection, string sql)
        {
            SQLiteCommand command = new SQLiteCommand(sql, connection);

            return command.ExecuteReader();
        }
    }
}
