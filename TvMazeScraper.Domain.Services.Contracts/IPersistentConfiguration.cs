namespace TvMazeScraper.Domain.Services.Contracts
{
    public interface IPersistentConfiguration
    {
        void AddOrUpdateAppSetting<T>(string sectionPathKey, T value);

        T GetAppSetting<T>(string key);
    }
}
