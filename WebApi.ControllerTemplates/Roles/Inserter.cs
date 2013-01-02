namespace WebApi.ControllerTemplates.Roles
{
    public interface Inserter<in TInstance>
    {
        string Insert(TInstance instance);
    }
}