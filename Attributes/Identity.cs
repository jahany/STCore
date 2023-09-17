namespace STCore.Attributes
{
    /// <summary>
    /// If one prop of class have this attr in insert and Update Not use that proper
    /// </summary>
    public class Identity : System.Attribute
    {
        int increase;
        public Identity(int a = 1)
        { 
            increase = a;
        }
    }
}
