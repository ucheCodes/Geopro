using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DBreeze;
using DBreeze.DataTypes;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace PenPro.Database
{

    public class DatabaseService
    {

        private static string path = Path.Combine(Directory.GetCurrentDirectory(), ("Database\\data"));
        private static DBreezeEngine engine = new DBreezeEngine(path);

        public bool Create(string tablename, string serializedKey, string serializedValue)
        {
            using (var trans = engine.GetTransaction())
            {
                try
                {
                    trans.Insert<string, string>(tablename, serializedKey, serializedValue);
                    trans.Commit();
                    return true;
                }
                catch (System.Exception)
                {
                    return false;
                }
            }
        }

        public KeyValuePair<string, string> Read(string tablename, string key)
        {
            using (var trans = engine.GetTransaction())
            {
                try
                {
                    string serializedKey = JsonConvert.SerializeObject(key);
                    var data = trans.Select<string, string>(tablename, serializedKey);
                    if (data.Exists)
                    {
                        return new KeyValuePair<string, string>(data.Key, data.Value);
                    }
                }
                catch (System.Exception)
                {
                    return new KeyValuePair<string, string>("", "");
                }
            }
            return new KeyValuePair<string, string>("", "");
        }

        public IEnumerable<KeyValuePair<string, string>> ReadAll(string tablename)
        {
            var list = new List<KeyValuePair<string, string>>();
            using (var trans = engine.GetTransaction())
            {
                try
                {
                    var data = trans.SelectForward<string, string>(tablename);
                    if (data != null)
                    {
                        foreach (var d in data)
                        {
                            list.Add(new KeyValuePair<string, string>(d.Key, d.Value));
                        }
                        return list;
                    }
                }
                catch (System.Exception)
                {
                    return list;
                }
            }
            return list;
        }

        public bool Delete(string tablename, string key)
        {
            using (var trans = engine.GetTransaction())
            {
                try
                {
                    string serializedKey = JsonConvert.SerializeObject(key);
                    if (trans.Select<string, string>(tablename, serializedKey).Exists)
                    {
                        trans.RemoveKey<string>(tablename, serializedKey);
                        trans.Commit();
                        return true;
                    }
                }
                catch (System.Exception)
                {
                    return false;
                }
            }
            return false;
        }

        public bool Exists(string tablename, string key)
        {
            using (var trans = engine.GetTransaction())
            {
                try
                {
                    string serializedKey = JsonConvert.SerializeObject(key);
                    if (trans.Select<string, string>(tablename, serializedKey).Exists)
                    {
                        return true;
                    }
                }
                catch (System.Exception)
                {
                    return false;
                }
            }
            return false;
        }

        public bool DeleteAll(string tablename, bool recreateFile)
        {
            using (var trans = engine.GetTransaction())
            {
                try
                {
                    trans.RemoveAllKeys(tablename, recreateFile);
                    return true;
                }
                catch (System.Exception)
                {
                    return false;
                }
            }
        }
    }
}
