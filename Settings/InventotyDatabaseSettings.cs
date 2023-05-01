namespace InventoryAPI.Settings
{
  public class InventotyDatabaseSettings : IInventotyDatabaseSettings
  {
    public string ProductCollectionName { get; set; }
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
  }
}
