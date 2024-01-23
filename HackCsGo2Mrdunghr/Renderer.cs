using ClickableTransparentOverlay;
using ImGuiNET;
using System.Numerics;

namespace HackCsGo2Mrdunghr
{
    internal class Renderer : Overlay
    {
        public Vector2 overlaySieze = new Vector2(1920, 1080); // cài màn hình mặc định
        Vector2 windowLocation = new Vector2(0, 0); // if windowed fullscreen xy = 0
        public List<Entity> entitiesCopy = new List<Entity>();
        public Entity localPlayerCopy = new Entity();
        ImDrawListPtr drawListPtr;
        public bool esp = false;
        Vector4 teamColor = new Vector4(1, 1, 1, 1); // trang
        Vector4 enemyColor = new Vector4(1, 0, 0, 1); // do

        public Vector2 screenSize = new Vector2(1920, 1080); //đặt ở độ phân giải của bạn, của tôi đang là 1920 1080
        public float FOV = 50; // in pixels, we are using w2s and not angles!
        public Vector4 circleColor = new Vector4(1, 1, 1, 1); // màu cảu vòng tròn, mặc định trắng

        float boneThickness = 4;

        public bool aimbot = false;
        public bool aimOnTeam = false;
        public bool triggerBot = true;
        public bool antiFlash = true;
        public bool glowHack = true;
        public bool fovAim = false;


