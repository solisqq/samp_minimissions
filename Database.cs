﻿/*#define DB_SQLITE*/
#define DB_POSTGRE

using System;
using System.Collections.Generic;
using System.Text;
#if DB_SQLITE
using System.Data.SQLite;
#elif DB_POSTGRE
using Npgsql;
#endif
using System.Data.Common;
using System.Data;
using System.Text.Json;
using System.Linq;
using SampSharp.GameMode.Definitions;
using System.Data.SQLite;

namespace partymode
{
    internal class Database
    {
#if DB_SQLITE
        SQLiteConnection connection;
#elif DB_POSTGRE
        NpgsqlDataSource dataSource;
#endif
        TCPMsg loginPlayer;
        TCPMsg infoMsg;
        TCPMsg changeSkinMsg;
        TCPMsg carInfo;
        TCPMsg buyCarMsg;
        string playerTable = "samp_player";

        static public Database instance { get; private set; }
        public Database(string path) {
            /*SELECT 1 FROM samp WHERE username = @username AND password = @password*/
            instance = this;
#if DB_SQLITE
            Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": connecting to SQLiteDB");
            connection = new SQLiteConnection("Data Source = "+path+";");
            connection.Open();
            if (connection != null && connection.State == ConnectionState.Closed)
            {
                Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": failed to connect to DB!");
                System.Threading.Thread.Sleep(9999999);
            } else Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": connected to DB " + path);
#elif DB_POSTGRE
            Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": connecting to PostgresDB");
            var connectionString = "Host=127.0.0.1;Username=postgres;Password=1937555a;Database=samp";
            dataSource = NpgsqlDataSource.Create(connectionString);
            Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": successfully connected to PostgresDB " + path);
