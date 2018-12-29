using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace howl
{
    class Release
    {
        public string name;
        public string timestamp;
        public string bigmd5;
        public string hwlmd5;
        public string hwllist;

        public string[] fileList;

        public static Release Find(string md5)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            string rel = path + "releases.txt";
            string names = path + "howlnames.txt";

            if (File.Exists(rel))
            {
                string[] x = File.ReadAllLines(rel);

                for (int i = 1; i < x.Length; i++)
                {
                    Release r = new Release();
                    r.Read(x[i]);

                    if (r.hwlmd5 == md5)
                    {
                        if (File.Exists(names))
                            r.fileList = Release.ReadNames(names, r.hwllist);
                        return r;
                    }
                }
            }

            return null;
        }

        public void Read(string s)
        {
            string[] x = s.Replace("\t", "").Replace(" ", "").Split(';');

            name = x[0];
            timestamp = x[1];
            bigmd5 = x[2];
            hwlmd5 = x[3];
            hwllist = x[4];

            //Console.WriteLine(name + " " + timestamp + " " + bigmd5 + " " + hwlmd5 + " " + hwllist);
        }


        public static string[] ReadNames(string fn, string listname)
        {
            string[] lines = File.ReadAllLines(fn);

            for (int i = 0; i < lines.Length; i++)
            {
                string[] x = lines[i].Replace("\t", "").Replace(" ", "").Split(':');

                if (x[0] == listname)
                    return x[1].Split(';');
            }

            return null;
        }
    }
}
