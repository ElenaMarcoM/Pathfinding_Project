using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class DBManager : MonoBehaviour
{
    //Base de datos
    private string dbUri = "Uri=file:mydb.sqlite";

    // Crear tablas
    private string sql_createTable_Player = "CREATE TABLE IF NOT EXISTS Player " +
        "(Id INTEGER UNIQUE NOT NULL PRIMARY KEY, " +
        "Name TEXT, " +
        "GameMode TEXT DEFAULT 'Easy', " +
        "Nationality TEXT DEFAULT 'Spain', " +
        "Performance INTEGER REFERENCE Performance);";
    private string sql_createTable_Performance = "CREATE TABLE IF NOT EXISTS Performance " +
        "(Id INTEGER UNIQUE NOT NULL PRIMARY KEY, " +
        "Score INTEGER);";

    //DEFAULT SCORES
    private int[] scores_default = {2, 4, 6, 8, 10 };
    private string[] nombres_default = { "Daria", "Mercedes", "Ana", "Fernando", "Ramon", "Ben", "Raul", "Pedro", "Lucia", "Poncho" };
    private string[] nacionalidad_default = { "Spanish", "Brazilian", "German", "British", "French", "Italian", "Chinese", "Australian", "Japanese", "Ecuatorian" };
    private int NUM_PLAYERS = 100;

    // Start is called before the first frame update
    void Start()
    {
        // Creamos la base de datos
        IDbConnection dbConnection = CreateAndOpenDataBase();

        dbConnection.Close();
    }

    private IDbConnection CreateAndOpenDataBase()
    {
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();

        return dbConnection;
    }

    private void InitializeDB(IDbConnection dbConnection)
    {
        // Crear las tablas || Command para ejecutar sentencias de sql
        IDbCommand dbCmd = dbConnection.CreateCommand();
        dbCmd.CommandText = sql_createTable_Player + sql_createTable_Performance;
        dbCmd.ExecuteReader();
    }

    void AddDataToPlayer(IDbConnection dbConnection ,string Nombre, string Nacionalidad)
    {
        // Añadir elemento a Tabla
        string command = "INSERT INTO Player (Name, Nationality) VALUES ";
        command += $"({Nombre}, {Nacionalidad});";

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = command;

        // Usamos .ExecuteNonQuery() cuando no quiero procesar info
        dbCommand.ExecuteNonQuery();
    }

    void AddDataToPerformance(IDbConnection dbConnection, int puntuacion)
    {
        // Añadir elemento a Tabla
        string command = "INSERT INTO Performance (Score) VALUES ";
        command += $"({puntuacion});";

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = command;

        // Usamos .ExecuteNonQuery() cuando no quiero procesar info
        dbCommand.ExecuteNonQuery();
    }

    string SearchByName(IDbConnection dbConnection, string nombre)
    {
        string player = "";

        // Buscar elemento(s) en mi tabla
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"SELECT Name, GameMode, Nationality FROM Player WHERE Name='{nombre}';";

        //Usamos .ExecuteReader() cuando queremos q nos devuelva un reader para procesar
        //los datos q me devuelve
        IDataReader reader = dbCommand.ExecuteReader();
        while(reader.Read())
        {
            player += $"{reader.GetString(0)}, {reader.GetString(1)}, {reader.GetString(2)}\n";
        }

        return player;
    }

    string SearchByNationality(IDbConnection dbConnection, string nacionalidad)
    {
        string player = "";

        // Buscar elemento(s) en mi tabla
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"SELECT Name, GameMode, Nationality FROM Player WHERE Nationality='{nacionalidad}';";

        //Usamos .ExecuteReader() cuando queremos q nos devuelva un reader para procesar
        //los datos q me devuelve
        IDataReader reader = dbCommand.ExecuteReader();
        while (reader.Read())
        {
            player += $"{reader.GetString(0)}, {reader.GetString(1)}, {reader.GetString(2)}\n";
        }

        return player;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AddRandomData(IDbConnection dbConnection)
    {
        int num_scores = scores_default.Length;
        string command = "INSERT INTO Performance (Score) VALUES ";
        for (int i = 0; i < num_scores; i++)
        {
            command += $"('{scores_default[i]}'),";
        }
        command = command.Remove(command.Length - 1, 1);
        command += ";";
        command += "INSERT INTO Player (Name, Nationality, Performance) VALUES ";
        System.Random rnd = new System.Random();
        for (int i = 0; i < NUM_PLAYERS; i++)
        {
            string names = nombres_default[rnd.Next(nombres_default.Length)];
            string nationalities = nacionalidad_default[rnd.Next(nacionalidad_default.Length)];
            int scores = rnd.Next(num_scores) + 1;
            command += $"('{names}','{nationalities}','{scores}'),";
        }
        command = command.Remove(command.Length - 1, 1);
        command += ";";
        //Debug.Log(command);
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = command;
        dbCommand.ExecuteNonQuery();
    }
}
