using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Struct
{
    #region Entity
    public class Entity
    {
        public Entity(IntPtr intPtr)
        {
            Pointer = intPtr;
        }
        public IntPtr Pointer { get; set; }
        public bool IsValid
        {
            get
            {
                return Pointer != IntPtr.Zero;
            }
        }
        public uint entityID
        {
            get
            {
                return Memory.Reader.Read<uint>(Pointer + 0x1C);
            }
        }
        public EntityInfo GetEntityInfo
        {
            get
            {
                return new EntityInfo(Memory.Reader.Read<IntPtr>(Pointer + 0x20));
            }
        }
        public ModelInfo GetModelInfo
        {
            get
            {
                return new ModelInfo(Memory.Reader.Read<IntPtr>(Pointer + 0x24));
            }
        }
        public ActorInfo GetActorInfo
        {
            get
            {
                return new ActorInfo(Memory.Reader.Read<IntPtr>(Pointer + 0x40));
            }
        }

        public override string ToString()
        {
            return string.Concat(new string[]
            {
                "Entity Ptr: 0x",
                Pointer.ToString("X"),
                Environment.NewLine,
                "EntityInfo Ptr: 0x",
                GetEntityInfo.Pointer.ToString("X"),
                Environment.NewLine,
            });
        }

        #region ASM
        private IntPtr DoUIAction(int slotType)
        {
            string[] mnemonics =
                nMnemonics.localPlayer.DoUIAction(slotType, MemoryStore.PLAYER_DoUIAction);

            return Memory.Assemble.Execute<IntPtr>(mnemonics, "DoUIAction");
            //return Memory.Assemble.InjectAndExecute(mnemonics);

        }
        public void TeleportInterface()
        {
            DoUIAction(0x11);
        }
        #endregion

        public float SetMovementValue(float val = 20)
        {
            if (IsValid && GetEntityInfo.IsValid && GetEntityInfo.MovementValue != 0)
            {
                if (GetModelInfo.IsMounted)
                {
                    GetEntityInfo.MovementValue = val + 10;
                }
                else
                    GetEntityInfo.MovementValue = val;
            }
            return GetEntityInfo.MovementValue;
        }
        public void SetHeight(float val = 1000)
        {
            if(IsValid && GetModelInfo.IsValid && GetModelInfo.Height != 0)
                GetModelInfo.Height = val;
        }
    }
    public class EntityInfo
    {
        public EntityInfo(IntPtr intPtr)
        {
            Pointer = intPtr;
        }
        public IntPtr Pointer { get; set; }
        public bool IsValid
        {
            get
            {
                return Pointer != IntPtr.Zero;
            }
        }
        public uint currentHP
        {
            get
            {
                return Memory.Reader.Read<uint>(Pointer + 0xD0);
            }
        }
        public uint maxHP
        {
            get
            {
                return Memory.Reader.Read<uint>(Pointer + 0xD8);
            }
        }
        public uint level
        {
            get
            {
                return Memory.Reader.Read<uint>(Pointer + 0x14);
            }
        }
        public float MovementValue
        {
            get
            {
                return Memory.Reader.Read<float>(Pointer + 0x2C);
            }
            set
            {
                Memory.Writer.Write<float>(Pointer + 0x2C, value);
            }


        }
        public string charName
        {
            get
            {
                return Memory.Reader.ReadSTDString(Pointer + 0x17C, Encoding.UTF7);
            } 
        }

        public IntPtr inventoryPtr
        {
            get
            {
                return Memory.Reader.Read<IntPtr>(Pointer + 0x3E4);
            }
        }

    }
    public class ModelInfo
    {
        public ModelInfo(IntPtr intPtr)
        {
            Pointer = intPtr;
        }
        public IntPtr Pointer { get; set; }
        public bool IsValid
        {
            get
            {
                return Pointer != IntPtr.Zero;
            }
        }
        public bool IsMounted
        {
            get
            {
                return Memory.Reader.Read<uint>(Pointer + 0x234) != 0u;
            }
        }
        public float Height
        {
            get
            {
                return Memory.Reader.Read<float>(Pointer + 0x18C);
            }
            set
            {
                Memory.Writer.Write<float>(Pointer + 0x18C, value);
            }
        }
    }
    public class ActorInfo
    {
        public ActorInfo(IntPtr intPtr)
        {
            Pointer = intPtr;
        }
        public IntPtr Pointer { get; set; }
        public bool IsValid
        {
            get
            {
                return Pointer != IntPtr.Zero;
            }
        }
    }
    #endregion
    public class CurrentMap
    {
        public CurrentMap(IntPtr intPtr)
        {
            Pointer = intPtr;
        }
        public IntPtr Pointer { get; set; }
        public bool IsValid
        {
            get
            {
                return Pointer != IntPtr.Zero;
            }
        }
        public short mapID
        {
            get
            {
                return Memory.Reader.Read<short>(Pointer + 0x6);
            }
        }
        public bool onMap
        {
            get
            {
                return mapID > 0;
            }
        }
        public bool charSelection
        {
            get
            {
                return mapID == 0x63;
            }
        }
        public bool mapMeridia
        {
            get
            {
                return mapID == 0x64;
            }
        }
        public bool mapWesternContinent
        {
            get
            {
                return mapID == 0x65;
            }
        }
    }

    #region Inventory
    public class InventoryBag
    {
        public InventoryBag(IntPtr intPtr)
        {
            Pointer = intPtr;
        }
        public IntPtr Pointer { get; set; }
        public bool IsValid
        {
            get
            {
                return Pointer != IntPtr.Zero;
            }
        }
        private uint bagID
        {
            get
            {
                return Memory.Reader.Read<uint>(Pointer + 0x4);
            }
        }
        
        private InventoryItem begin
        {
            get
            {
                return new InventoryItem(Memory.Reader.Read<IntPtr>(Pointer + 0x8));
            }
        }
        private InventoryItem end
        {
            get
            {
                return new InventoryItem(Memory.Reader.Read<IntPtr>(Pointer + 0xC));
            }
        }

        private uint Begin
        {
            get
            {
                return Memory.Reader.Read<uint>(Pointer + 0x8);
            }
        }
        private uint End
        {
            get
            {
                return Memory.Reader.Read<uint>(Pointer + 0xC);
            }
        }

        public uint GetItemCount()
        {
            return (((uint)end.Pointer - (uint)begin.Pointer) >> 2);
        }
        public InventoryItem GetItem(uint index)
        {
            return (index < GetItemCount()) && begin.itemID == index ? begin : null;
        }

        public uint NumSlots
        {
            get
            {
                return ((End - Begin) / 4u);
            }
        }
        public uint UsedSlots
        {
            get
            {
                uint num = 0;
                IntPtr pointer = new IntPtr(Begin);
                uint num2 = 0;
                while (num2 < NumSlots && num2 <= 400)// IBT_MAX = 20 x20
                {
                    IntPtr intPtr = Memory.Reader.Read<IntPtr>(pointer + (int)num2 * 4);
                    if (intPtr != IntPtr.Zero && Memory.Reader.Read<IntPtr>(intPtr + 0) != IntPtr.Zero)
                    {
                        num++;
                    }
                    num2++;
                }
                return num;
            }
        }
        public uint FreeSlots
        {
            get
            {
                return NumSlots - UsedSlots;
            }
        }
    }
    public class InventoryItem
    {
        public InventoryItem(IntPtr intPtr)
        {
            Pointer = intPtr;
        }
        public IntPtr Pointer { get; set; }
        public bool IsValid
        {
            get
            {
                return Pointer != IntPtr.Zero;
            }
        }        
        public uint itemID
        {
            get
            {
                return Memory.Reader.Read<uint>(Pointer);
            }
        }
        public bool IsFilled
        {
            get
            {
                return itemID != 0;
            }
        }
    }
    #endregion

    #region Camera
    public class CameraStruct
    {
        public CameraStruct(IntPtr intPtr)
        {
            Pointer = intPtr;
        }
        public IntPtr Pointer { get; set; }
        public bool IsValid
        {
            get
            {
                return Pointer != IntPtr.Zero;
            }
        }
        public float Limiter
        {
            get
            {
                return Memory.Reader.Read<float>(Pointer + 0x54);
            }
            set
            {
                Memory.Writer.Write<float>(Pointer + 0x54, value);
            }
        }
        public float LimiterValue
        {
            get
            {
                return Memory.Reader.Read<float>(Pointer + 0x58);
            }
        }
    }

    #endregion
    #region WindowManager Struct

    #endregion
}
