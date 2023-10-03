using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data.Common;
using System.Data;
using System.Text.Json;
using System.Linq;

namespace partymode
{
    internal class Database
    {
        SQLiteConnection connection;
        TCPMsg loginPlayer;
        TCPMsg infoMsg;
        TCPMsg changeSkinMsg;
        string playerTable = "samp_player";

        static public Database instance { get; private set; }
        public Database(string path) {
            /*SELECT 1 FROM samp WHERE username = @username AND password = @password*/
            instance = this;
            Console.WriteLine(path);
            connection = new SQLiteConnection("Data Source = "+path+";");
            connection.Open();
            if (connection != null && connection.State == ConnectionState.Closed)
            {
                Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": Failed to connect to DB!");
                System.Threading.Thread.Sleep(9999999);
            }
            else Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": Connected to DB " + path);

            loginPlayer = new TCPMsg("auth", this.authenticate);
            infoMsg = new TCPMsg("player_info", this.playerInfo);
            changeSkinMsg = new TCPMsg("change_skin", this.setSkin);
        }

        public bool set(Dictionary<string, object> keyValuePairs, string login)
        {
            try
            {
                string keyvaluesString="";
                foreach (var pair in keyValuePairs)
                    keyvaluesString = pair.Key+"=" + pair.Value.ToString()+",";
                if (keyvaluesString.Length == 0) return false;
                keyvaluesString = keyvaluesString.Remove(keyvaluesString.Length-1,1);
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = "UPDATE " + this.playerTable + " SET "+ keyvaluesString + " WHERE name = @name;";
                Console.WriteLine(command.CommandText);
                command.Parameters.AddWithValue("@name", login);
                SQLiteDataReader result = command.ExecuteReader();
                return true;
            } catch (Exception ex)
            {
                return false;
            }
        }
        public KeyValuePair<bool,T> get<T>(string field, string login)
        {
            var result = get(new List<string>() { field }, login);
            if (result.Count == 0) return new KeyValuePair<bool, T>();
            try
            {
                return new KeyValuePair<bool, T>(true, (T)Convert.ChangeType(result["skin"], typeof(T)));
            } catch (Exception) {
                return new KeyValuePair<bool, T>();
            }
            
        }
        public Dictionary<string, object> get(List<string> keys, string login)
        {
            Dictionary<string, object> toRet = new Dictionary<string, object>();
            try
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                string keysString = string.Join(',', keys.Select(x => x));
                command.CommandText = "SELECT " + keysString + " FROM " + this.playerTable + " WHERE name = @name;";
                command.Parameters.AddWithValue("@name", login);
                SQLiteDataReader result = command.ExecuteReader();

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
        public string setSkin(JsonElement data)
        {
            string username = data.GetProperty("login").ToString();
            var result = set(new Dictionary<string,object>() { { "skin", data.GetProperty("skin").ToString() } }, username);
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
            var result = get(new List<string>(){"score", "skin"}, username);
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
            Console.WriteLine(data);
            try
            {
                string username = data.GetProperty("login").ToString();
                string password = data.GetProperty("password").ToString();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = "SELECT 1 FROM "+ this.playerTable + " WHERE name = @name AND password = @pass;";
                
                command.Parameters.AddWithValue("@name", username);
                command.Parameters.AddWithValue("@pass", password);
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": User " + username + " has been authenticated.");
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
