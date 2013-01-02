namespace WebApi.ControllerTemplates
{
    public interface Inserter<in TInstance>
    {
        string Insert(TInstance instance);
    }
}