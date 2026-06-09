namespace RIVS.ASAK.Core.Contract.Values
{
    public interface IParamValueContainer
    {
        ParamValueCollection GetParamValues(string prefix = "");
    }
}
