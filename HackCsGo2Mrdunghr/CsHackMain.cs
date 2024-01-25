using HackCsGo2Mrdunghr;
using HVDLibMem64;
using System.Numerics;
using System.Runtime.InteropServices;


HVDLibMem swed = null;

while (swed == null)
{
    try
    {
        // Thử tạo một đối tượng HVDLibMem với tên tiến trình "cs2"
        swed = new HVDLibMem("cs2");
    }
    catch (Exception ex)
    {
        // Nếu không thành công, in ra thông báo và chờ 1 giây trước khi thử lại
        Console.WriteLine("CounterStrike2 is not open yet, please open the game to continue using hacked BY MRDUNGHR.....");
        Thread.Sleep(1000);
    }
}

Renderer renderer = new Renderer();
renderer.Start().Wait();

IntPtr client = swed.GetModuleBase("client.dll");
IntPtr forceAttack = client + Offsets.dwForceAttack;

List<Entity> entities = new List<Entity>(); // lấy ra danh sách các người chơi trong phòng
Entity localPlayer = new Entity();

//additional memory reader
Reader reader = new Reader(swed);

Vector2 screen = new Vector2(1920, 1080); // màn hình trò chơi của chúng tôi, hãy đảm bảo sử dụng ở chế độ toàn màn hình có cửa sổ! cái này sẽ không ở chế độ toàn màn hình không có cửa sổ
Vector2 screenSize = renderer.screenSize;
renderer.overlaySieze = screen; // ghi màn hình mặc định

while (true)
{
    Console.Clear();
    entities.Clear();

    if (renderer.triggerBot)
        triggerBot();
        //triggerBotv2();

    if (renderer.antiFlash)
        antiFlash();

    if (renderer.glowHack)
        glowHack();

    //if (renderer.aimbot)
    //    //aimBot();
    //    aimBotHead();

    if (renderer.esp)
        espSkelettons();

    if (renderer.fovAim)
        fovAimBot();
}

void antiFlash(){
    IntPtr localPlayerPawn = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
    float flashDuration = swed.ReadFloat(localPlayerPawn, Offsets.m_flFlashBangTime);
    if (flashDuration > 0)
        swed.WriteFloat(localPlayerPawn, Offsets.m_flFlashBangTime, 0);
    Console.WriteLine(flashDuration);
}

void triggerBot()
{
    
    // nhận giá trị cập nhập
    IntPtr localPlayerPawn = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
    int entIndex = swed.ReadInt(localPlayerPawn, Offsets.m_iIDEntIndex);
    Console.WriteLine($"Crosshair/Entity ID: {entIndex}");

    if (GetAsyncKeyState(0x02) < 0)
    {
        if (entIndex > 0) // nếu thực thể *bất kỳ* nào nằm trong tâm ngắm
        {
            // bắn
            swed.WriteInt(forceAttack, 65537); // +attack
            Thread.Sleep(1);
            swed.WriteInt(forceAttack, 256); // -attack
        }
    }
}

void triggerBotv2()
{
    // lấy ra entity list và localplayer
    IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);
    IntPtr localPlayer = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);

    //get our team and crosshair id
    int team = swed.ReadInt(Offsets.dwLocalPlayerPawn, Offsets.m_iTeamNum);
    int entIndex = swed.ReadInt(Offsets.dwLocalPlayerPawn, Offsets.m_iIDEntIndex);

    // print index to console
    Console.WriteLine($"Crosshair/Entity ID: {entIndex}");

    //if entity in crosshair
    if(entIndex != 1)
    {
        // get controller from entity index
        IntPtr listEntry = swed.ReadPointer(entityList, 0x8 * ((entIndex & 0x7FFF) >> 9) + 0x10);

        // then get the paw from that controller
        IntPtr currentPawn = swed.ReadPointer(listEntry, 0x78 * (entIndex & 0x1FF));

        // get entity team
        int entityTeam = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);

        if(team != entityTeam) // if entity is enemy
        {
            if (GetAsyncKeyState(0x02) < 0)
            {
                if (entIndex > 0) // nếu thực thể *bất kỳ* nào nằm trong tâm ngắm
                {
                    swed.WriteInt(client, Offsets.dwForceAttack, 65537);
                    Thread.Sleep(10);
                    swed.WriteInt(client, Offsets.dwForceAttack, 256);
                    Thread.Sleep(10);
                }
            }
        }
    }
    Thread.Sleep(2);
}

