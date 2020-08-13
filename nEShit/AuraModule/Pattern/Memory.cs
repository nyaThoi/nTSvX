using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MemoryStore
{
    public static IntPtr GET_LOCAL_PLAYER = IntPtr.Zero;
    public static IntPtr INVENTORY_ACCESS_FUNCTION = IntPtr.Zero;
    public static IntPtr TARGETING_COLLECTIONS_BASE = IntPtr.Zero;
    public static IntPtr WND_INTERFACE_BASE = IntPtr.Zero;

    public static IntPtr CURRENT_MAP_BASE = IntPtr.Zero;
    public static IntPtr DETOUR_MAIN_LOOP_OFFSET = IntPtr.Zero;

    public static IntPtr PLAYER_DoUIAction = IntPtr.Zero;

    public static IntPtr CAMERA_ACCESS_OFFSET = IntPtr.Zero;
}
public class Load_Pattern
{
    public static void SetPatterns()
    {
        System.Diagnostics.ProcessModule gameproc = Minimem.FindProcessModule("game.bin", false);
        MemoryStore.DETOUR_MAIN_LOOP_OFFSET = PatternManager.FindPattern(gameproc, "55 8B EC 83 EC 24 80 3D ? ? ? ? ?");
        
        MemoryStore.GET_LOCAL_PLAYER = PatternManager.FindPatternAlain(gameproc, "e8 ? ? ? ? 8b 4b ? 3b 48 ? 75", 0, 1, PatternManager.MemoryType.RT_READNEXT4_BYTES);
        MemoryStore.INVENTORY_ACCESS_FUNCTION = IntPtr.Zero;
        MemoryStore.TARGETING_COLLECTIONS_BASE = PatternManager.FindPatternAlain(gameproc, "83 3d ? ? ? ? ? 56 57 8b f1 75 ? e8 ? ? ? ? 8b 0d ? ? ? ? e8 ? ? ? ? 8b f8 0f bf 86", 1, 1, PatternManager.MemoryType.RT_READNEXT4_BYTES_RAW);
        MemoryStore.WND_INTERFACE_BASE = PatternManager.FindPatternAlain(gameproc, "a1 ? ? ? ? 8b 30 5b", 0, 1, PatternManager.MemoryType.RT_READNEXT4_BYTES_RAW);
        MemoryStore.CURRENT_MAP_BASE = PatternManager.FindPatternAlain(gameproc, "8b 0d ? ? ? ? 89 4e ? 89 35", 1, 1, PatternManager.MemoryType.RT_READNEXT4_BYTES_RAW);
        
        MemoryStore.PLAYER_DoUIAction = PatternManager.FindPatternAlain(gameproc, "e8 ? ? ? ? 83 c4 ? b0 ? c3 cc cc cc cc 55", 0, 1, PatternManager.MemoryType.RT_READNEXT4_BYTES);

        MemoryStore.CAMERA_ACCESS_OFFSET = PatternManager.FindPatternAlain(gameproc, "89 35 ? ? ? ? 5e b0", 1, 1, PatternManager.MemoryType.RT_READNEXT4_BYTES_RAW);

    }

    public static bool RetrieveAddresses(uint gamePID)
    {
        Attach.PID = gamePID;
        if (Attach.PID == 0) return false;
        Attach.OpenProcess();
        SetPatterns();//Load Patterns
#if DEBUG
        Debug.WriteLine($"----- Debug Pattern -----");
        Debug.WriteLine($"DETOUR_MAIN_LOOP_OFFSET 0x{MemoryStore.DETOUR_MAIN_LOOP_OFFSET.ToString("X")}\n");
        Debug.WriteLine($"GET_LOCAL_PLAYER 0x{MemoryStore.GET_LOCAL_PLAYER.ToString("X")}\n");
        Debug.WriteLine($"INVENTORY_ACCESS_FUNCTION 0x{MemoryStore.INVENTORY_ACCESS_FUNCTION.ToString("X")}\n");
        Debug.WriteLine($"TARGETING_COLLECTIONS_BASE 0x{MemoryStore.TARGETING_COLLECTIONS_BASE.ToString("X")}\n");
        Debug.WriteLine($"WND_INTERFACE_BASE 0x{MemoryStore.WND_INTERFACE_BASE.ToString("X")}\n");
        Debug.WriteLine($"CURRENT_MAP_BASE 0x{MemoryStore.CURRENT_MAP_BASE.ToString("X")}\n");

        Debug.WriteLine($"PLAYER_DoUIAction 0x{MemoryStore.PLAYER_DoUIAction.ToString("X")}\n");

        Debug.WriteLine($"----- End -----");
#endif
        if
            (
            MemoryStore.DETOUR_MAIN_LOOP_OFFSET == IntPtr.Zero ||
            MemoryStore.GET_LOCAL_PLAYER == IntPtr.Zero ||
            //MemoryStore.INVENTORY_ACCESS_FUNCTION == IntPtr.Zero ||
            MemoryStore.TARGETING_COLLECTIONS_BASE == IntPtr.Zero ||
            MemoryStore.WND_INTERFACE_BASE == IntPtr.Zero ||
            MemoryStore.CURRENT_MAP_BASE == IntPtr.Zero ||

            MemoryStore.PLAYER_DoUIAction == IntPtr.Zero

            )
            return true;
        else return false;
    }
}
