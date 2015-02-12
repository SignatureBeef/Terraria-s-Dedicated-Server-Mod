namespace tdsm.api.Permissions
{
    /// <summary>
    /// Permission node class.
    /// </summary>
    public struct Node
    {
        private string stringNode;

        // these are stable APIs
        public string Path { get { return stringNode; } }

        public static Node FromPath(string node)
        {
            return new Node(node);
        }

        /// <summary>
        /// Node class constructor.
        /// </summary>
        /// <param name="node">New text path to create node from</param>
        public Node(string node)
        {
            stringNode = node;
        }
    }
}