void aimBot()
{
    // đọc bộ nhớ của danh sách entity
    IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);

    // đọc tiếp địa chỉ thu được bên trên với offset là 0x10
    IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

    // Thông tin bản thân, của nhân vật của mình
    localPlayer.pawnAddress = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
    localPlayer.team = swed.ReadInt(localPlayer.pawnAddress, Offsets.m_iTeamNum);
    localPlayer.origin = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vOldOrigin);
    localPlayer.view = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vecViewOffset);

    for (int i = 0; i < 64; i++)
    {
        if (listEntry == IntPtr.Zero)
            continue;

        // lặp qua từng thằng trong danh sách Entry với offset là 0x78, mỗi thằng cách nhau 0x78 bytes
        IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);
        if (currentController == IntPtr.Zero)
            continue;

        int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);
        if (pawnHandle == 0)
            continue;

        //apply bitmask 0x7FFF and shift bits by 9
        // 2^9 = 512
        // x / 512 lấy phần nguyên
        IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);

        //get pawn, with 1FF mask
        IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
        if (currentPawn == localPlayer.pawnAddress) // nếu đây là nhân vật của ta thì bỏ qua
            continue;

        //đọc các thuộc tính đã lấy được từ 1 thực thể
        int health = swed.ReadInt(currentPawn, Offsets.m_iHealth);
        int team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
        uint lifeState = swed.ReadUInt(currentPawn, Offsets.m_lifeState);

        //nếu các thuộc tính được giữ nguyên, thì sẽ thêm vào danh sách thực thể - đã có xác định sống chết
        if (lifeState != 256)
            continue;

        if (team == localPlayer.team && !renderer.aimOnTeam) // chức năng aim cả đồng đội
            continue;

        Entity entity = new Entity();
        entity.pawnAddress = currentPawn;
        entity.controllerAddress = currentController;
        entity.health = health;
        entity.lifeState = lifeState;
        entity.origin = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
        entity.view = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset);
        entity.distance = Vector3.Distance(entity.origin, localPlayer.origin);
        entity.playerName = swed.ReadString(currentController, Offsets.m_iszPlayerName, 16);

        entities.Add(entity);

        // hiển thị ra console
        Console.ForegroundColor = ConsoleColor.Green;
        if (team != localPlayer.team)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        Console.WriteLine($"{entity.health}hp, distance: {(int)(entity.distance) / 100}m, lifeState: {entity.lifeState}, Name: {entity.playerName}");
        Console.ResetColor();

    }

    // sắp xếp theo khảng cách và ngắm
    entities = entities.OrderBy(o => o.distance).ToList();

    if (entities.Count > 0 && GetAsyncKeyState(0x06) < 0 && renderer.aimbot)
    {
        //get view pos
        Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
        Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);

        // get angles
        Vector2 newAngles = Calculate.CalculateAngles(playerView, entityView);
        Vector3 newAnglesVec3 = new Vector3(newAngles.Y, newAngles.X, 0.0f);

        //force new angles
        swed.WriteVec(client, Offsets.dwViewAngles, newAnglesVec3);
    }
}

