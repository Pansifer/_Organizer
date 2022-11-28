using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace PersonalOrganizer.MVVM.Class
{
    internal class XmlSaver
    {
        //Path of save or find the xml file
        string path;

        //Costructor get path 
        public XmlSaver(string path)
        {
            this.path = path;
        }

        //Write inside path or create file if this not exist
        public void WriteData(object obj)
        {
            try
            {
                FileStream writerStream = File.OpenWrite(path);
                XmlSerializer sr = new XmlSerializer(obj.GetType());
                sr.Serialize(writerStream, obj);
                writerStream.Dispose();
            }
            catch
            {
            }
        }
        //Delete file for recreate this and prevent eventual error
        //it is not good, but Tuesday is cooming...
        public void CleanData()
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
            }
        }
        //Read data insiade the path
        public object ReadData(object obj)
        {
            try
            {
                FileStream readStream = File.OpenRead(path);
                XmlSerializer sr = new XmlSerializer(obj.GetType());
                var result = sr.Deserialize(readStream);
                readStream.Dispose();
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
