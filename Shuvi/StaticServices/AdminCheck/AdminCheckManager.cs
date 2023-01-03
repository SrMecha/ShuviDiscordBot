 namespace Shuvi.StaticServices.AdminCheck
{
    public static class AdminCheckManager
    {
        public static AdminsData Data { get; private set; } = new();

        public static void SetAdmins(AdminsData data)
        {
            Data = data;
        }
        public static bool AddAdmin(ulong id)
        {
            if (Data.AdminIds.Contains(id))
                return false;
            Data.AdminIds.Add(id);
            return true;
        }
        public static bool RemoveAdmin(ulong id)
        {
            return Data.AdminIds.Remove(id);
        }
        public static bool IsAdmin(ulong id)
        {
            return Data.AdminIds.Contains(id);
        }
    }
}
