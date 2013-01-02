namespace WebApi.ControllerTemplates.Roles
{
    public interface Upserter<in TInstance>
    {
        UpsertResult Upsert(string id, TInstance instance);
    }
}