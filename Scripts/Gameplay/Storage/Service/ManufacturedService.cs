namespace GamePlay.Storage.Service
{
    public class ManufacturedService : StorageService
    {
        public static ManufacturedService Instance { private set; get; }

        protected void Awake()
        {
            Instance ??= this;
        }

        public static bool IsEnoughSpaceInManufacturedBox()
        {
            return false;
        }
    }
}