void espSkelettons()
{
    //lấy danh sách thực thể
    IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);

    // thực thể đầu trong danh sách
    IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

    // nhân vật của bản thân
    localPlayer.pawnAddress = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
    localPlayer.team = swed.ReadInt(localPlayer.pawnAddress, Offsets.m_iTeamNum);
    localPlayer.origin = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vOldOrigin);

    for (int i = 0; i < 64; i++)
    {
        if (listEntry == IntPtr.Zero)
            continue;

        // lặp qua từng thằng trong danh sách Entry với offset là 0x78, mỗi thằng cách nhau 0x78 bytes
        IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);
        if (currentController == IntPtr.Zero)
            continue;

        // lấy pawn của thằng hiện tại
        int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);
        if (pawnHandle == 0)
            continue;

        //apply bitmask 0x7FFF and shift bits by 9
        // 2^9 = 512
        // x / 512 lấy phần nguyên
        IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);

        //get pawn, with 1FF mask
        IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
        if (currentPawn == localPlayer.pawnAddress) // nếu đây là nhân vật của ta thì bỏ qua
            continue;

        IntPtr sceneNode = swed.ReadPointer(currentPawn, Offsets.m_pGameSceneNode);
        IntPtr boneMatrix = swed.ReadPointer(sceneNode, Offsets.m_modelState + 0x80); //0x80 is the dwBoneMatrix

        // lấy ma trận tầm nhìn
        ViewMaxtrix viewMaxtrix = reader.readMatrix(client + Offsets.dwViewMatrix);

        //get paw attributes
        int team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
        uint lifeState = swed.ReadUInt(currentPawn, Offsets.m_lifeState);

        // check if alive
        if (lifeState != 256)
            continue;

        Entity entity = new Entity();
        entity.pawnAddress = currentPawn;
        entity.controllerAddress = currentController;
        entity.team = team;
        entity.lifeState = lifeState;
        entity.origin = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
        entity.distance = Vector3.Distance(entity.origin, localPlayer.origin);
        entity.bones = reader.ReadBones(boneMatrix);
        entity.bones2d = reader.ReadBones2d(entity.bones, viewMaxtrix, screen);

        entities.Add(entity);

        // hiển thị ra console
        Console.ForegroundColor = ConsoleColor.Green;
        if (team != localPlayer.team)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        //Console.WriteLine($"{entity.health}hp, distance: {(int)(entity.distance) / 100}m, lifeState: {entity.lifeState}");
        Console.ResetColor();
    }

    //fetch over to renderer
    renderer.entitiesCopy = entities;
    renderer.localPlayerCopy = localPlayer;
    Thread.Sleep(3);
}

void glowHack()
{
    IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);
    IntPtr listEntry = swed.ReadPointer(entityList, 0x10);
    for (int i = 0; i < 64; i++)
    {
        if (listEntry == IntPtr.Zero)
            continue;

        IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);
        if (currentController == IntPtr.Zero)
            continue;

        int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);
        if (pawnHandle == 0)
            continue;

        IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);

        IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
        if (currentPawn == localPlayer.pawnAddress) // nếu đây là nhân vật của ta thì bỏ qua
            continue;

        //now that we have the pawn we can force the glow
        swed.WriteFloat(currentPawn, Offsets.m_flDetectedByEnemySensorTime, 86400); // for some odd reason this is the value for

        // write pawn so the we can see that they're there
        Console.WriteLine($"{i}: {currentPawn}");
    }

    Thread.Sleep(1);
}

