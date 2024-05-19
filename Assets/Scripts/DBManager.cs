using Mono.Data.Sqlite;
using Newtonsoft.Json;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;

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
    private int[] scores_default = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
    private string[] nombres_default = { "Daria", "Mercedes", "Ana", "Fernando", "Ramon", "Ben", "Raul", "Pedro", "Lucia", "Poncho" };
    private string[] nacionalidad_default = { "Spanish", "Brazilian", "German", "British", "French", "Italian", "Chinese", "Australian", "Japanese", "Ecuatorian" };
    private int NUM_PLAYERS = 100;

    // Start is called before the first frame update
    void Start()
    {
        // Creamos la base de datos
        IDbConnection dbConnection = CreateAndOpenDataBase();
        InitializeDB(dbConnection);
        AddRandomData(dbConnection);
        UpdatePlayerInfo(dbConnection, "Filipino", "Ben");
        DeletePlayerInfo(dbConnection, "Poncho");
        Debug.Log(SearchByScore(dbConnection, 5)); ;

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

    void UpdatePlayerInfo(IDbConnection dbConnection, string nacionalidad, string name)
    {
        // Editar un valor de la tabla
        string command = "UPDATE Player " +
            $"SET Nationality='{nacionalidad}' " +
            $"WHERE Name='{name}';";
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = command;
        dbCommand.ExecuteNonQuery();
    }

    void DeletePlayerInfo(IDbConnection dbConnection, string name)
    {
        // Eliminar una(s) filas de la tabla
        string command = "DELETE FROM Player " +
            $"WHERE Name='{name}';";
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = command;
        dbCommand.ExecuteNonQuery();
    }

    string SearchByName(IDbConnection dbConnection, string nombre)
    {
        string player = "";

        // Buscar elemento(s) en mi tabla
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"SELECT Name, GameMode, Nationality, Performance.Score " +
            $"FROM Player  " +
            $"JOIN Performance ON player.Performance = Performance.Id" +
            $"WHERE Name='{nombre}';";

        //Usamos .ExecuteReader() cuando queremos q nos devuelva un reader para procesar
        //los datos q me devuelve
        IDataReader reader = dbCommand.ExecuteReader();
        while(reader.Read())
        {
            player += $"{reader.GetString(0)}, {reader.GetString(1)}, {reader.GetString(2)}, {reader.GetInt32(3)}\n";
        }

        return player;
    }

    string SearchByNationality(IDbConnection dbConnection, string nacionalidad)
    {
        string player = "";

        // Buscar elemento(s) en mi tabla
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"SELECT Name, GameMode, Nationality, Performance.Score " +
            $"FROM Player  " +
            $"JOIN Performance ON player.Performance = Performance.Id" +
            $"WHERE Nationality='{nacionalidad}';";

        //Usamos .ExecuteReader() cuando queremos q nos devuelva un reader para procesar
        //los datos q me devuelve
        IDataReader reader = dbCommand.ExecuteReader();
        while (reader.Read())
        {
            player += $"{reader.GetString(0)}, {reader.GetString(1)}, {reader.GetString(2)}, {reader.GetInt32(3)}\n";
        }
        return player;
    }

    string SearchByScore(IDbConnection dbConnection, int puntuacion)
    {
        // Buscamos en la tabla Performance todos valores q coincidan con
        //puntuacion y guardamos el id
        IDbCommand dbCmd = dbConnection.CreateCommand();
        dbCmd.CommandText = $"SELECT Id FROM Performance WHERE Score='{puntuacion}';";
        IDataReader reader = dbCmd.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }
        int id_performance = reader.GetInt32(0);
        reader.Close();

        dbCmd.CommandText = $"SELECT Name, GameMode, Nationality, Performance.Score " +
            $"FROM Player  " +
            $"JOIN Performance ON player.Performance = Performance.Id" +
            $"WHERE Performance='{id_performance}';";
        reader = dbCmd.ExecuteReader();
        string players = "";
        while (reader.Read())
        {
            players += $"{reader.GetString(0)}, {reader.GetString(1)}, {reader.GetString(2)}, {reader.GetInt32(3)}\n";
        }
        return players;
    }

    void FromSqlToXml (IDbConnection dbConnection)
    {
        string command = "SELECT * FROM Player, ";
        IDbCommand dbCommand = dbConnection.CreateCommand();

    }

    /// <summary>
    /// Guarda en fichero (xml) un objeto.
    /// </summary>
    /// <param name="objetoBbDd">Objeto a guardar.</param>
    /// <param name="fileName">Nombre del fichero sin extensión.</param>
    private void WriteSqlToXml(object objetoBbDd, string fileName)
    {
        XmlSerializer xs = new XmlSerializer(objetoBbDd.GetType());

        string filePathComplete = Path.Combine(Directory.GetCurrentDirectory(), $"{fileName}.xml");
        Stream stream = new FileStream(filePathComplete, FileMode.Create);
        
        XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Unicode);

        xs.Serialize(writer, objetoBbDd);
        writer.Close();
    }

    /// <summary>
    /// Guarda en fichero (json) un objeto.
    /// </summary>
    /// <param name="objetoBbDd">Objeto a guardar.</param>
    /// <param name="fileName">Nomvre del fichero sin extensión.</param>
    private void WriteSqlToJson(object objetoBbDd, string fileName)
    {
        string textjson = JsonConvert.SerializeObject(objetoBbDd);

        string filePathComplete = Path.Combine(Directory.GetCurrentDirectory(), $"{fileName}.json");
        File.WriteAllText(filePathComplete, textjson);
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
