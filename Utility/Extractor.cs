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

            if (!string.IsNullOrEmpty(last))
            {
                System.Console.Write(new string('\b', last.Length));
            }

            last = string.Format("{0}/{1}", this.current, this.max);

            System.Console.Write(last);
        }

        public virtual string Name { get { return this.GetType().Name; } }

        public abstract void Extract();

        protected void Write<T>(T item, string filename)
        {
            if (System.IO.File.Exists(filename)) System.IO.File.Delete(filename);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
            System.IO.File.WriteAllText(filename, json);
        }
    }
}
