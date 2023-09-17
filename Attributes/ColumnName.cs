namespace STCore.Attributes
{
    public class ColumnName : System.Attribute
    {
        public string columnName;
        public ColumnName(string colname)
        {
            columnName = colname;
        }
    }
}
