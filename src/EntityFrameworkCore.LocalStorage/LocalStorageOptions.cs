namespace EntityFrameworkCore.LocalStorage
{
    public class LocalStorageOptions
    {
        public string Serializer { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public string FileManager { get; set; }
        public string Location { get; set; }
    }
}