#endif
            loginPlayer = new TCPMsg("auth", this.authenticate);
            infoMsg = new TCPMsg("player_info", this.playerInfo);
            changeSkinMsg = new TCPMsg("change_skin", this.setSkin);
            carInfo = new TCPMsg("get_car", this.getCarInfo);
            buyCarMsg = new TCPMsg("buy_car", this.buyCar);
        }
        public string buyCar(JsonElement data)
        {
            string username = data.GetProperty("login").ToString();
            var result = this.get<int>("score", this.playerTable, "name", username, 0);
            try
            {
                var pointsToSpend = Convert.ToInt32(data.GetProperty("price").ToString());
                if (result.Key == false) Console.WriteLine("Cos nie tak");
                else Console.WriteLine(result.Value);/**/
                if (result.Value >= pointsToSpend)
                {
                    
                    var toRet = utils.createFlatJson(new Dictionary<string, object>() { 
                        { "status", "success" },
                        { "login", username },
                        { "model", data.GetProperty("model").ToString() },
                        { "name", data.GetProperty("name").ToString() }
                    });
                    set(new Dictionary<string, object>() { { "score", (result.Value - pointsToSpend) } }, this.playerTable, "name", username);
                    return toRet;
                } else
                {
                    var toRet = utils.createFlatJson(new Dictionary<string, object>() {
                        { "status", "success" },
                        { "login", username },
                        { "name", "points" }
                    });
                    return toRet;
                }
            } catch {}
            return utils.createFlatJson(new Dictionary<string, object>(){
                { "status", "fail" },
                { "login", username }
            });
        }
        public string getCarInfo(JsonElement data)
        {
            string username = data.GetProperty("login").ToString();
            var result = this.getPlayerCarInfo(username);
            if (result.Count != 6) return utils.createFlatJson(result);
            return utils.createFlatJson(new Dictionary<string, object>(){
                { "status", "fail" },
                { "login", username }
            });
        }
        public bool set(Dictionary<string, object> keyValuePairs, string table, string whereKey, string whereValue)
        {
            try
            {
                string keyvaluesString="";
                foreach (var pair in keyValuePairs)
                    keyvaluesString = pair.Key+"=" + pair.Value.ToString()+",";
                if (keyvaluesString.Length == 0) return false;
                keyvaluesString = keyvaluesString.Remove(keyvaluesString.Length-1,1);
#if DB_SQLITE
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = "UPDATE " + table + " SET "+ keyvaluesString + " WHERE "+whereKey+" = @name;";
#elif DB_POSTGRE
                var command = dataSource.CreateCommand("UPDATE " + table + " SET " + keyvaluesString + " WHERE " + whereKey + " = @name;");
#endif
                Console.WriteLine(command.CommandText);
                command.Parameters.AddWithValue("@name", whereValue);
                command.ExecuteReader();
                return true;
            } catch (Exception ex)
            {
                return false;
            }
        }
        public KeyValuePair<bool,T> get<T>(string field, string table, string whereKey, string whereValue, T defaultValue)
        {
            var result = get(new List<string>() { field }, table, whereKey, whereValue);
            if (result.Count == 0) return new KeyValuePair<bool, T>(false, defaultValue);
            try
            {
                return new KeyValuePair<bool, T>(true, (T)Convert.ChangeType(result[field], typeof(T)));
            } catch (Exception) {
                return new KeyValuePair<bool, T>(false, defaultValue);
            }
        }
        public Dictionary<string, object> get(List<string> keys, string table, string whereKey, string whereValue)
        {
            Dictionary<string, object> toRet = new Dictionary<string, object>();
            try
            {
                string keysString = string.Join(',', keys.Select(x => x));
#if DB_SQLITE
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = "SELECT " + keysString + " FROM " + table + " WHERE "+ whereKey + " = @value;";
#elif DB_POSTGRE
                var command = dataSource.CreateCommand("SELECT " + keysString + " FROM " + table + " WHERE " + whereKey + " = @value;");
#endif
                command.Parameters.AddWithValue("@value", whereValue);
                var result = command.ExecuteReader();

                if (result.Read())
                {
                    int id = 0;
                    foreach (var key in keys)
                    {
                        toRet.Add(key, result.GetValue(id++));
                    }
                }
            }
            catch (Exception ex){}
            return toRet;
        }
        public Dictionary<string, object> getPlayerCarInfo(string login)
        {
            Dictionary<string, object> toRet = new Dictionary<string, object>();
            try
            {
                var result = get(new List<string>() { "carid", "name", "model", "parts", "color" }, "samp_car", "owner", login); 
                toRet.Add("status", "success");
                toRet.Add("login", login);
                if (result.Count>0)
                {
                    toRet.Add("carid", result["carid"].ToString());
                    toRet.Add("name",  result["name"].ToString());
                    toRet.Add("model", result["model"].ToString());
                    toRet.Add("parts", result["parts"].ToString());
                    toRet.Add("color", result["color"].ToString());
                } 
                else toRet.Add("carid", (-1).ToString());
            }
            catch (Exception ex) { }
            return toRet;
        }
        public string setSkin(JsonElement data)
        {
            string username = data.GetProperty("login").ToString();
            var result = set(new Dictionary<string,object>() { { "skin", data.GetProperty("skin").ToString() } }, this.playerTable, "name", username);
            if (result) return utils.createFlatJson(new Dictionary<string, object>() {
                { "status", "success" },
                { "login", username }
            });
            return utils.createFlatJson(new Dictionary<string, object>(){
                { "status", "fail" },
                { "login", username }
            });
        }
        public string playerInfo(JsonElement data)
        {
            string username = data.GetProperty("login").ToString();
            var result = get(new List<string>(){"score", "skin"}, this.playerTable, "name", username);
            if (result.Count == 2) return utils.createFlatJson(new Dictionary<string, object>() { 
                { "status", "success" },
                { "login", username },
                { "score", result["score"] },
                { "skin", result["skin"] },
            });
            return utils.createFlatJson(new Dictionary<string, object>(){ 
                { "status", "fail" }, 
                { "login", username } 
            });
        }
        public string authenticate(JsonElement data)
        {
            try
            {
                string username = data.GetProperty("login").ToString();
                string password = data.GetProperty("password").ToString();
#if DB_SQLITE
                var command = new SQLiteCommand(connection);
                command.CommandText = "SELECT 1 FROM "+ this.playerTable + " WHERE name = @name AND password = @pass;";
#elif DB_POSTGRE
                var command = dataSource.CreateCommand("SELECT 1 FROM " + this.playerTable + " WHERE name = @name AND password = @pass;");
#endif
                command.Parameters.AddWithValue("@name", username);
                command.Parameters.AddWithValue("@pass", password);
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return "\"status\": \"success\", \"login\": \""+username+"\"";
                }
                else
                {
                    Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": Failed to authenticate user " + username);
                    return "\"status\": \"fail\", \"login\": \""+username+"\"";
                }
            }
            catch (Exception ex)
            {
                return "\"status\": \"fail\", \"login\": \"unknown\"";
            }
        }
    }
}
