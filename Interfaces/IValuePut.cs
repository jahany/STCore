using static STCore.CLS.QueryValues;

namespace STCore.Interfaces
{
    public interface IValuePut
    {
        TIUD_Parameters AggregatePutValues { get; set; }
        Cmd_Parameters ProtectPutValues { get; set; }
        IUD_Parameters PutValues { get; set; }
    }
}
