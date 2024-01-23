using System.Numerics;

namespace HackCsGo2Mrdunghr
{
    public class Entity
    {
        public IntPtr pawnAddress { get; set; }
        public IntPtr controllerAddress { get; set; }
        public Vector3 origin { get; set; }
        public Vector3 view { get; set; }
        public Vector3 head { get; set; }
        public Vector2 head2d { get; set; } // where their head is located on the screen
        public int health { get; set; } // máu
        public int team { get; set; } // đội
        public uint lifeState { get; set; } // trạng thái nhân vật
        public float distance { get; set; } // khoảng cách nhân vật
        public float pixcelDistance { get; set; }
        public String playerName { get; set; } // tên  người chơi
        public List<Vector3> bones { get; set; }
        public List<Vector2> bones2d { get; set; }
    }
    // làm khung cho xương
    public enum BoneIds // chúng cần phải theo thứ tự giá trị bằng số, nếu không việc lặp lại sẽ rối tung lên
    {                   // đây là từ góc nhìn của thực thể, Handright sẽ là cánh tay phải của họ
        Waist = 0,              // 0 Cổ bụng
        Neck = 5,               // 1 Cổ 
        Head = 6,               // 2 Đầu 
        ShoulderLeft = 8,       // 3 Vai trái
        ForeLeft = 9,           // 4 Cánh tay trái
        HandLeft = 11,          // 5 Bàn tay trái 
        ShoulderRight = 13,     // 6 Vai phải
        ForeRight = 14,         // 7 Cánh tay phải
        HandRight = 16,         // 8 Bàn tay phải
        KneeLeft = 23,          // 9 Đầu gối trái
        FeetLeft = 24,          // 10 Chân trái
        KneeRight = 26,         // 11 Đầu gối phải
        FeetRight = 27          // 12 Chân phải
    }
}
