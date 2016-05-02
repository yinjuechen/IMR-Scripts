using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using NGenerics.DataStructures.General;
using UnityEngine;

namespace Assets.Scripts
{
    public class DataBaseControl
    {
        public Graph<string> GetVertexes()
        {
            var videoGraph = new Graph<string>(true);
            string conn = "URI=file:" + Application.dataPath + "/DataBaseTest.sqlite3"; //Path to database.
            IDbConnection dbconn;
            dbconn = new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM Vertexes";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                string vertex = reader.GetString(0);
                var tmpFromVer = videoGraph.AddVertex(vertex);

            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
            return videoGraph;
        }

        public Graph<string> LoadGraph()
        {
            var videoGraph = new Graph<string>(true);
            videoGraph = GetVertexes();
            string conn = "URI=file:" + Application.dataPath + "/DataBaseTest.sqlite3"; //Path to database.
            IDbConnection dbconn;
            dbconn = new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM Vertexes";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                string vertex = reader.GetString(0);
                string edges = reader.GetString(1);
                if (edges != "")
                {
                    var incidentVertexes = edges.Split(',');
                    for (int i = 0; i < incidentVertexes.Length; i++)
                    {
                        Vertex<string> tmpFromVer = videoGraph.GetVertex(vertex);
                        Vertex<string> tmpToVer = videoGraph.GetVertex(incidentVertexes[i]);
                        var tmpEdge = new Edge<string>(tmpFromVer, tmpToVer, 1, true);
                        videoGraph.AddEdge(tmpEdge);
                        Debug.LogWarning("From Vertex: " + vertex + ";" + " To Vertex: " + incidentVertexes[i]);
                    }

                }
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            return videoGraph;
        }

        public List<string> GetVideoList(string VertexValue)
        {
            List<string> videoList = new List<string>();
            string conn = "URI=file:" + Application.dataPath + "/DataBaseTest.sqlite3";//Path to database.
            IDbConnection dbconn = new SqliteConnection(conn);
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM VideoList WHERE Vertex = '" + VertexValue + "'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                string video = reader.GetString(1);
                videoList.Add(video);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
            return videoList;
        }

        public string GetPlayInfo(string FromVertexStr, string ToVertexStr, string VideoName)
        {
            string filePath = "";
            string sqlQuery = "";
            string conn = "URI=file:" + Application.dataPath + "/DataBaseTest.sqlite3";//Path to database.
            IDbConnection dbconn = new SqliteConnection(conn);
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT * FROM VideoInfo WHERE Video = '" + VideoName + "'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                filePath += reader.GetString(1);
            }
            reader.Close();
            reader = null;
            sqlQuery = "SELECT * FROM Videolist WHERE Vertex = '" + FromVertexStr + "' AND Videos = '" + VideoName +
                       "'";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                filePath += "&" + reader.GetInt32(2);
            }
            reader.Close();
            reader = null;
            sqlQuery = "SELECT * FROM Videolist WHERE Vertex = '" + ToVertexStr + "' AND Videos = '" + VideoName + "'";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                filePath += "&" + reader.GetInt32(2);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
            Debug.LogWarning("filePath: " + filePath);
            return filePath;
        }

        public void SaveAnnotation(string FilePath, string Frame, string Date, float CameraAngle, string Annotaion )
        {
            string conn = "URI=file:" + Application.dataPath + "/DataBaseTest.sqlite3"; //Path to database.
            IDbConnection dbconn;
            dbconn = new SqliteConnection(conn);
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "INSERT INTO `Annotation Info`(`FilePath`,`Frame`,`Date`,`CameraAngle`,`Annotation`) VALUES ('" + FilePath + "','" + Frame + "','" + Date + "'," + CameraAngle + ",'" + Annotaion + "' )";
            dbcmd.CommandText = sqlQuery;
            int index = dbcmd.ExecuteNonQuery();
            Debug.LogWarning("Annotation Index: " + index);
            dbconn.Close();
        }

        public List<string> GetAnnotaionList()
        {
            var annotationList = new List<string>();
            string conn = "URI=file:" + Application.dataPath + "/DataBaseTest.sqlite3"; //Path to database.
            IDbConnection dbconn;
            dbconn = new SqliteConnection(conn);
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM 'Annotation Info'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while(reader.Read())
            {
                string filePath = reader.GetString(0);
                string frame = reader.GetString(1);
                string date = reader.GetString(2);
                string option = filePath + "/" + frame + "," + date;
                annotationList.Add(option);
            }
            return annotationList;
        }

        public float GetAnnotaionAngle(string Date)
        {
            float angle = 0f;
            string conn = "URI=file:" + Application.dataPath + "/DataBaseTest.sqlite3"; //Path to database.
            IDbConnection dbconn;
            dbconn = new SqliteConnection(conn);
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM 'Annotation Info' WHERE Date = '" + Date +"'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                angle = reader.GetFloat(3);
            }
            return angle;
        }

        public string GetAnnoatationContent(string Date)
        {
            string comment = "";
            string conn = "URI=file:" + Application.dataPath + "/DataBaseTest.sqlite3"; //Path to database.
            IDbConnection dbconn;
            dbconn = new SqliteConnection(conn);
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * FROM 'Annotation Info' WHERE Date = '" + Date + "'";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                comment = reader.GetString(4);
            }
            return comment;
        }
    }
}