void aimBotHead()
{
    // đọc bộ nhớ của danh sách entity
    IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);

    // đọc tiếp địa chỉ thu được bên trên với offset là 0x10
    IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

    // Thông tin bản thân, của nhân vật của mình
    localPlayer.pawnAddress = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
    localPlayer.team = swed.ReadInt(localPlayer.pawnAddress, Offsets.m_iTeamNum);
    localPlayer.origin = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vOldOrigin);
    localPlayer.view = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vecViewOffset);

    for (int i = 0; i < 64; i++)
    {
        if (listEntry == IntPtr.Zero)
            continue;

        // lặp qua từng thằng trong danh sách Entry với offset là 0x78, mỗi thằng cách nhau 0x78 bytes
        IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);
        if (currentController == IntPtr.Zero)
            continue;

        int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);
        if (pawnHandle == 0)
            continue;

        //apply bitmask 0x7FFF and shift bits by 9
        // 2^9 = 512
        // x / 512 lấy phần nguyên
        IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);

        //get pawn, with 1FF mask
        IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
        if (currentPawn == localPlayer.pawnAddress) // nếu đây là nhân vật của ta thì bỏ qua
            continue;

        // get scene node
        IntPtr sceneNode = swed.ReadPointer(currentPawn, Offsets.m_pGameSceneNode);

        // get bone array / bone matrix
        IntPtr boneMatrix = swed.ReadPointer(sceneNode, Offsets.m_modelState + 0x80); //0x80 is the dwBoneMatrix

        //đọc các thuộc tính đã lấy được từ 1 thực thể
        int health = swed.ReadInt(currentPawn, Offsets.m_iHealth);
        int team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
        uint lifeState = swed.ReadUInt(currentPawn, Offsets.m_lifeState);

        //nếu các thuộc tính được giữ nguyên, thì sẽ thêm vào danh sách thực thể - đã có xác định sống chết
        if (lifeState != 256)
            continue;

        if (team == localPlayer.team && !renderer.aimOnTeam) // chức năng aim cả đồng đội
            continue;

        Entity entity = new Entity();
        entity.pawnAddress = currentPawn;
        entity.controllerAddress = currentController;
        entity.health = health;
        entity.lifeState = lifeState;
        entity.origin = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
        entity.view = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset);
        entity.distance = Vector3.Distance(entity.origin, localPlayer.origin);
        entity.playerName = swed.ReadString(currentController, Offsets.m_iszPlayerName, 16);
        entity.head = swed.ReadVec(boneMatrix, 6 * 32); // 6 = bone id head, 32 = step between bone coordinates

        entities.Add(entity);

        // hiển thị ra console
        Console.ForegroundColor = ConsoleColor.Green;
        if (team != localPlayer.team)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        Console.WriteLine($"{entity.health}hp, distance: {(int)(entity.distance) / 100}m, Name: {entity.playerName}, head coordinates: {entity.head}");
        Console.ResetColor();

    }

    // sắp xếp theo khảng cách và ngắm
    entities = entities.OrderBy(o => o.distance).ToList();

    if (entities.Count > 0 && GetAsyncKeyState(0x06) < 0 && renderer.aimbot)
    {
        //get view pos
        Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
        Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);

        // get angles
        Vector2 newAngles = Calculate.CalculateAngles(playerView, entities[0].head);
        Vector3 newAnglesVec3 = new Vector3(newAngles.Y, newAngles.X, 0.0f);

        //force new angles
        swed.WriteVec(client, Offsets.dwViewAngles, newAnglesVec3);
    }
}

