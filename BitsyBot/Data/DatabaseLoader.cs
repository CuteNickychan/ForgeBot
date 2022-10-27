using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Data.SQLite;
using Discord;
using Newtonsoft.Json;


namespace BitsyBot.Data
{
    public static class DatabaseLoader
    {
        private static readonly SQLiteConnection? _connection;
        
        private static readonly string ReactionRolesPath = "Data/reactionRoles.json";

        private static Dictionary<ulong, Dictionary<string, ulong>>? _messageReactionRoles;

        public static void LoadDatabase(){

            Console.WriteLine("Loaded Reaction Roles");
            FileInfo storageFile = new FileInfo(ReactionRolesPath);
            
            if (storageFile.DirectoryName != null) _ = Directory.CreateDirectory(storageFile.DirectoryName);
            else throw new Exception("Could not create directory for reaction roles storage file");
            
            if (!File.Exists(ReactionRolesPath))
            {
                Console.WriteLine("Reaction Role File Does Not Exist, Creating New File");
                File.Create(ReactionRolesPath).Close();

                _messageReactionRoles = new Dictionary<ulong, Dictionary<string, ulong>>();
                var newReactionEntry = new Dictionary<string, ulong>
                {
                    { "testRole", 0 }
                };
                _messageReactionRoles.Add(0, newReactionEntry);

                string json = JsonConvert.SerializeObject(_messageReactionRoles, Formatting.Indented);
                File.WriteAllText(ReactionRolesPath, json);
            }

            var reactionRoleString = File.ReadAllText(ReactionRolesPath);
            _messageReactionRoles = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, ulong>>>(reactionRoleString);

            //string cs = "Data/BitsyDB.sqlite";


            //FileInfo dbFile = new FileInfo(cs);
            //Directory.CreateDirectory(dbFile.DirectoryName ?? string.Empty);
            //SQLiteConnection.CreateFile(cs);


            //_connection = new SQLiteConnection("Data Source =" + cs);

            //TODO: create tables that do not yet exist!
        }

        public static ulong GetRole(ulong messageId, String reactionId)
        {
            return _messageReactionRoles?[messageId][reactionId] != null ? _messageReactionRoles[messageId][reactionId] : 0;
        }

        public static bool AddReactionRoleRole(ulong messageId, string emoteName, ulong roleId)
        {
            if (_messageReactionRoles.ContainsKey(messageId))
            {
                _messageReactionRoles[messageId].Add(emoteName, roleId);
            }
            else
            {
                var newReactionEntry = new Dictionary<string, ulong>
                {
                    { emoteName, roleId }
                };
                if (_messageReactionRoles == null) return false;
                _messageReactionRoles.Add(messageId, newReactionEntry);
            }
            return SaveReactionRoles();
        }

        private static bool SaveReactionRoles()
        {
            try
            {
                File.WriteAllText(ReactionRolesPath, JsonConvert.SerializeObject(_messageReactionRoles));
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
