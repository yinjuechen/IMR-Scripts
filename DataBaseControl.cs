﻿using System;
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

        //public List<string> GetAllbundlePath()
        //{
        //    List<string> bundleList = new List<string>();
        //    string conn = "URI=file:" + Application.dataPath + "/DataBaseTest.sqlite3";//Path to database.
        //    IDbConnection dbconn = new SqliteConnection(conn);
        //    dbconn.Open();
        //    IDbCommand dbcmd = dbconn.CreateCommand();
        //    string sqlQuery = "SELECT * FROM VideoInfo";
        //    dbcmd.CommandText = sqlQuery;
        //    IDataReader reader = dbcmd.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        string bundleName = reader.GetString(1);
        //        bundleList.Add(bundleName);
        //    }
        //    reader.Close();
        //    reader = null;
        //    dbcmd.Dispose();
        //    dbcmd = null;
        //    dbconn.Close();
        //    dbconn = null;
        //    return bundleList;
        //}

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
    }
}