namespace InventoryAPI.Settings;

public interface IInventotyDatabaseSettings
{
  string ProductCollectionName { get; set; }
  string ConnectionString { get; set; }
  string DatabaseName { get; set; }
}