void fovAimBot()
{
    // đọc bộ nhớ của danh sách entity
    IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);

    // đọc tiếp địa chỉ thu được bên trên với offset là 0x10
    IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

    // Thông tin bản thân, của nhân vật của mình
    localPlayer.pawnAddress = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
    localPlayer.team = swed.ReadInt(localPlayer.pawnAddress, Offsets.m_iTeamNum);
    localPlayer.origin = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vOldOrigin);
    localPlayer.view = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vecViewOffset);

    for (int i = 0; i < 64; i++)
    {
        if (listEntry == IntPtr.Zero)
            continue;

        // lặp qua từng thằng trong danh sách Entry với offset là 0x78, mỗi thằng cách nhau 0x78 bytes
        IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);
        if (currentController == IntPtr.Zero)
            continue;

        int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);
        if (pawnHandle == 0)
            continue;

        //apply bitmask 0x7FFF and shift bits by 9
        // 2^9 = 512
        // x / 512 lấy phần nguyên
        IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);

        //get pawn, with 1FF mask
        IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
        if (currentPawn == localPlayer.pawnAddress) // nếu đây là nhân vật của ta thì bỏ qua
            continue;

        // get scene node
        IntPtr sceneNode = swed.ReadPointer(currentPawn, Offsets.m_pGameSceneNode);

        // get bone array / bone matrix
        IntPtr boneMatrix = swed.ReadPointer(sceneNode, Offsets.m_modelState + 0x80); //0x80 is the dwBoneMatrix

        //đọc các thuộc tính đã lấy được từ 1 thực thể
        int health = swed.ReadInt(currentPawn, Offsets.m_iHealth);
        int team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
        uint lifeState = swed.ReadUInt(currentPawn, Offsets.m_lifeState);

        //nếu các thuộc tính được giữ nguyên, thì sẽ thêm vào danh sách thực thể - đã có xác định sống chết
        if (lifeState != 256)
            continue;

        if (team == localPlayer.team && !renderer.aimOnTeam) // chức năng aim cả đồng đội
            continue;

        Entity entity = new Entity();
        entity.pawnAddress = currentPawn;
        entity.controllerAddress = currentController;
        entity.health = health;
        entity.lifeState = lifeState;
        entity.origin = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
        entity.view = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset);
        entity.distance = Vector3.Distance(entity.origin, localPlayer.origin);
        entity.playerName = swed.ReadString(currentController, Offsets.m_iszPlayerName, 16);
        entity.head = swed.ReadVec(boneMatrix, 6 * 32); // 6 = bone id head, 32 = step between bone coordinates

        // get 2d Info
        ViewMaxtrix viewMatrix = ReadMatrix(client + Offsets.dwViewMatrix);

        //get head2d
        entity.head2d = Calculate.WorldToScreen(viewMatrix, entity.head, (int)screenSize.X, (int)screenSize.Y);

        //get distance form crosshair
        entity.pixcelDistance = Vector2.Distance(entity.head2d, new Vector2(screenSize.X / 2, screenSize.Y / 2));
        entities.Add(entity);

        // hiển thị ra console
        Console.ForegroundColor = ConsoleColor.Green;
        if (team != localPlayer.team)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        Console.WriteLine($"{entity.health}hp, distance: {(int)(entity.distance) / 100}m, Name: {entity.playerName}, head coordinates: {entity.head}");
        Console.ResetColor();
    }

    // sắp xếp theo khảng cách và ngắm
    entities = entities.OrderBy(o => o.pixcelDistance).ToList();

    if (entities.Count > 0 && GetAsyncKeyState(0x02) < 0 && renderer.fovAim)
    {
        //get view pos
        Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
        Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);

        //check if in FOV
        if (entities[0].pixcelDistance < renderer.FOV)
        {
            // get angles
            Vector2 newAngles = Calculate.CalculateAngles(playerView, entityView);
            Vector3 newAnglesVec3 = new Vector3(newAngles.Y, newAngles.X, 0.0f);

            //force new angles
            swed.WriteVec(client, Offsets.dwViewAngles, newAnglesVec3);
        }
    }
}

[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int Vkey); // handle

ViewMaxtrix ReadMatrix(IntPtr matrixAddress)
{
    var viewMatrix = new ViewMaxtrix();
    var matrix = swed.ReadMatrix(matrixAddress);

    // convert into our class

    // first row
    viewMatrix.m11 = matrix[0];
    viewMatrix.m12 = matrix[1];
    viewMatrix.m13 = matrix[2];
    viewMatrix.m14 = matrix[3];

    //second
    viewMatrix.m21 = matrix[4];
    viewMatrix.m22 = matrix[5];
    viewMatrix.m23 = matrix[6];
    viewMatrix.m24 = matrix[7];

    //third
    viewMatrix.m31 = matrix[8];
    viewMatrix.m32 = matrix[9];
    viewMatrix.m33 = matrix[10];
    viewMatrix.m34 = matrix[11];

    //fourth
    viewMatrix.m41 = matrix[12];
    viewMatrix.m42 = matrix[13];
    viewMatrix.m43 = matrix[14];
    viewMatrix.m44 = matrix[15];

    return viewMatrix;
}