namespace Jrd.Gameplay.Storage.Service
{
    public class ManufacturedService : StorageService
    {
        public static ManufacturedService Instance { private set; get; }

        protected void Awake()
        {
            Instance ??= this;
        }
    }
}