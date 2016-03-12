using System.Xml.Serialization;

namespace TDSM.Utility
{
    public abstract class Extractor
    {
        private int min, max, current;
        private string last;

        protected void Min(int value)
        {
            this.min = value;
            this.current = 0;
        }

        protected void Max(int value)
        {
            this.max = value;
        }

        protected void Increment()
        {
            this.current++;

            if(!string.IsNullOrEmpty(last))
            {
                System.Console.Write(new string('\b', last.Length));
            }

            last = string.Format("{0}/{1}", this.current, this.max);

            System.Console.Write(last);
        }

        public virtual string Name { get { return this.GetType().Name; } }

        public abstract void Extract();

        protected void Write<T>(T item)
        {
            var bf = new XmlSerializer(typeof(T));
            var info = new System.IO.FileInfo("npc.xml");
            if (info.Exists) info.Delete();

            using (var fs = info.OpenWrite())
            {
                bf.Serialize(fs, item);
                fs.Flush();
            }

            bf = null;
        }
    }
}
