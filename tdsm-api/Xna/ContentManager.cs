namespace Microsoft.Xna.Framework.Content
{
    public class ContentManager
    {
        public string RootDirectory { get; set; }

        public T Load<T>(string path)
        {
            return default(T);
        }
        //public Effect Load<Effect>(string path)
        //{
        //    return default(Effect);
        //}
    }
}
