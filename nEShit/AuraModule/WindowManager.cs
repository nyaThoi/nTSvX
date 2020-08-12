﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public class WindowManager
{
    private WindowManager(IntPtr ptr)
    {
        Pointer = ptr;
    }

    [CompilerGenerated]
    private sealed class WindowClass
    {
        internal bool Result(WindowManager wnd)
        {
            return wnd.WindowName == wndname;
        }
        public string wndname;
    }
    private IntPtr Pointer { get; set; }
    private enum windowTable
    {
               sTable_TSTO = 0xC,
               vTable_TSTO = 0x24
    }
    private string WindowName
    {
        get
        {
            return Memory.Reader.ReadSTDString(Pointer + (int)windowTable.sTable_TSTO, Encoding.UTF7);
        }
    }
    private static List<WindowManager> windowManagers
    {
        get
        {
            List<WindowManager> list = new List<WindowManager>();
            IntPtr intPtr = Memory.Reader.Read<IntPtr>(MemoryStore.WND_INTERFACE_BASE);
            IntPtr intPtr1 = intPtr;
            do
            {
                IntPtr intPtr3 = Memory.Reader.Read<IntPtr>(intPtr1 + (int)windowTable.vTable_TSTO);
                if (intPtr3 != IntPtr.Zero)
                {
                    list.Add(new WindowManager(intPtr3));
                }
                intPtr1 = Memory.Reader.Read<IntPtr>(intPtr1);
                if (!(intPtr1 != IntPtr.Zero))
                {
                    break;
                }
            }
            while (intPtr1 != intPtr);
            return list;
        }
    }
    private static WindowManager GetWindowByName(string wndName)
    {
        WindowClass windowClass = new WindowClass();
        windowClass.wndname = wndName;
        List<WindowManager> source = windowManagers.Where(new Func<WindowManager, bool>(windowClass.Result)).ToList<WindowManager>();
        if (source.Any<WindowManager>())
            return source.First<WindowManager>();
        return new WindowManager(IntPtr.Zero);
    }
    public static void PrintList()
    {
        Debug.WriteLine($"----- Beginn WND_INTERFACE -----");
        foreach (var window in windowManagers)
        {
            Console.WriteLine($"{window.WindowName}, {window.Pointer.ToString("X")}");
        }
        Debug.WriteLine($"----- end WND_INTERFACE -----");
    }

}