        protected override void Render()
        {
            ImGui.Begin("Ha Van Dung Hack CS2");
            if (ImGui.BeginTabBar("Tabs"))
            {
                if (ImGui.BeginTabItem("ESP"))
                {
                    ImGui.Checkbox("Skelettons", ref esp);
                    ImGui.SliderFloat("Bone Thickness", ref boneThickness, 4, 5000);

                    if (ImGui.CollapsingHeader("Team Color")) // team
                        ImGui.ColorPicker4("##teamcolor", ref teamColor);

                    if (ImGui.CollapsingHeader("Enemy Color")) // enemy
                        ImGui.ColorPicker4("##enemy", ref enemyColor);
                    // Thêm các chức năng ESP khác nếu cần

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("AimBot"))
                {
                    ImGui.Checkbox("AimBot", ref aimbot);
                    ImGui.Checkbox("Aim On Team", ref aimOnTeam);
                    ImGui.Checkbox("TriggerBot", ref triggerBot);
                    // Thêm các chức năng AimBot khác nếu cần

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("FOV AimBot"))
                {
                    ImGui.Checkbox("AimBot", ref fovAim);
                    ImGui.Checkbox("Aim On Team", ref aimOnTeam);
                    ImGui.SliderFloat("Pixel FOV", ref FOV, 10, 300);
                    if (ImGui.CollapsingHeader("FOV circle Color"))
                        ImGui.ColorPicker4("##circlecolor", ref circleColor);
                    // Thêm các chức năng AimBot khác nếu cần

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Misc"))
                {
                    ImGui.Checkbox("Anti Flash", ref antiFlash);
                    ImGui.Checkbox("Glow Hack", ref glowHack);
                    // Thêm các chức năng Misc khác nếu cần

                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }

            if (esp)
            {
                //draw overlay and skeletons
                DraOverlay();
                DrawSkelettons();
            }

            if (fovAim)
            {
                DraOverlay();
                ImDrawListPtr drawList = ImGui.GetWindowDrawList();
                drawList.AddCircle(new Vector2(screenSize.X / 2, screenSize.Y / 2), FOV, ImGui.ColorConvertFloat4ToU32(circleColor));
            }

            ImGui.End();


            //ImGui.Checkbox("ESP Skelettons", ref esp);
            //ImGui.Checkbox("AimBot", ref aimbot);
            //ImGui.Checkbox("AimOnTeam", ref aimOnTeam);
            //ImGui.Checkbox("TriggerBot", ref triggerBot);
            //ImGui.Checkbox("Anti Flash", ref antiFlash);
            //ImGui.Checkbox("Glow Hack", ref glowHack);

            //ImGui.SliderFloat("bone thickness", ref boneThickness, 4, 5000);

            ////color scheme
            //if (ImGui.CollapsingHeader("Team Color")) // team
            //    ImGui.ColorPicker4("##teamcolor", ref teamColor);

            //if (ImGui.CollapsingHeader("Enemy Color")) // enemy
            //    ImGui.ColorPicker4("##enemy", ref enemyColor);

            //if (esp)
            //{
            //    //draw overlay and skeletons
            //    DraOverlay();
            //    DrawSkelettons();
            //}
            //ImGui.End();
        }

        void DrawSkelettons()
        {
            if (entitiesCopy.Count == 0 || entitiesCopy == null)
                return; // safety check

            //List<Entity> tempEntities = new List<Entity>(entitiesCopy).ToList(); // make another copy // code gốc
            //List<Entity> tempEntities = entitiesCopy != null ? new List<Entity>(entitiesCopy) : new List<Entity>(); code sửa để fix null trong list

            drawListPtr = ImGui.GetWindowDrawList();
            uint unitColor;

            //loop through bone and draw
            //foreach (Entity entity in tempEntities)  // code gốc
            entitiesCopy.RemoveAll(e => e == null);
            foreach (Entity entity in entitiesCopy.Where(e => e != null).ToList())
            {
                if (entity == null) continue;
                //get either team or enemy color depending on the team
                unitColor = localPlayerCopy.team == entity.team
                    ? ImGui.ColorConvertFloat4ToU32(teamColor)
                    : ImGui.ColorConvertFloat4ToU32(enemyColor);

                // check that entity is on screen
                if (entity.bones2d[2].X > 0 && entity.bones2d[2].Y > 0 && entity.bones2d[2].X < overlaySieze.X && entity.bones2d[2].Y < overlaySieze.Y)
                {
                    float currenBoneThickness = boneThickness / entity.distance; // not perfect but something
                    //draw lines between bones
                    drawListPtr.AddLine(entity.bones2d[1], entity.bones2d[2], unitColor, currenBoneThickness); // vẽ cổ tới đầu
                    drawListPtr.AddLine(entity.bones2d[1], entity.bones2d[3], unitColor, currenBoneThickness); // vẽ cổ đến vai trái
                    drawListPtr.AddLine(entity.bones2d[1], entity.bones2d[6], unitColor, currenBoneThickness); // vẽ cổ đến vai phải
                    drawListPtr.AddLine(entity.bones2d[3], entity.bones2d[4], unitColor, currenBoneThickness); // vẽ vai trái tới cánh tay trái
                    drawListPtr.AddLine(entity.bones2d[6], entity.bones2d[7], unitColor, currenBoneThickness); // vẽ vai phải tới cánh tay phải
                    drawListPtr.AddLine(entity.bones2d[4], entity.bones2d[5], unitColor, currenBoneThickness); // vẽ cánh tay trái tới bàn tay trái
                    drawListPtr.AddLine(entity.bones2d[7], entity.bones2d[8], unitColor, currenBoneThickness); // vẽ cánh tay phải tới bàn tay phải
                    drawListPtr.AddLine(entity.bones2d[1], entity.bones2d[0], unitColor, currenBoneThickness); // vẽ cổ tới bụng
                    drawListPtr.AddLine(entity.bones2d[0], entity.bones2d[9], unitColor, currenBoneThickness); // vẽ bụng tới đầu gối trái
                    drawListPtr.AddLine(entity.bones2d[0], entity.bones2d[11], unitColor, currenBoneThickness); // vẽ bụn tới đầu gối phải
                    drawListPtr.AddLine(entity.bones2d[9], entity.bones2d[10], unitColor, currenBoneThickness); // vẽ đầu gối trái tới chân trái
                    drawListPtr.AddLine(entity.bones2d[11], entity.bones2d[12], unitColor, currenBoneThickness); // vẽ đầu gối phải tới chân phải

                    drawListPtr.AddCircle(entity.bones2d[2], 3 + currenBoneThickness, unitColor); //circle on head

                    // Thêm khoảng cách ngay trên đầu bộ xương
                    float distanceAboveHead = 30.0f; // Điều chỉnh khoảng cách tùy ý
                    Vector2 textPosition = new Vector2(entity.bones2d[2].X, entity.bones2d[2].Y - distanceAboveHead);
                    drawListPtr.AddText(textPosition, unitColor, $"{(int)(entity.distance) / 100}m");
                }

            }
        }

        void DraOverlay() // thay overlay we have used a million times, make a new window and clickthrough
        {
            ImGui.SetNextWindowSize(overlaySieze);
            ImGui.SetNextWindowPos(windowLocation);
            ImGui.Begin("overlay", ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoScrollWithMouse
                );

        }
    }
}
