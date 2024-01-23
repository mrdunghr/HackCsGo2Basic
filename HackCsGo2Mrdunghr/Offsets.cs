
namespace HackCsGo2Mrdunghr
{
    public static class Offsets
    {
        // offsets.cs
        public static int dwViewAngles = 0x1890F30;
        public static int dwLocalPlayerPawn = 0x16D4F48; // base nhân vật
        public static int dwEntityList = 0x17CE6A0; // base list nhân vật
        public static int dwForceAttack = 0x16CDE80;
        public static int dwViewMatrix = 0x182CEA0;

        // client.dll.cs
        public static int m_hPlayerPawn = 0x7EC; // địa chỉ nhân vật, gần như cái nào cũng sài
        public static int m_iHealth = 0x32C; // máu
        public static int m_vOldOrigin = 0x1224; // vị trí nhân vật
        public static int m_iTeamNum = 0x3BF; // team
        public static int m_vecViewOffset = 0xC48; // hướng nhìn nhân vật
        public static int m_lifeState = 0x330; // trạng thái nhân vật
        public static int m_iszPlayerName = 0x640; // tên
        public static int m_modelState = 0x160; // ma trận xương khung xương - esp
        public static int m_pGameSceneNode = 0x310; // tạm thời chưa biết làm gì nhưng dùng để hack eps
        public static int m_flFlashBangTime = 0x145C; // anti flash
        public static int m_iIDEntIndex = 0x1544; // id khi lia tâm vào entity - trigerbot
        public static int m_flDetectedByEnemySensorTime = 0x13E4; // thời gian glow - glow hack
    }
}
